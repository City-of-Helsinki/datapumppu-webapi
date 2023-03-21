using System.Collections.Concurrent;
using System.Threading;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    public interface IAgendaSubItemsProvider
    {
        Task<List<StorageAgendaSubItemDTO>> GetAgendaPointSubItems(string meetingId, int agendaPoint);

        Task ResetCache();
    }

    public class AgendaSubItemDataCache
    {
        public DateTime Timestamp { get; set; }

        public List<StorageAgendaSubItemDTO> Items { get; set; } = new List<StorageAgendaSubItemDTO>();
    }


    public class AgendaSubItemsProvider : IAgendaSubItemsProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, AgendaSubItemDataCache> _dataCache = new ConcurrentDictionary<string, AgendaSubItemDataCache>();
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
