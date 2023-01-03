
using System.Text;
using System.Text.Json;
using WebAPI.Controllers.DTOs;

namespace WebAPI.StorageClient
{
    public interface IStorageApiClient
    {
        Task<StorageMeetingDTO?> RequestMeeting(string year, string sequenceNumber, string language);

        Task<StorageDecisionDTO?> RequestDecision(string caseIdLabel, string language);

        Task<List<SeatDTO>> RequestSeats(string meetingId, string caseNumber);
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

        public async Task<StorageDecisionDTO?> RequestDecision(string caseIdLabel, string language)
        {
            _logger.LogInformation("Executing RequestDecision()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/decisions/{caseIdLabel}/{language}");
            var decision = await response.Content.ReadFromJsonAsync<StorageDecisionDTO>();

            return decision;
        }

        public async Task<List<SeatDTO>> RequestSeats(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing RequestSeats()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/seats/{meetingId}/{caseNumber}");
            var seats = await response.Content.ReadFromJsonAsync<SeatDTO[]>();

            return seats.ToList();
        }
    }
}
