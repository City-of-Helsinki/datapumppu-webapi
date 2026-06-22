using System.Collections.Concurrent;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.Data.Statistics
{
    /// <summary>
    /// Provides cached access to participant attendance statistics from the storage API.
    /// </summary>
    public interface IParticipantStatisticsDataProvider
    {
        /// <summary>
        /// Retrieves participant attendance statistics for a given year.
        /// </summary>
        /// <param name="year">The year to retrieve statistics for.</param>
        /// <returns>A list of participant statistics, or null if not found.</returns>
        Task<List<ParticipationsPersonDTO>?> GetStatistics(int year);
    }

    /// <summary>
    /// Caching data provider for participant statistics. Caches results for 1 day.
    /// Thread-safe via <see cref="SemaphoreSlim"/>.
    /// </summary>
    public class ParticipantStatisticsDataProvider : IParticipantStatisticsDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, DataCache> _dataCache = new Dictionary<string, DataCache>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public ParticipantStatisticsDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<List<ParticipationsPersonDTO>?> GetStatistics(int year)
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

                var votings = await apiClient.RequestParticipantStatistics(year);
                _dataCache[dataKey] = new DataCache
                {
                    Data = votings,
                    Timestamp = DateTime.UtcNow,
                };

                return votings;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        class DataCache
        {
            public DateTime Timestamp { get; set; }

            public List<ParticipationsPersonDTO>? Data { get; set; }
        }

    }
}
