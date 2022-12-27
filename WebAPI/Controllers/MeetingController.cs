using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("meetings")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class MeetingController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStorageApiClient _storageApiClient;
        private readonly ILogger<MeetingController> _logger;

        public MeetingController(IConfiguration configuration, IStorageApiClient storageApiClient, ILogger<MeetingController> logger)
        {
            _configuration = configuration;
            _storageApiClient = storageApiClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("meeting")]
        public async Task<IActionResult> GetMeeting(string year, string sequenceNumber, string lang)
        {
            _logger.LogInformation("Executing GetMeeting()");
            var meeting = await _storageApiClient.RequestMeeting(year, sequenceNumber, lang);
            if (meeting == null)
            {
                return NoContent();
            }

            return Ok(meeting);
        }
    }
};
