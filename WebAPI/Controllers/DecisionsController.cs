using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.DTOs;
using WebAPI.Controllers.Filters;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("decisions")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class DecisionsController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStorageApiClient _storageApiClient;
        private readonly ILogger<MeetingController> _logger;

        public DecisionsController(IConfiguration configuration, IStorageApiClient storageApiClient, ILogger<MeetingController> logger)
        {
            _configuration = configuration;
            _storageApiClient = storageApiClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("{caseLabelId}/{language}")]
        public async Task<IActionResult> GetDecision(string caseLabelId, string language)
        {
            _logger.LogInformation("Executing GetDecision()");
            var decision = await _storageApiClient.RequestDecision(caseLabelId, language.Substring(0, 2));
            if (decision== null)
            {
                return NoContent();
            }

            return new OkObjectResult(decision);
        }
    }
}
