using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for retrieving voting records for meeting cases.
    /// </summary>
    [ApiController]
    [Route("voting")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class VotesController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IVotingDataProvider _dataProvider;
        private readonly ILogger<VotesController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VotesController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="dataProvider">The voting data provider.</param>
        /// <param name="logger">The logger instance.</param>
        public VotesController(
            IConfiguration configuration,
            IVotingDataProvider dataProvider,
            ILogger<VotesController> logger)
        {
            _configuration = configuration;
            _dataProvider = dataProvider;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves voting records for a specific meeting case.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="caseNumber">The case number within the meeting.</param>
        /// <returns>A list of voting records, or an empty list if none found.</returns>
        [HttpGet]
        [Route("{meetingId}/{caseNumber}")]
        public async Task<List<StorageVotingDTO>> GetVoting(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing GetVoting() {0}, {1}", meetingId, caseNumber);
            var voting = await _dataProvider.GetVoting(meetingId, caseNumber);
            
            return voting ?? new List<StorageVotingDTO>();
        }
    }
}
