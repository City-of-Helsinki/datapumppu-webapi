using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using WebAPI.Data;

namespace WebAPI.LiveMeetings
{
    public class KafkaLiveMeetingObserver : BackgroundService
    {
        private readonly IHubContext<LiveMeetingsHub> _hub;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaLiveMeetingObserver> _logger;
        private IHostEnvironment _hostEnvironment;
        private readonly ICache _cache;

        private readonly Dictionary<string, DateTime> _latestSignals = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, Waiter> _waiters = new Dictionary<string, Waiter>();

        public KafkaLiveMeetingObserver(
            IHubContext<LiveMeetingsHub> hub,
            IConfiguration configuration,
            ILogger<KafkaLiveMeetingObserver> logger,
            ICache cache,
            IHostEnvironment hostEnvironment)
        {
            _hub = hub;
            _configuration = configuration;
            _logger = logger;
            _cache = cache;
            _hostEnvironment = hostEnvironment;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => MessageHandler(stoppingToken), stoppingToken);
        }

        private async void MessageHandler(CancellationToken stoppingToken)
        {
            const int WaitTimeMS = 2000;
            var topic = _configuration["KAFKA_CONSUMER_TOPIC"];
            var consumer = CreateConsumer();

            consumer.Subscribe(topic);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(1000);
                    if (cr != null && !string.IsNullOrEmpty(cr.Message.Value))
                    {
                        var message = JsonConvert.DeserializeObject<StorageEventDTO>(cr.Message.Value);
                        _logger.LogInformation("message received: " + cr.Message.Value);

                        var key = message.MeetingId + "-" + message.CaseNumber;
                        if (!_waiters.ContainsKey(key))
                        {
                            _waiters[key] = new Waiter
                            {
                                Timestamp = DateTime.Now,
                                Message = message,
                            };
                        }

                        consumer.Commit(cr);
                        _logger.LogInformation("Live Meeting Consumer event successfully received.");
                    }

                    foreach (var waiterKey in _waiters.Keys.ToList())
                    {
                        if (_waiters[waiterKey].Timestamp < DateTime.Now.AddMilliseconds(-WaitTimeMS))
                        {
                            _logger.LogInformation("Cache reset key: " + waiterKey);
                            await _cache.ResetCache(_waiters[waiterKey].Message?.IsLiveEvent);
                            if (_waiters[waiterKey].Message?.IsLiveEvent == true)
                            {
                                await _hub.Clients.All.SendAsync("receiveMessage", _waiters[waiterKey].Message);
                            }
                            _waiters.Remove(waiterKey);
                        }
                    }
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
                AutoOffsetReset = AutoOffsetReset.Latest
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

        private class Waiter
        {
            public DateTime Timestamp { get; set; }

            public StorageEventDTO? Message { get; set; }
        }
    }
}
