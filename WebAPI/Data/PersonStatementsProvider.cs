using System.Collections.Concurrent;
using System.Threading;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    /// <summary>
    /// Provides cached access to person-specific statement data from the storage API.
    /// </summary>
    public interface IPersonStatementsProvider
    {
        /// <summary>
        /// Retrieves statements for a person by name, year, and language.
        /// </summary>
        /// <param name="personName">The person's name.</param>
        /// <param name="year">The year to query.</param>
        /// <param name="lang">The language code (en, fi, or sv).</param>
        /// <returns>A list of statements for the specified person.</returns>
        Task<List<StatementDTO>> GetStatements(string personName, int year, string lang);

        /// <summary>
        /// Retrieves statements by person name(s) and/or date range.
        /// </summary>
        /// <param name="names">Comma-separated person names.</param>
        /// <param name="startDate">Start date filter.</param>
        /// <param name="endDate">End date filter.</param>
        /// <param name="lang">The language code (en, fi, or sv).</param>
        /// <returns>A list of matching statements.</returns>
        Task<List<StatementDTO>> GetStatementsLookup(string? names, string? startDate, string? endDate, string lang);

        /// <summary>
        /// Clears all cached person statement data.
        /// </summary>
        Task ResetCache();
    }

    /// <summary>
    /// Caching data provider for person statements. Caches results for 1 hour.
    /// Thread-safe via <see cref="SemaphoreSlim"/>.
    /// </summary>
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
