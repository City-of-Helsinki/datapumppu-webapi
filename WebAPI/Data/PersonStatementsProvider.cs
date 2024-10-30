using System.Collections.Concurrent;
using System.Threading;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    public interface IPersonStatementsProvider
    {
        Task<List<StatementDTO>> GetStatements(string personName, int year, string lang);

        Task<List<StatementDTO>> GetStatementsLookup(string? names, string? startDate, string? endDate, string lang);

        Task ResetCache();
    }


    public class PersonStatementsProvider : IPersonStatementsProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, DataCache> _dataCache = new Dictionary<string, DataCache>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public PersonStatementsProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ResetCache()
        {
            await _semaphore.WaitAsync();
            _dataCache.Clear();
            _semaphore.Release();
        }

        public async Task<List<StatementDTO>> GetStatementsLookup(string? names, string? startDate, string? endDate, string lang)
        {
            var dataKey = $"{names}-{startDate}-{endDate}-{lang}";
            try
            {
                await _semaphore.WaitAsync();

                if (_dataCache.TryGetValue(dataKey, out DataCache? dataCache))
                {
                    if (dataCache?.Timestamp > DateTime.UtcNow.AddHours(-1))
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

                var items = await apiClient.GetStatementsByPersonOrDate(names, startDate, endDate, lang);
                _dataCache[dataKey] = new DataCache
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

        public async Task<List<StatementDTO>> GetStatements(string personName, int year, string lang)
        {
            var dataKey = $"{personName}-{year}-{lang}";
            try
            {
                await _semaphore.WaitAsync();

                if (_dataCache.TryGetValue(dataKey, out DataCache? dataCache))
                {
                    if (dataCache?.Timestamp > DateTime.UtcNow.AddHours(-1))
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

                var items = await apiClient.GetStatementsByPerson(personName, year, lang);
                _dataCache[dataKey] = new DataCache
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

        class DataCache
        {
            public DateTime Timestamp { get; set; }

            public List<StatementDTO> Items { get; set; } = new List<StatementDTO>();
        }
    }
}
