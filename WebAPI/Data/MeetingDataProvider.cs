using System.Collections.Concurrent;
using System.Threading;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    public interface IMeetingDataProvider
    {
        Task<StorageMeetingDTO?> GetMeeting(string year, string sequenceNumber, string language);

        Task ResetCache();
    }

    public class MeetingDataCache
    {
        public DateTime Timestamp { get; set; }

        public StorageMeetingDTO? Meeting { get; set; }
    }


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
                    if (meetingDataCache?.Timestamp > DateTime.UtcNow.AddMinutes(-2))
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
