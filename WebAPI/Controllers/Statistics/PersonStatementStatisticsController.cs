using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebAPI.Controllers.Filters;
using WebAPI.Data.Statistics;

namespace WebAPI.Controllers.Statistics
{
    /// <summary>
    /// Controller for downloading per-person statement statistics as CSV.
    /// </summary>
    [ApiController]
    [Route("statistics/personstatements")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class PersonStatementStatisticsController
    {
        private readonly ILogger<PersonStatementStatisticsController> _logger;
        private readonly IPersonStatementStatisticsDataProvider _statementStatisticsDataProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonStatementStatisticsController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="statementStatisticsDataProvider">The person statement statistics data provider.</param>
        public PersonStatementStatisticsController(
            ILogger<PersonStatementStatisticsController> logger,
            IPersonStatementStatisticsDataProvider statementStatisticsDataProvider)
        {
            _logger = logger;
            _statementStatisticsDataProvider = statementStatisticsDataProvider;
        }

        /// <summary>
        /// Downloads per-person statement statistics for a given year as a CSV file.
        /// </summary>
        /// <param name="year">The year to retrieve statistics for.</param>
        /// <returns>A CSV file download containing person statement statistics.</returns>
        [HttpGet]
        [Route("{year}")]
        public async Task<IActionResult> GetStatementStatistics(int year)
        {
            _logger.LogInformation("Executing GetStatementStatistics() {0}", year);

            var items = await _statementStatisticsDataProvider.GetStatements(year);

            var csvLines = items?.Select(item => $"\"{item.Person}\",{item.MeetingId},\"{item.Title}\",{item.Started},{item.Ended},{item.DurationSeconds}").ToList() ?? new List<string>();

            var headers = "person,meeting_id,title,started,ended,duration_seconds";
            csvLines.Insert(0, headers);

            var fileContent = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, csvLines));

            return new FileContentResult(fileContent, "text/csv")
            {
                FileDownloadName = $"person_statements-{year}.csv"
            };
        }

    }
}
