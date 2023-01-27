using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.LiveMeetings
{
    public class KafkaLiveMeetingObserver : BackgroundService
    {
        private readonly IHubContext<LiveMeetingsHub> _hub;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaLiveMeetingObserver> _logger;
        private IHostEnvironment _hostEnvironment;

        public KafkaLiveMeetingObserver(
            IHubContext<LiveMeetingsHub> hub,
            IConfiguration configuration,
            ILogger<KafkaLiveMeetingObserver> logger,
            IHostEnvironment hostEnvironment)
        {
            _hub = hub;
            _configuration = configuration;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => MessageHandler(stoppingToken), stoppingToken);
        }

        private async void MessageHandler(CancellationToken stoppingToken)
        {
            var topic = _configuration["KAFKA_CONSUMER_TOPIC"];
            var consumer = CreateConsumer();

            consumer.Subscribe(topic);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(stoppingToken);
                    var message = cr.Message.Value;
                    _logger.LogInformation(message);

                    await _hub.Clients.All.SendAsync("receiveMessage", message);

                    consumer.Commit(cr);
                    _logger.LogInformation("Live Meeting Consumer event successfully received.");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Live Meeting Consumer Operation Canceled.");
                    break;
                }
                catch (ConsumeException e)
                {
                    _logger.LogError("Live Meeting Consumer Error: " + e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError("Live Meeting Consumer Unexpected Error: " + e.Message);
                }
            }
        }

        private IConsumer<Null, string> CreateConsumer()
        {
            var groupId = Guid.NewGuid().ToString();
            var config = CreateConsumerConfiguration(groupId);

            return new ConsumerBuilder<Null, string>(config).Build();
        }

        private ConsumerConfig CreateConsumerConfiguration(string groupId)
        {
            if (_hostEnvironment.IsDevelopment())
            {
                return new ConsumerConfig
                {
                    BootstrapServers = _configuration["KAFKA_BOOTSTRAP_SERVER"],
                    GroupId = groupId,
                };
            }

            var cert = ParseCert(_configuration["SSL_CERT_PEM"]);

            return new ConsumerConfig
            {
                BootstrapServers = _configuration["KAFKA_BOOTSTRAP_SERVER"],
                GroupId = groupId,
                SaslMechanism = SaslMechanism.ScramSha512,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = _configuration["KAFKA_USER_USERNAME"],
                SaslPassword = _configuration["KAFKA_USER_PASSWORD"],
                SslCaPem = cert,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        private string ParseCert(string cert)
        {
            // To prevent pipeline errors the keyvault ca.crt is in quotes and without the begin/end tags. 
            cert = cert.Replace("\"", "");

            var certBegin = "-----BEGIN CERTIFICATE-----\n";
            var certEnd = "\n-----END CERTIFICATE-----";

            return certBegin + cert + certEnd;
        }
    }
}
