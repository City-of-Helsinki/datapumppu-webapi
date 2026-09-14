using System.Collections.Concurrent;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    /// <summary>
    /// Provides cached access to statement data from the storage API.
    /// </summary>
    public interface IStatementsDataProvider
    {
        /// <summary>
        /// Retrieves statements for a specific meeting case.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="caseNumber">The case number.</param>
        /// <returns>A list of statements, or null if not found.</returns>
        Task<List<StatementDTO>?> GetStatements(string meetingId, string caseNumber);

        /// <summary>
        /// Clears all cached statement data.
        /// </summary>
        Task ResetCache();
    }

    /// <summary>
    /// Cache entry for statement data with a timestamp for expiration.
    /// </summary>
    public class StatementsDataCache
    {
        public DateTime Timestamp { get; set; }

        public List<StatementDTO>? Statements { get; set; }
    }


    /// <summary>
    /// Caching data provider for statement data. Caches results for 5 minutes.
    /// Thread-safe via <see cref="SemaphoreSlim"/>.
    /// </summary>
    public class StatementsDataProvider : IStatementsDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, StatementsDataCache> _dataCache = new Dictionary<string, StatementsDataCache>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public StatementsDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ResetCache()
        {
            await _semaphore.WaitAsync();
            _dataCache.Clear();
            _semaphore.Release();
        }

        public async Task<List<StatementDTO>?> GetStatements(string meetingId, string caseNumber)
        {
            var dataKey = $"{meetingId}-{caseNumber}";
            await _semaphore.WaitAsync();

            try
            {
                if (_dataCache.TryGetValue(dataKey, out StatementsDataCache? dataCache))
                {
                    if (dataCache?.Timestamp > DateTime.UtcNow.AddMinutes(-5))
                    {
                        return dataCache.Statements;
                    }
                }

                var scope = _serviceProvider.CreateScope();
                var apiClient = scope.ServiceProvider.GetService<IStorageApiClient>();
                if (apiClient == null)
                {
                    throw new InvalidOperationException();
                }

                var statements = await apiClient.GetStatements(meetingId, caseNumber);
                _dataCache[dataKey] = new StatementsDataCache
                {
                    Statements = statements,
                    Timestamp = DateTime.UtcNow,
                };

                return statements;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
