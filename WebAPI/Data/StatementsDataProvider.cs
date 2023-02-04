using System.Collections.Concurrent;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Data
{
    public interface IStatementsDataProvider
    {
        Task<List<StatementDTO>?> GetStatements(string meetingId, string caseNumber);

        void ResetCache();
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

        public StatementsDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ResetCache()
        {
            _dataCache.Clear();
        }

        public async Task<List<StatementDTO>?> GetStatements(string meetingId, string caseNumber)
        {
            var dataKey = $"{meetingId}-{caseNumber}";

            if (_dataCache.TryGetValue(dataKey, out StatementsDataCache? dataCache))
            {
                if (dataCache?.Timestamp > DateTime.UtcNow.AddMinutes(-5))
                {
                    return dataCache.Statements;
                }
            }

            var scope = _serviceProvider.CreateScope();
            var apiClient = scope.ServiceProvider.GetService<IStorageApiClient>();
            if (apiClient== null)
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
    }
}
