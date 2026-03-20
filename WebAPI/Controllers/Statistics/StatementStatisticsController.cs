using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebAPI.Controllers.Filters;
using WebAPI.Data.Statistics;

namespace WebAPI.Controllers.Statistics
{
    /// <summary>
    /// Controller for downloading per-issue statement statistics as CSV.
    /// </summary>
    [ApiController]
    [Route("statistics/statements")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class StatementStatisticsController
    {
        private readonly ILogger<StatementStatisticsController> _logger;
        private readonly IStatementStatisticsDataProvider _statementStatisticsDataProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatementStatisticsController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="statementStatisticsDataProvider">The statement statistics data provider.</param>
        public StatementStatisticsController(
            ILogger<StatementStatisticsController> logger,
            IStatementStatisticsDataProvider statementStatisticsDataProvider)
        {
            _logger = logger;
            _statementStatisticsDataProvider = statementStatisticsDataProvider;
        }

        /// <summary>
        /// Downloads per-issue statement statistics for a given year as a CSV file.
        /// </summary>
        /// <param name="year">The year to retrieve statistics for.</param>
        /// <returns>A CSV file download containing statement statistics.</returns>
        [HttpGet]
        [Route("{year}")]
        public async Task<IActionResult> GetStatementStatistics(int year)
        {
            _logger.LogInformation("Executing GetStatementStatistics() {0}", year);

            var items = await _statementStatisticsDataProvider.GetStatements(year);

            var csvLines = items?.Select(item => $"\"{item.MeetingId}\",{item.CaseNumber},\"{item.Title}\",{item.Count},{item.TotalDuration},{item.IsMotion}").ToList() ?? new List<string>();

            var headers = "meeting_id,case_number,title,statement_count,statement_total_duration_s,is_motion";
            csvLines.Insert(0, headers);

            var fileContent = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, csvLines));

            return new FileContentResult(fileContent, "text/csv")
            {
                FileDownloadName = $"issue_statements-{year}.csv"
            };
        }

    }
}
