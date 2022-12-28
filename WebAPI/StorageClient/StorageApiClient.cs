
using System.Text;
using System.Text.Json;
using WebAPI.Controllers.DTOs;

namespace WebAPI.StorageClient
{
    public interface IStorageApiClient
    {
        Task<StorageMeetingDTO?> RequestMeeting(string year, string sequenceNumber, string language);

        Task<StorageDecisionDTO> RequestDecision(string caseIdLabel, string language);
    }

    public class StorageApiClient : IStorageApiClient
    {
        private readonly IStorageConnection _storageConnection;
        private readonly ILogger<StorageApiClient> _logger;

        public StorageApiClient(ILogger<StorageApiClient> logger,
            IStorageConnection storageConnection)
        {
            _logger = logger;
            _storageConnection = storageConnection;
        }

        public async Task<StorageMeetingDTO?> RequestMeeting(string year, string sequenceNumber, string language)
        {
            _logger.LogInformation("Executing RequestMeeting()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/meetinginfo/meeting/{year}/{sequenceNumber}/{language}");
            if ((int)response.StatusCode == StatusCodes.Status204NoContent)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<StorageMeetingDTO>();
        }

        public async Task<StorageDecisionDTO> RequestDecision(string caseIdLabel, string language)
        {
            _logger.LogInformation("Executing RequestMeeting()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/decisions/{caseIdLabel}/{language}");
            var decision = await response.Content.ReadFromJsonAsync<StorageDecisionDTO>();

            return decision;
        }

    }
}
