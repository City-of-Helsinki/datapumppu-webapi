using WebAPI.StorageClient;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.Data.Statistics
{
    public interface IPersonStatementStatisticsDataProvider
    {
        Task<List<StoragePersonStatementStatisticsDTO>?> GetStatements(int year);
    }


    public class PersonStatementStatisticsDataProvider : IPersonStatementStatisticsDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, DataCache> _dataCache = new Dictionary<string, DataCache>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public PersonStatementStatisticsDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<List<StoragePersonStatementStatisticsDTO>?> GetStatements(int year)
        {
            var dataKey = $"{year}";
            await _semaphore.WaitAsync();

            try
            {
                if (_dataCache.TryGetValue(dataKey, out DataCache? dataCache))
                {
                    if (dataCache?.Timestamp > DateTime.UtcNow.AddDays(-1))
                    {
                        return dataCache.Data;
                    }
                }

                var scope = _serviceProvider.CreateScope();
                var apiClient = scope.ServiceProvider.GetService<IStorageApiClient>();
                if (apiClient == null)
                {
                    throw new InvalidOperationException();
                }

                var statements = await apiClient.RequestPersonStatementStatistics(year);
                _dataCache[dataKey] = new DataCache
                {
                    Data = statements,
                    Timestamp = DateTime.UtcNow,
                };

                return statements;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        class DataCache
        {
            public DateTime Timestamp { get; set; }

            public List<StoragePersonStatementStatisticsDTO>? Data { get; set; }
        }
    }
}
