namespace WebAPI.StorageClient
{
    public interface IStorageConnection
    {
        HttpClient CreateConnection();
    }

    public class StorageConnection : IStorageConnection
    {
        private readonly IConfiguration _configuration;

        public StorageConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public HttpClient CreateConnection()
        {
            var connection = new HttpClient();
            connection.BaseAddress = new Uri(_configuration["STORAGE_URL"]);
            return connection;
        }
    }
}
