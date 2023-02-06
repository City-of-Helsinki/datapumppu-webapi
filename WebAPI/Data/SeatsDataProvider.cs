using Microsoft.Extensions.Azure;
using System.Collections.Concurrent;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    public interface ISeatsDataProvider
    {
        Task<List<SeatDTO>?> GetSeats(string meetingId, string caseNumber);

        Task ResetCache();
    }

    class DataCache
    {
        public DateTime Timestamp { get; set; }

        public List<SeatDTO>? Seats { get; set; }
    }


    public class SeatsDataProvider : ISeatsDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, DataCache> _dataCache = new ConcurrentDictionary<string, DataCache>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public SeatsDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ResetCache()
        {
            await _semaphore.WaitAsync();
            _dataCache.Clear();
            _semaphore.Release();
        }

        public async Task<List<SeatDTO>?> GetSeats(string meetingId, string caseNumber)
        {
            var dataKey = $"{meetingId}-{caseNumber}";
            await _semaphore.WaitAsync();

            try
            {
                if (_dataCache.TryGetValue(dataKey, out DataCache? dataCache))
                {
                    if (dataCache?.Timestamp > DateTime.UtcNow.AddMinutes(-5))
                    {
                        return dataCache.Seats;
                    }
                }

                var scope = _serviceProvider.CreateScope();
                var apiClient = scope.ServiceProvider.GetService<IStorageApiClient>();
                if (apiClient == null)
                {
                    throw new InvalidOperationException();
                }

                var seats = await apiClient.RequestSeats(meetingId, caseNumber);
                _dataCache[dataKey] = new DataCache
                {
                    Seats = seats,
                    Timestamp = DateTime.UtcNow,
                };

                return seats;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
