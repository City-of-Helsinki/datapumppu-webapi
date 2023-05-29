using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("voting")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class VotesController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IVotingDataProvider _dataProvider;
        private readonly ILogger<VotesController> _logger;

        public VotesController(
            IConfiguration configuration,
            IVotingDataProvider dataProvider,
            ILogger<VotesController> logger)
        {
            _configuration = configuration;
            _dataProvider = dataProvider;
            _logger = logger;
        }

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
