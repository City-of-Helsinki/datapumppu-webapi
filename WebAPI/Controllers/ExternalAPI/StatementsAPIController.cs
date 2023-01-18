using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.StorageClient;

namespace WebAPI.Controllers.ExternalAPI
{
    [ApiController]
    [Route("api/statements")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class StatementsAPIController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStorageApiClient _storageApiClient;
        private readonly ILogger<StatementsAPIController> _logger;

        public StatementsAPIController(
            IConfiguration configuration,
            IStorageApiClient storageApiClient,
            ILogger<StatementsAPIController> logger)
        {
            _configuration = configuration;
            _storageApiClient = storageApiClient;
            _logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> GetStatementsByPerson(
            [FromQuery] string name,
            [FromQuery] int year,
            [FromQuery] string lang)
        {
            _logger.LogInformation($"GetStatementsByPerson {name} {year}");
            return new OkObjectResult(await _storageApiClient.GetStatementsByPerson(name, year, lang));
        }
    }
}
