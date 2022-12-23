using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("meetings")]
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
        [Route("next")]
        public async Task<StorageMeetingDTO> GetMeeting()
        {
            _logger.LogInformation("Executing GetMeeting()");
            var meeting = await _storageApiClient.RequestMeeting("2022", "20");

            return meeting;
        }
    }
}
