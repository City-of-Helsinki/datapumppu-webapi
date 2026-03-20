using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebAPI.Controllers.Filters;
using WebAPI.Data.Statistics;

namespace WebAPI.Controllers.Statistics
{
    /// <summary>
    /// Controller for downloading voting statistics as CSV.
    /// </summary>
    [ApiController]
    [Route("statistics/votings")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class VotingStatisticsController
    {
        private readonly ILogger<VotingStatisticsController> _logger;
        private readonly IVotingStatisticsDataProvider _statisticsDataProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="VotingStatisticsController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="statisticsDataProvider">The voting statistics data provider.</param>
        public VotingStatisticsController(
            ILogger<VotingStatisticsController> logger,
            IVotingStatisticsDataProvider statisticsDataProvider)
        {
            _logger = logger;
            _statisticsDataProvider = statisticsDataProvider;
        }

        /// <summary>
        /// Downloads voting statistics for a given year as a CSV file.
        /// </summary>
        /// <param name="year">The year to retrieve statistics for.</param>
        /// <returns>A CSV file download containing voting statistics.</returns>
        [HttpGet]
        [Route("{year}")]
        public async Task<IActionResult> GetStatistics(int year)
        {
            _logger.LogInformation("Executing GetStatistics() {0}", year);

            var items = await _statisticsDataProvider.GetVotings(year);

            var csvLines = items?.Select(item => $"\"{item.Person}\",{item.AdditionalInfoFi},{item.For},{item.Against},{item.Empty},{item.Absent},{item.Sum}").ToList() ?? new List<string>();

            var headers = "person,additional_info,for,against,empty,absent,sum";
            csvLines.Insert(0, headers);

            var fileContent = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, csvLines));

            return new FileContentResult(fileContent, "text/csv")
            {
                FileDownloadName = $"votes-{year}.csv"
            };
        }

    }
}
