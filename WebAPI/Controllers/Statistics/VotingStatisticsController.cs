using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebAPI.Controllers.Filters;
using WebAPI.Data.Statistics;

namespace WebAPI.Controllers.Statistics
{
    [ApiController]
    [Route("statistics/votings")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class VotingStatisticsController
    {
        private readonly ILogger<VotingStatisticsController> _logger;
        private readonly IVotingStatisticsDataProvider _statisticsDataProvider;

        public VotingStatisticsController(
            ILogger<VotingStatisticsController> logger,
            IVotingStatisticsDataProvider statisticsDataProvider)
        {
            _logger = logger;
            _statisticsDataProvider = statisticsDataProvider;
        }

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
