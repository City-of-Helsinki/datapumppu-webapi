using System.Collections.Concurrent;
using System.Threading;
using WebAPI.StorageClient;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.Data
{
    /// <summary>
    /// Provides cached access to meeting data from the storage API.
    /// </summary>
    public interface IMeetingDataProvider
    {
        /// <summary>
        /// Retrieves a meeting by year, sequence number, and language.
        /// </summary>
        /// <param name="year">The meeting year.</param>
        /// <param name="sequenceNumber">The sequence number within the year.</param>
        /// <param name="language">The language code (en, fi, or sv).</param>
        /// <returns>The meeting data, or null if not found.</returns>
        Task<StorageMeetingDTO?> GetMeeting(string year, string sequenceNumber, string language);

        /// <summary>
        /// Clears all cached meeting data.
        /// </summary>
        Task ResetCache();
    }

    /// <summary>
    /// Cache entry for meeting data with a timestamp for expiration.
    /// </summary>
    public class MeetingDataCache
    {
        public DateTime Timestamp { get; set; }

        public StorageMeetingDTO? Meeting { get; set; }
    }


    /// <summary>
    /// Caching data provider for meeting data. Caches results for 5 minutes.
    /// Thread-safe via <see cref="SemaphoreSlim"/>.
    /// </summary>
    public class MeetingDataProvider : IMeetingDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, MeetingDataCache> _dataCache = new ConcurrentDictionary<string, MeetingDataCache>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public MeetingDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ResetCache()
        {
            await _semaphore.WaitAsync();
            _dataCache.Clear();
            _semaphore.Release();
        }

        public async Task<StorageMeetingDTO?> GetMeeting(string year, string sequenceNumber, string language)
        {
            var dataKey = $"{year}-{sequenceNumber}-{language}";
            await _semaphore.WaitAsync();
            try
            {
                if (_dataCache.TryGetValue(dataKey, out MeetingDataCache? meetingDataCache))
                {
                    if (meetingDataCache?.Timestamp > DateTime.UtcNow.AddMinutes(-5))
                    {
                        return meetingDataCache.Meeting;
                    }
                }

                var scope = _serviceProvider.CreateScope();
                var apiClient = scope.ServiceProvider.GetService<IStorageApiClient>();
                if (apiClient == null)
                {
                    throw new InvalidOperationException();
                }

                var meeting = await apiClient.RequestMeeting(year, sequenceNumber, language);
                _dataCache[dataKey] = new MeetingDataCache
                {
                    Meeting = meeting,
                    Timestamp = DateTime.UtcNow,
                };

                return meeting;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
