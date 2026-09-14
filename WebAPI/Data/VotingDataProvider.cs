using System.Collections.Concurrent;
using WebAPI.StorageClient;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.Data
{
    /// <summary>
    /// Provides cached access to voting data from the storage API.
    /// </summary>
    public interface IVotingDataProvider
    {
        /// <summary>
        /// Retrieves voting records for a specific meeting case.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="caseNumber">The case number.</param>
        /// <returns>A list of voting records, or null if not found.</returns>
        Task<List<StorageVotingDTO>?> GetVoting(string meetingId, string caseNumber);

        /// <summary>
        /// Clears all cached voting data.
        /// </summary>
        Task ResetCache();
    }

    /// <summary>
    /// Cache entry for voting data with a timestamp for expiration.
    /// </summary>
    public class VoteDataCache
    {
        public DateTime Timestamp { get; set; }

        public List<StorageVotingDTO>? Voting { get; set; }
    }


    /// <summary>
    /// Caching data provider for voting data. Caches results for 5 minutes.
    /// Thread-safe via <see cref="SemaphoreSlim"/>.
    /// </summary>
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
