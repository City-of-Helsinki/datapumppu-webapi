using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.DTOs;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.Data.Statistics;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("statement")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class StatementController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStatementsDataProvider _statementsDataProvider;
        private readonly ILogger<StatementController> _logger;

        public StatementController(
            IConfiguration configuration,
            IStatementsDataProvider statementsDataProvider,
            ILogger<StatementController> logger)
        {
            _configuration = configuration;
            _statementsDataProvider = statementsDataProvider;
            _logger = logger;
        }

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
