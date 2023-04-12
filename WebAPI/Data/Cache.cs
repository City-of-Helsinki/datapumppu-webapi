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
        private readonly IAgendaSubItemsProvider _agendaSubItemsProvider;
        private readonly IReservationsDataProvider _reservationsDataProvider;
        private readonly IPersonStatementsProvider _personStatementsProvider;

        public Cache(IStatementsDataProvider statementsDataProvider,
            IVotingDataProvider votingDataProvider,
            ISeatsDataProvider seatsDataProvider,
            IAgendaSubItemsProvider agendaSubItemsProvider,
            IReservationsDataProvider reservationsDataProvider,
            IPersonStatementsProvider personStatementsProvider)
        {
            _statementsDataProvider = statementsDataProvider;
            _votingDataProvider = votingDataProvider;
            _seatsDataProvider = seatsDataProvider;
            _agendaSubItemsProvider = agendaSubItemsProvider;
            _reservationsDataProvider = reservationsDataProvider;
            _personStatementsProvider = personStatementsProvider;
        }

        public async Task ResetCache()
        {
            await _statementsDataProvider.ResetCache();
            await _votingDataProvider.ResetCache();
            await _seatsDataProvider.ResetCache();
            await _agendaSubItemsProvider.ResetCache();
            await _reservationsDataProvider.ResetCache();
            await _personStatementsProvider.ResetCache();
        }
    }
}
