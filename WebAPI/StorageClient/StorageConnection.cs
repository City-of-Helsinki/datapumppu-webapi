namespace WebAPI.StorageClient
{
    /// <summary>
    /// Creates HTTP connections to the external storage API.
    /// </summary>
    public interface IStorageConnection
    {
        /// <summary>
        /// Creates a new <see cref="HttpClient"/> configured with the storage API base address.
        /// </summary>
        /// <returns>An <see cref="HttpClient"/> instance pointing to the storage service.</returns>
        HttpClient CreateConnection();
    }

    /// <summary>
    /// Default implementation of <see cref="IStorageConnection"/> using the STORAGE_URL configuration.
    /// </summary>
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
