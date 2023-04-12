using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers.ExternalAPI
{
    [ApiController]
    [Route("api/statements")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class StatementsAPIController : ControllerBase
    {
        private readonly IPersonStatementsProvider _personStatementsProvider;
        private readonly ILogger<StatementsAPIController> _logger;

        public StatementsAPIController(
            IConfiguration configuration,
            IPersonStatementsProvider personStatementsProvider,
            ILogger<StatementsAPIController> logger)
        {
            _personStatementsProvider = personStatementsProvider;
            _logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> GetStatementsByPerson(
            [FromQuery] string name,
            [FromQuery] int year,
            [FromQuery] string lang)
        {
            _logger.LogInformation($"GetStatementsByPerson {name} {year} {lang}");
            return new OkObjectResult(await _personStatementsProvider.GetStatements(name, year, lang));
        }
    }
}
