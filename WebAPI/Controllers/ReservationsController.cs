using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("reservations")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class ReservationsController: ControllerBase
    {
        private readonly IStorageApiClient _storageApiClient;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(
            IStorageApiClient storageApiClient,
            ILogger<ReservationsController> logger)
        {
            _storageApiClient = storageApiClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("{meetingId}/{caseNumber}")]
        public async Task<IActionResult> GetReservations(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing GetStatements()");
            var reservations = await _storageApiClient.GetReservations(meetingId, caseNumber);
            
            return new OkObjectResult(reservations);
        }
    }
}
