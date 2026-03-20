using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.DTOs;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.Data.Statistics;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for retrieving statement records for meeting cases.
    /// </summary>
    [ApiController]
    [Route("statement")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class StatementController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStatementsDataProvider _statementsDataProvider;
        private readonly ILogger<StatementController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatementController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="statementsDataProvider">The statements data provider.</param>
        /// <param name="logger">The logger instance.</param>
        public StatementController(
            IConfiguration configuration,
            IStatementsDataProvider statementsDataProvider,
            ILogger<StatementController> logger)
        {
            _configuration = configuration;
            _statementsDataProvider = statementsDataProvider;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves statement records for a specific meeting case.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="caseNumber">The case number within the meeting.</param>
        /// <returns>The list of statements for the specified case.</returns>
        [HttpGet]
        [Route("{meetingId}/{caseNumber}")]
        public async Task<IActionResult> GetStatements(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing GetStatements() {0}, {1}", meetingId, caseNumber);
            var turns = await _statementsDataProvider.GetStatements(meetingId, caseNumber);
            
            return new OkObjectResult(turns);
        }
    }
}
