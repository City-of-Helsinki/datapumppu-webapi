namespace WebAPI.Data
{
    public interface ICache
    {
        Task ResetCache();
    }

    public class Cache : ICache
    {
        private readonly IStatementsDataProvider _statementsDataProvider;
        private readonly IVotingDataProvider _votingDataProvider;
        private readonly ISeatsDataProvider _seatsDataProvider;

        public Cache(IStatementsDataProvider statementsDataProvider,
            IVotingDataProvider votingDataProvider,
            ISeatsDataProvider seatsDataProvider)
        {
            _statementsDataProvider = statementsDataProvider;
            _votingDataProvider = votingDataProvider;
            _seatsDataProvider = seatsDataProvider;
        }

        public async Task ResetCache()
        {
            await _statementsDataProvider.ResetCache();
            await _votingDataProvider.ResetCache();
            await _seatsDataProvider.ResetCache();
        }
    }
}
