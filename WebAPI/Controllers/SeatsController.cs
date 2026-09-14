using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.DTOs;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for retrieving seating information for meeting cases.
    /// </summary>
    [ApiController]
    [Route("seats")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class SeatsController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISeatsDataProvider _seatsProvider;
        private readonly ILogger<SeatsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeatsController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="seatsProvider">The seats data provider.</param>
        /// <param name="logger">The logger instance.</param>
        public SeatsController(
            IConfiguration configuration,
            ISeatsDataProvider seatsProvider,
            ILogger<SeatsController> logger)
        {
            _configuration = configuration;
            _seatsProvider = seatsProvider;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves seating information for a specific meeting case.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="caseNumber">The case number within the meeting.</param>
        /// <returns>The list of seating arrangement layout DTOs for the specified case.</returns>
        [HttpGet]
        [Route("{meetingId}/{caseNumber}")]
        public async Task<IActionResult> GetSeats(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing GetSeats() {0}, {1}", meetingId, caseNumber);
            var seats = await _seatsProvider.GetSeats(meetingId, caseNumber);
            
            return new OkObjectResult(seats);
        }
    }
}
