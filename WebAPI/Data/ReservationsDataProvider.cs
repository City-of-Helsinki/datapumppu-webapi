using System.Collections.Concurrent;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    /// <summary>
    /// Provides cached access to reservation data from the storage API.
    /// </summary>
    public interface IReservationsDataProvider
    {
        /// <summary>
        /// Retrieves reservations for a specific meeting case.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="caseNumber">The case number.</param>
        /// <returns>A list of reservations, or null if not found.</returns>
        Task<List<ReservationDTO>?> GetReservations(string meetingId, string caseNumber);

        /// <summary>
        /// Clears all cached reservation data.
        /// </summary>
        Task ResetCache();
    }

    /// <summary>
    /// Caching data provider for reservation data. Caches results for 5 minutes.
    /// Thread-safe via <see cref="SemaphoreSlim"/>.
    /// </summary>
    public class ReservationsDataProvider : IReservationsDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, DataCache> _dataCache = new Dictionary<string, DataCache>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public ReservationsDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ResetCache()
        {
            await _semaphore.WaitAsync();
            _dataCache.Clear();
            _semaphore.Release();
        }

        public async Task<List<ReservationDTO>?> GetReservations(string meetingId, string caseNumber)
        {
            var dataKey = $"{meetingId}-{caseNumber}";
            await _semaphore.WaitAsync();

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
                _semaphore.Release();
            }
        }

        class DataCache
        {
            public DateTime Timestamp { get; set; }

            public List<ReservationDTO>? Reservations { get; set; }
        }

    }
}
