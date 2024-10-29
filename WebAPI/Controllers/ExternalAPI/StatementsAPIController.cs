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

        [HttpGet("lookup")]
        public async Task<IActionResult> GetStatementsByPersonOrDate(
            [FromQuery] string? names,
            [FromQuery] string? startDate,
            [FromQuery] string? endDate)
        {
            bool hasNames = !string.IsNullOrWhiteSpace(names);
            bool hasDateRange = !string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate);

            if (!hasNames && !hasDateRange)
            {
                return BadRequest("Kyselyssä tulee vähintään olla joko 'names' tai aikaväli 'startDate' ja 'endDate'");
            }

            _logger.LogInformation($"GetStatementsByPersonOrDate {names} {startDate} {endDate}");
            return new OkObjectResult(await _personStatementsProvider.GetStatementsLookup(names, startDate, endDate));
        }
    }
}
