using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebAPI.Controllers.Filters;
using WebAPI.Data.Statistics;

namespace WebAPI.Controllers.Statistics
{
    [ApiController]
    [Route("statistics/participants")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class ParticipantsStatisticsController
    {
        private readonly ILogger<ParticipantsStatisticsController> _logger;
        private readonly IParticipantStatisticsDataProvider _statisticsDataProvider;

        public ParticipantsStatisticsController(
            ILogger<ParticipantsStatisticsController> logger,
            IParticipantStatisticsDataProvider statisticsDataProvider)
        {
            _logger = logger;
            _statisticsDataProvider = statisticsDataProvider;
        }

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
