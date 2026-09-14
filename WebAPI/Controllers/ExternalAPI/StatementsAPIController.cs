using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers.ExternalAPI
{
    /// <summary>
    /// External API controller for querying statements by person name, year, or date range.
    /// </summary>
    [ApiController]
    [Route("api/statements")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class StatementsAPIController : ControllerBase
    {
        private readonly IPersonStatementsProvider _personStatementsProvider;
        private readonly ILogger<StatementsAPIController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatementsAPIController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="personStatementsProvider">The person statements data provider.</param>
        /// <param name="logger">The logger instance.</param>
        public StatementsAPIController(
            IConfiguration configuration,
            IPersonStatementsProvider personStatementsProvider,
            ILogger<StatementsAPIController> logger)
        {
            _personStatementsProvider = personStatementsProvider;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves statements for a person by name and year.
        /// </summary>
        /// <param name="name">The person's name.</param>
        /// <param name="year">The year to query.</param>
        /// <param name="lang">The language code (en, fi, or sv).</param>
        /// <returns>A list of statements for the specified person and year.</returns>
        [HttpGet()]
        public async Task<IActionResult> GetStatementsByPerson(
            [FromQuery] string name,
            [FromQuery] int year,
            [FromQuery] string lang)
        {
            _logger.LogInformation($"GetStatementsByPerson {name} {year} {lang}");
            return new OkObjectResult(await _personStatementsProvider.GetStatements(name, year, lang));
        }

        /// <summary>
        /// Retrieves statements by person name(s) and/or date range.
        /// At least one of names or date range (startDate and endDate) is required.
        /// </summary>
        /// <param name="names">Comma-separated person names (optional if date range provided).</param>
        /// <param name="startDate">Start date in YYYY-MM-DD format (optional if names provided).</param>
        /// <param name="endDate">End date in YYYY-MM-DD format (optional if names provided).</param>
        /// <param name="lang">The language code (en, fi, or sv).</param>
        /// <returns>A list of matching statements, or 400 Bad Request if parameters are missing.</returns>
        [HttpGet("lookup")]
        public async Task<IActionResult> GetStatementsByPersonOrDate(
            [FromQuery] string? names,
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromQuery] string lang)
        {
            bool hasNames = !string.IsNullOrWhiteSpace(names);
            bool hasDateRange = !string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate);

            if (!hasNames && !hasDateRange)
            {
                return BadRequest("Kyselyssä tulee vähintään olla joko 'names' tai aikaväli 'startDate' ja 'endDate'");
            }

            _logger.LogInformation($"GetStatementsByPersonOrDate {names} {startDate} {endDate} {lang}");
            return new OkObjectResult(await _personStatementsProvider.GetStatementsLookup(names, startDate, endDate, lang));
        }
    }
}
