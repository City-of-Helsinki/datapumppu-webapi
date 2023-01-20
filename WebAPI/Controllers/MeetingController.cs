using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("meetings")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class MeetingController: ControllerBase
    {
        private readonly IMeetingDataProvider _meetingDataProvider;
        private readonly ILogger<MeetingController> _logger;

        public MeetingController(
            IMeetingDataProvider meetingDataProvider,
            ILogger<MeetingController> logger)
        {
            _meetingDataProvider = meetingDataProvider;
            _logger = logger;
        }

        [HttpGet]
        [Route("meeting")]
        public async Task<IActionResult> GetMeeting(string year, string sequenceNumber, string lang)
        {
            _logger.LogInformation("Executing GetMeeting()");

            var meeting = await _meetingDataProvider.GetMeeting(year, sequenceNumber, lang);
            if (meeting == null)
            {
                return NoContent();
            }

            return Ok(meeting);
        }
    }
};
