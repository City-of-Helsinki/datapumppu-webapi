using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebAPI.Controllers.Filters;
using WebAPI.Data.Statistics;

namespace WebAPI.Controllers.Statistics
{
    [ApiController]
    [Route("statistics/statements")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class StatementStatisticsController
    {
        private readonly ILogger<StatementStatisticsController> _logger;
        private readonly IStatementStatisticsDataProvider _statementStatisticsDataProvider;

        public StatementStatisticsController(
            ILogger<StatementStatisticsController> logger,
            IStatementStatisticsDataProvider statementStatisticsDataProvider)
        {
            _logger = logger;
            _statementStatisticsDataProvider = statementStatisticsDataProvider;
        }

        [HttpGet]
        [Route("{year}")]
        public async Task<IActionResult> GetStatementStatistics(int year)
        {
            _logger.LogInformation("Executing GetStatementStatistics() {0}", year);

            var items = await _statementStatisticsDataProvider.GetStatements(year);
            var csvLines = items?.Select(item => $"{item.MeetingId},{item.CaseNumber},\"{item.Title}\",{item.Count},{item.TotalDuration},{item.IsMotion}");

            var fileContent = new byte[0];            
            if (csvLines != null)
            {
                fileContent = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, csvLines));
            }

            return new FileContentResult(fileContent, "text/csv")
            {
                FileDownloadName = $"issue_statements-{year}.csv"
            };
        }

    }
}
