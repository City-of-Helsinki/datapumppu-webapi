namespace WebAPI.Data
{
    public interface ICache
    {
        void ResetCache();
    }

    public class Cache : ICache
    {
        private readonly IStatementsDataProvider _statementsDataProvider;
        private readonly IVotingDataProvider _votingDataProvider;

        public Cache(IStatementsDataProvider statementsDataProvider,
            IVotingDataProvider votingDataProvider)
        {
            _statementsDataProvider = statementsDataProvider;
            _votingDataProvider = votingDataProvider;
        }

        public void ResetCache()
        {
            _statementsDataProvider.ResetCache();
            _votingDataProvider.ResetCache();
        }
    }
}
