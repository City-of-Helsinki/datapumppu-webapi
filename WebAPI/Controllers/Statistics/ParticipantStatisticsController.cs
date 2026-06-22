using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebAPI.Controllers.Filters;
using WebAPI.Data.Statistics;

namespace WebAPI.Controllers.Statistics
{
    /// <summary>
    /// Controller for downloading participant attendance statistics as JSON.
    /// </summary>
    [ApiController]
    [Route("statistics/participants")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class ParticipantsStatisticsController
    {
        private readonly ILogger<ParticipantsStatisticsController> _logger;
        private readonly IParticipantStatisticsDataProvider _statisticsDataProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticipantsStatisticsController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="statisticsDataProvider">The participant statistics data provider.</param>
        public ParticipantsStatisticsController(
            ILogger<ParticipantsStatisticsController> logger,
            IParticipantStatisticsDataProvider statisticsDataProvider)
        {
            _logger = logger;
            _statisticsDataProvider = statisticsDataProvider;
        }

        /// <summary>
        /// Downloads participant statistics for a given year as a JSON file.
        /// </summary>
        /// <param name="year">The year to retrieve statistics for.</param>
        /// <returns>A JSON file download containing participant statistics.</returns>
        [HttpGet]
        [Route("{year}")]
        public async Task<IActionResult> GetStatistics(int year)
        {
            _logger.LogInformation("Executing GetStatistics() {0}", year);

            var items = await _statisticsDataProvider.GetStatistics(year);

            var fileContent = JsonConvert.SerializeObject(items) ?? string.Empty;

            return new FileContentResult(Encoding.UTF8.GetBytes(fileContent), "text/json")
            {
                FileDownloadName = $"participants-{year}.json"
            };
        }

    }
}
