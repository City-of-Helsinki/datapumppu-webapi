using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.DTOs;
using WebAPI.Controllers.Filters;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("statement")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class StatementController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStorageApiClient _storageApiClient;
        private readonly ILogger<StatementController> _logger;

        public StatementController(
            IConfiguration configuration,
            IStorageApiClient storageApiClient,
            ILogger<StatementController> logger)
        {
            _configuration = configuration;
            _storageApiClient = storageApiClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("{meetingId}/{caseNumber}")]
        public async Task<IActionResult> GetStatements(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing GetStatements()");
            var turns = await _storageApiClient.GetStatements(meetingId, caseNumber);
            
            return new OkObjectResult(turns);
        }
    }
}
