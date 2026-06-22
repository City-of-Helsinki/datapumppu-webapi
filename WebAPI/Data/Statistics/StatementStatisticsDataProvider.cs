using System.Collections.Concurrent;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.Data.Statistics
{
    /// <summary>
    /// Provides cached access to per-issue statement statistics from the storage API.
    /// </summary>
    public interface IStatementStatisticsDataProvider
    {
        /// <summary>
        /// Retrieves per-issue statement statistics for a given year.
        /// </summary>
        /// <param name="year">The year to retrieve statistics for.</param>
        /// <returns>A list of statement statistics, or null if not found.</returns>
        Task<List<StorageStatementStatisticsDTO>?> GetStatements(int year);
    }

    /// <summary>
    /// Caching data provider for per-issue statement statistics. Caches results for 1 day.
    /// Thread-safe via <see cref="SemaphoreSlim"/>.
    /// </summary>
    public class StatementStatisticsDataProvider : IStatementStatisticsDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, DataCache> _dataCache = new Dictionary<string, DataCache>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public StatementStatisticsDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<List<StorageStatementStatisticsDTO>?> GetStatements(int year)
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

                var statements = await apiClient.RequestStatementStatistics(year);
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

            public List<StorageStatementStatisticsDTO>? Data { get; set; }
        }
    }
}
