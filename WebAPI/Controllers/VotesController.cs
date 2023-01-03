using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.DTOs;
using WebAPI.Controllers.Filters;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("voting")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class VotesController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStorageApiClient _storageApiClient;
        private readonly ILogger<VotesController> _logger;

        public VotesController(
            IConfiguration configuration,
            IStorageApiClient storageApiClient,
            ILogger<VotesController> logger)
        {
            _configuration = configuration;
            _storageApiClient = storageApiClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("{meetingId}/{caseNumber}")]
        public async Task<IActionResult> GetVoting(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing GetVoting()");
            var voting = await _storageApiClient.RequestVote(meetingId, caseNumber);
            
            return new OkObjectResult(voting);
        }
    }
}
