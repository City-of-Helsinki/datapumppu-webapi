using System.Collections.Concurrent;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    public interface IReservationsDataProvider
    {
        Task<List<ReservationDTO>?> GetReservations(string meetingId, string caseNumber);

        Task ResetCache();
    }


    public class ReservationsDataProvider : IReservationsDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, DataCache> _dataCache = new ConcurrentDictionary<string, DataCache>();
        //private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public ReservationsDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ResetCache()
        {
            //await _semaphore.WaitAsync();
            _dataCache.Clear();
            //_semaphore.Release();
        }

        public async Task<List<ReservationDTO>?> GetReservations(string meetingId, string caseNumber)
        {
            var dataKey = $"{meetingId}-{caseNumber}";
            //await _semaphore.WaitAsync();

            try
            {
                if (_dataCache.TryGetValue(dataKey, out DataCache? dataCache))
                {
                    if (dataCache?.Timestamp > DateTime.UtcNow.AddMinutes(-5))
                    {
                        return dataCache.Reservations;
                    }
                }

                var scope = _serviceProvider.CreateScope();
                var apiClient = scope.ServiceProvider.GetService<IStorageApiClient>();
                if (apiClient == null)
                {
                    throw new InvalidOperationException();
                }

                var reservations = await apiClient.GetReservations(meetingId, caseNumber);
                _dataCache[dataKey] = new DataCache
                {
                    Reservations = reservations,
                    Timestamp = DateTime.UtcNow,
                };

                return reservations;
            }
            finally
            {
                //_semaphore.Release();
            }
        }

        class DataCache
        {
            public DateTime Timestamp { get; set; }

            public List<ReservationDTO>? Reservations { get; set; }
        }

    }
}
