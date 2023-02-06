using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.DTOs;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("seats")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class SeatsController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISeatsDataProvider _seatsProvider;
        private readonly ILogger<SeatsController> _logger;

        public SeatsController(
            IConfiguration configuration,
            ISeatsDataProvider seatsProvider,
            ILogger<SeatsController> logger)
        {
            _configuration = configuration;
            _seatsProvider = seatsProvider;
            _logger = logger;
        }

        [HttpGet]
        [Route("{meetingId}/{caseNumber}")]
        public async Task<IActionResult> GetSeats(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing GetSeats()");
            var seats = await _seatsProvider.GetSeats(meetingId, caseNumber);
            
            return new OkObjectResult(seats);
        }
    }
}
