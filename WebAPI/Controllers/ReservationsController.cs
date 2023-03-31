using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("reservations")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class ReservationsController: ControllerBase
    {
        private readonly IReservationsDataProvider _reservationsDataProvider;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(
            IReservationsDataProvider reservationsDataProvider,
            ILogger<ReservationsController> logger)
        {
            _reservationsDataProvider = reservationsDataProvider;
            _logger = logger;
        }

        [HttpGet]
        [Route("{meetingId}/{caseNumber}")]
        public async Task<IActionResult> GetReservations(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing GetReservations()");
            var reservations = await _reservationsDataProvider.GetReservations(meetingId, caseNumber);
            
            return new OkObjectResult(reservations);
        }
    }
}
