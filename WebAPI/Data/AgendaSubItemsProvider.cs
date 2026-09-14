using System.Collections.Concurrent;
using System.Threading;
using WebAPI.StorageClient;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.Data
{
    /// <summary>
    /// Provides cached access to agenda point sub-item data from the storage API.
    /// </summary>
    public interface IAgendaSubItemsProvider
    {
        /// <summary>
        /// Retrieves sub-items for a specific agenda point in a meeting.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="agendaPoint">The agenda point number.</param>
        /// <returns>A list of agenda sub-items.</returns>
        Task<List<StorageAgendaSubItemDTO>> GetAgendaPointSubItems(string meetingId, int agendaPoint);

        /// <summary>
        /// Clears all cached agenda sub-item data.
        /// </summary>
        Task ResetCache();
    }

    /// <summary>
    /// Cache entry for agenda sub-item data with a timestamp for expiration.
    /// </summary>
    public class AgendaSubItemDataCache
    {
        public DateTime Timestamp { get; set; }

        public List<StorageAgendaSubItemDTO> Items { get; set; } = new List<StorageAgendaSubItemDTO>();
    }


    /// <summary>
    /// Caching data provider for agenda sub-items. Caches results for 5 minutes.
    /// Thread-safe via <see cref="SemaphoreSlim"/>.
    /// </summary>
    public class AgendaSubItemsProvider : IAgendaSubItemsProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, AgendaSubItemDataCache> _dataCache = new Dictionary<string, AgendaSubItemDataCache>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public AgendaSubItemsProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ResetCache()
        {
            await _semaphore.WaitAsync();
            _dataCache.Clear();
            _semaphore.Release();
        }

        public async Task<List<StorageAgendaSubItemDTO>> GetAgendaPointSubItems(string meetingId, int agendaPoint)
        {
            var dataKey = $"{meetingId}-{agendaPoint}";
            await _semaphore.WaitAsync();
            try
            {
                if (_dataCache.TryGetValue(dataKey, out AgendaSubItemDataCache? dataCache))
                {
                    if (dataCache?.Timestamp > DateTime.UtcNow.AddMinutes(-5))
                    {
                        return dataCache.Items;
                    }
                }

                var scope = _serviceProvider.CreateScope();
                var apiClient = scope.ServiceProvider.GetService<IStorageApiClient>();
                if (apiClient == null)
                {
                    throw new InvalidOperationException();
                }

                var items = await apiClient.RequestAgendaPointSubItemsg(meetingId, agendaPoint);
                _dataCache[dataKey] = new AgendaSubItemDataCache
                {
                    Items = items,
                    Timestamp = DateTime.UtcNow,
                };

                return items;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
