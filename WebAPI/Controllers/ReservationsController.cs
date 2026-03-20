using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for retrieving reservation data for meeting cases.
    /// </summary>
    [ApiController]
    [Route("reservations")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class ReservationsController: ControllerBase
    {
        private readonly IReservationsDataProvider _reservationsDataProvider;
        private readonly ILogger<ReservationsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationsController"/> class.
        /// </summary>
        /// <param name="reservationsDataProvider">The reservations data provider.</param>
        /// <param name="logger">The logger instance.</param>
        public ReservationsController(
            IReservationsDataProvider reservationsDataProvider,
            ILogger<ReservationsController> logger)
        {
            _reservationsDataProvider = reservationsDataProvider;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves reservations for a specific meeting case.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="caseNumber">The case number within the meeting.</param>
        /// <returns>The reservation data for the specified case.</returns>
        [HttpGet]
        [Route("{meetingId}/{caseNumber}")]
        public async Task<IActionResult> GetReservations(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing GetReservations() {0} {1}", meetingId, caseNumber);
            var reservations = await _reservationsDataProvider.GetReservations(meetingId, caseNumber);
            
            return new OkObjectResult(reservations);
        }
    }
}
