using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for retrieving meeting data from the caching layer.
    /// </summary>
    [ApiController]
    [Route("meetings")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class MeetingController: ControllerBase
    {
        private readonly IMeetingDataProvider _meetingDataProvider;
        private readonly ILogger<MeetingController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingController"/> class.
        /// </summary>
        /// <param name="meetingDataProvider">The meeting data provider.</param>
        /// <param name="logger">The logger instance.</param>
        public MeetingController(
            IMeetingDataProvider meetingDataProvider,
            ILogger<MeetingController> logger)
        {
            _meetingDataProvider = meetingDataProvider;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves meeting details by year, sequence number, and language.
        /// </summary>
        /// <param name="year">The year of the meeting.</param>
        /// <param name="sequenceNumber">The sequence number of the meeting within the year.</param>
        /// <param name="lang">The language code (en, fi, or sv).</param>
        /// <returns>The meeting data or 204 No Content if not found.</returns>
        [HttpGet]
        [Route("meeting")]
        public async Task<IActionResult> GetMeeting(string year, string sequenceNumber, string lang)
        {
            _logger.LogInformation("Executing GetMeeting() {0} {1} {2}", year, sequenceNumber, lang);

            var meeting = await _meetingDataProvider.GetMeeting(year, sequenceNumber, lang);
            if (meeting == null)
            {
                return NoContent();
            }

            return Ok(meeting);
        }
    }
};
