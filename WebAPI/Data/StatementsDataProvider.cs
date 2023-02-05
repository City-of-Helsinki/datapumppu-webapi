using System.Collections.Concurrent;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    public interface IStatementsDataProvider
    {
        Task<List<StatementDTO>?> GetStatements(string meetingId, string caseNumber);

        Task ResetCache();
    }

    public class StatementsDataCache
    {
        public DateTime Timestamp { get; set; }

        public List<StatementDTO>? Statements { get; set; }
    }


    public class StatementsDataProvider : IStatementsDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, StatementsDataCache> _dataCache = new ConcurrentDictionary<string, StatementsDataCache>();
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
