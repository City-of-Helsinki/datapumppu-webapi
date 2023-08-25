using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebAPI.Controllers.Filters;
using WebAPI.Data.Statistics;

namespace WebAPI.Controllers.Statistics
{
    [ApiController]
    [Route("statistics/personstatements")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class PersonStatementStatisticsController
    {
        private readonly ILogger<PersonStatementStatisticsController> _logger;
        private readonly IPersonStatementStatisticsDataProvider _statementStatisticsDataProvider;

        public PersonStatementStatisticsController(
            ILogger<PersonStatementStatisticsController> logger,
            IPersonStatementStatisticsDataProvider statementStatisticsDataProvider)
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
