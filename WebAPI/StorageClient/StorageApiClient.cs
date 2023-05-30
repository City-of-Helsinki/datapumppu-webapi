
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.StorageClient
{
    public interface IStorageApiClient
    {
        Task<StorageMeetingDTO?> RequestMeeting(string year, string sequenceNumber, string language);

        Task<List<StorageAgendaSubItemDTO>> RequestAgendaPointSubItemsg(string meetingId, int agendaPoint);

        Task<List<SeatDTO>> RequestSeats(string meetingId, string caseNumber);

        Task<List<StorageVotingDTO>?> RequestVote(string meetingId, string caseNumber);

        Task<List<StatementDTO>> GetStatements(string meetingId, string caseNumber);

        Task<List<StatementDTO>> GetStatementsByPerson(string name, int year, string lang);

        Task<List<ReservationDTO>> GetReservations(string meetingId, string caseNumber);

        Task<bool> CheckLogin(string username, string password);

        Task<bool> UpdateAgendaPoint(EditAgendaPointDTO dto);

        Task<List<StorageStatementStatisticsDTO>?> RequestStatementStatistics(int year);

        Task<List<StoragePersonStatementStatisticsDTO>?> RequestPersonStatementStatistics(int year);

        Task<List<StorageVotingStatisticsDTO>?> RequestVotingStatistics(int year);

        Task<bool> UpdateVideoSync(VideoSyncDTO videoSyncDTO);
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

        public async Task<bool> CheckLogin(string username, string password)
        {
            _logger.LogInformation("Executing CheckLogin()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/auth/validate?username={username}&password={password}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAgendaPoint(EditAgendaPointDTO dto)
        {
            _logger.LogInformation("Executing UpdateAgendaPoint()");

            using var connection = _storageConnection.CreateConnection();
            var response = await connection.PostAsJsonAsync($"api/meetinginfo/agendapoint", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<StatementDTO>> GetStatementsByPerson(string name, int year, string lang)
        {
            _logger.LogInformation("GetStatementsByPerson()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/statements/person?name={name}&year={year}&lang={lang}");
            var statements = await response.Content.ReadFromJsonAsync<StatementDTO[]>();

            return statements?.ToList() ?? new List<StatementDTO>();
        }

        public async Task<List<StatementDTO>> GetStatements(string meetingId, string caseNumber)
        {
            _logger.LogInformation("GetStatements()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/statements/{meetingId}/{caseNumber}");
            var statements = await response.Content.ReadFromJsonAsync<StatementDTO[]>();

            return statements?.ToList() ?? new List<StatementDTO>();
        }

        public async Task<List<ReservationDTO>> GetReservations(string meetingId, string caseNumber)
        {
            _logger.LogInformation("GetReservations()");
            var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/reservations/{meetingId}/{caseNumber}");
            var reservations = await response.Content.ReadFromJsonAsync<ReservationDTO[]>();

            return reservations?.ToList() ?? new List<ReservationDTO>();
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

        public async Task<List<StorageAgendaSubItemDTO>> RequestAgendaPointSubItemsg(string meetingId, int agendaPoint)
        {
            _logger.LogInformation("Executing RequestAgendaPointSubItemsg()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/meetinginfo/meeting/{meetingId}/{agendaPoint}");
            if ((int)response.StatusCode == StatusCodes.Status204NoContent)
            {
                return new List<StorageAgendaSubItemDTO>();
            }

            return await response.Content.ReadFromJsonAsync<List<StorageAgendaSubItemDTO>>() ?? new List<StorageAgendaSubItemDTO>();
        }

        public async Task<List<SeatDTO>> RequestSeats(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing RequestSeats()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/seats/{meetingId}/{caseNumber}");
            var seats = await response.Content.ReadFromJsonAsync<SeatDTO[]>();

            return seats?.ToList() ?? new List<SeatDTO>();
        }

        public async Task<List<StorageVotingDTO>?> RequestVote(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing RequestVote()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/voting/{meetingId}/{caseNumber}");
            if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new List<StorageVotingDTO>();
            }

            return await response.Content.ReadFromJsonAsync<List<StorageVotingDTO>>();
        }

        public async Task<List<StorageStatementStatisticsDTO>?> RequestStatementStatistics(int year)
        {
            _logger.LogInformation("Executing RequestStatementStatistics()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/statistics/statements/{year}");
            if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new List<StorageStatementStatisticsDTO>();
            }

            return await response.Content.ReadFromJsonAsync<List<StorageStatementStatisticsDTO>>();
        }

        public async Task<List<StoragePersonStatementStatisticsDTO>?> RequestPersonStatementStatistics(int year)
        {
            _logger.LogInformation("Executing RequestPersonStatementStatistics()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/statistics/personstatements/{year}");
            if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new List<StoragePersonStatementStatisticsDTO>();
            }

            return await response.Content.ReadFromJsonAsync<List<StoragePersonStatementStatisticsDTO>>();
        }

        public async Task<List<StorageVotingStatisticsDTO>?> RequestVotingStatistics(int year)
        {
            _logger.LogInformation("Executing RequestVotingStatistics()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/statistics/votings/{year}");
            if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new List<StorageVotingStatisticsDTO>();
            }

            return await response.Content.ReadFromJsonAsync<List<StorageVotingStatisticsDTO>>();
        }

        public async Task<bool> UpdateVideoSync(VideoSyncDTO videoSyncDTO)
        {
            _logger.LogInformation("Executing UpdateVideoSync()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.PostAsJsonAsync($"api/videosync/position", videoSyncDTO);
            return response.IsSuccessStatusCode;
        }
    }
}
