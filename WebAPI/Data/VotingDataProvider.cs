using System.Collections.Concurrent;
using WebAPI.StorageClient;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.Data
{
    public interface IVotingDataProvider
    {
        Task<List<StorageVotingDTO>?> GetVoting(string meetingId, string caseNumber);

        Task ResetCache();
    }

    public class VoteDataCache
    {
        public DateTime Timestamp { get; set; }

        public List<StorageVotingDTO>? Voting { get; set; }
    }


    public class VotingDataProvider : IVotingDataProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, VoteDataCache> _dataCache = new Dictionary<string, VoteDataCache>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public VotingDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ResetCache()
        {
            await _semaphore.WaitAsync();
            _dataCache.Clear();
            _semaphore.Release();
        }

        public async Task<List<StorageVotingDTO>?> GetVoting(string meetingId, string caseNumber)
        {
            var dataKey = $"{meetingId}-{caseNumber}";

            await _semaphore.WaitAsync();
            try
            {
                if (_dataCache.TryGetValue(dataKey, out VoteDataCache? dataCache))
                {
                    if (dataCache?.Timestamp > DateTime.UtcNow.AddMinutes(-5))
                    {
                        return dataCache.Voting;
                    }
                }

                var scope = _serviceProvider.CreateScope();
                var apiClient = scope.ServiceProvider.GetService<IStorageApiClient>();
                if (apiClient == null)
                {
                    throw new InvalidOperationException();
                }

                var voting = await apiClient.RequestVote(meetingId, caseNumber);
                _dataCache[dataKey] = new VoteDataCache
                {
                    Voting = voting,
                    Timestamp = DateTime.UtcNow,
                };

                return voting;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
