namespace WebAPI.Data
{
    /// <summary>
    /// Coordinates cache reset operations across all data providers.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Resets all caches. When <paramref name="liveEvent"/> is true, meeting data cache is preserved
        /// since meeting metadata does not change during live events.
        /// </summary>
        /// <param name="liveEvent">Whether the reset is triggered by a live meeting event.</param>
        Task ResetCache(bool liveEvent);
    }

    /// <summary>
    /// Unified cache manager that resets all registered data provider caches.
    /// </summary>
    public class Cache : ICache
    {
        private readonly IStatementsDataProvider _statementsDataProvider;
        private readonly IVotingDataProvider _votingDataProvider;
        private readonly ISeatsDataProvider _seatsDataProvider;
        private readonly IAgendaSubItemsProvider _agendaSubItemsProvider;
        private readonly IReservationsDataProvider _reservationsDataProvider;
        private readonly IPersonStatementsProvider _personStatementsProvider;
        private readonly IMeetingDataProvider _meetingDataProvider;

        public Cache(IStatementsDataProvider statementsDataProvider,
            IVotingDataProvider votingDataProvider,
            ISeatsDataProvider seatsDataProvider,
            IAgendaSubItemsProvider agendaSubItemsProvider,
            IReservationsDataProvider reservationsDataProvider,
            IPersonStatementsProvider personStatementsProvider,
            IMeetingDataProvider meetingDataProvider)
        {
            _statementsDataProvider = statementsDataProvider;
            _votingDataProvider = votingDataProvider;
            _seatsDataProvider = seatsDataProvider;
            _agendaSubItemsProvider = agendaSubItemsProvider;
            _reservationsDataProvider = reservationsDataProvider;
            _personStatementsProvider = personStatementsProvider;
            _meetingDataProvider = meetingDataProvider;
        }

        public async Task ResetCache(bool liveEvent)
        {
            await _statementsDataProvider.ResetCache();
            await _votingDataProvider.ResetCache();
            await _seatsDataProvider.ResetCache();
            await _agendaSubItemsProvider.ResetCache();
            await _reservationsDataProvider.ResetCache();
            await _personStatementsProvider.ResetCache();

            // meeting data provider do not contain live data, so this should be resetted only if 
            // event is not live event
            if (!liveEvent)
            {
                await _meetingDataProvider.ResetCache();
            }
        }
    }
}
