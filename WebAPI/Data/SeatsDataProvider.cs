using Microsoft.Extensions.Azure;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    /// <summary>
    /// Provides cached access to seating data from the storage API.
    /// </summary>
    public interface ISeatsDataProvider
    {
        /// <summary>
        /// Retrieves seat assignments for a specific meeting case.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="caseNumber">The case number.</param>
        /// <returns>A list of seat layout snapshots, or null if not found.</returns>
        Task<List<WebApiSeatsDTO>?> GetSeats(string meetingId, string caseNumber);

        /// <summary>
        /// Clears all cached seating data.
        /// </summary>
        Task ResetCache();
    }

    /// <summary>
    /// Caching data provider for seating data. Caches results for 5 minutes.
    /// Thread-safe via <see cref="SemaphoreSlim"/>.
    /// </summary>
    public class SeatsDataProvider : ISeatsDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, DataCache> _dataCache = new Dictionary<string, DataCache>();
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

        public async Task<List<WebApiSeatsDTO>?> GetSeats(string meetingId, string caseNumber)
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

        class DataCache
        {
            public DateTime Timestamp { get; set; }

            public List<WebApiSeatsDTO>? Seats { get; set; }
        }
    }
}
