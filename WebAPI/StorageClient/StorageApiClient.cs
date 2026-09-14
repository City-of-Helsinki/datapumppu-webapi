
using WebAPI.Controllers.DTOs;
using WebAPI.StorageClient.DTOs;

namespace WebAPI.StorageClient
{
    /// <summary>
    /// Client interface for all operations against the external storage API.
    /// </summary>
    public interface IStorageApiClient
    {
        /// <summary>
        /// Requests meeting data from the storage API.
        /// </summary>
        Task<StorageMeetingDTO?> RequestMeeting(string year, string sequenceNumber, string language);

        /// <summary>
        /// Requests agenda point sub-items from the storage API.
        /// </summary>
        Task<List<StorageAgendaSubItemDTO>> RequestAgendaPointSubItemsg(string meetingId, int agendaPoint);

        /// <summary>
        /// Requests seating information from the storage API.
        /// </summary>
        Task<List<WebApiSeatsDTO>> RequestSeats(string meetingId, string caseNumber);

        /// <summary>
        /// Requests voting records from the storage API.
        /// </summary>
        Task<List<StorageVotingDTO>?> RequestVote(string meetingId, string caseNumber);

        /// <summary>
        /// Retrieves statements for a specific meeting case from the storage API.
        /// </summary>
        Task<List<StatementDTO>> GetStatements(string meetingId, string caseNumber);

        /// <summary>
        /// Retrieves statements by person name and year from the storage API.
        /// </summary>
        Task<List<StatementDTO>> GetStatementsByPerson(string name, int year, string lang);

        /// <summary>
        /// Retrieves statements by person name(s) and/or date range from the storage API.
        /// </summary>
        Task<List<StatementDTO>> GetStatementsByPersonOrDate(string? name, string? startDate, string? endDate, string lang);

        /// <summary>
        /// Retrieves reservations for a meeting case from the storage API.
        /// </summary>
        Task<List<ReservationDTO>> GetReservations(string meetingId, string caseNumber);

        /// <summary>
        /// Validates editor credentials against the storage API.
        /// </summary>
        Task<bool> CheckLogin(string username, string password);

        /// <summary>
        /// Updates an agenda point via the storage API.
        /// </summary>
        Task<bool> UpdateAgendaPoint(EditAgendaPointDTO dto);

        /// <summary>
        /// Requests statement statistics for a year from the storage API.
        /// </summary>
        Task<List<StorageStatementStatisticsDTO>?> RequestStatementStatistics(int year);

        /// <summary>
        /// Requests per-person statement statistics for a year from the storage API.
        /// </summary>
        Task<List<StoragePersonStatementStatisticsDTO>?> RequestPersonStatementStatistics(int year);

        /// <summary>
        /// Requests voting statistics for a year from the storage API.
        /// </summary>
        Task<List<StorageVotingStatisticsDTO>?> RequestVotingStatistics(int year);

        /// <summary>
        /// Requests participant attendance statistics for a year from the storage API.
        /// </summary>
        Task<List<ParticipationsPersonDTO>?> RequestParticipantStatistics(int year);

        /// <summary>
        /// Updates the video synchronization position via the storage API.
        /// </summary>
        Task<bool> UpdateVideoSync(VideoSyncDTO videoSyncDTO);
    }

    /// <summary>
    /// HTTP client implementation for communicating with the external storage service.
    /// All data in the WebAPI is sourced through this client.
    /// </summary>
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
            var response = await connection.PostAsJsonAsync("api/auth/login", new { username, password });
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

        public async Task<List<StatementDTO>> GetStatementsByPersonOrDate(string? names, string? startDate, string? endDate, string lang)
        {
            _logger.LogInformation("GetStatementsByPersonOrDate()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/statements/lookup?names={names}&startDate={startDate}&endDate={endDate}&lang={lang}");
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

        public async Task<List<WebApiSeatsDTO>> RequestSeats(string meetingId, string caseNumber)
        {
            _logger.LogInformation("Executing RequestSeats() for {0}, {1}", meetingId, caseNumber);
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/seats/{meetingId}/{caseNumber}");
            var seats = await response.Content.ReadFromJsonAsync<WebApiSeatsDTO[]>();

            return seats?.ToList() ?? new List<WebApiSeatsDTO>();
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

        public async Task<List<ParticipationsPersonDTO>?> RequestParticipantStatistics(int year)
        {
            _logger.LogInformation("Executing RequestParticipantStatistics()");
            using var connection = _storageConnection.CreateConnection();
            var response = await connection.GetAsync($"api/statistics/participants/{year}");
            if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new List<ParticipationsPersonDTO>();
            }

            return await response.Content.ReadFromJsonAsync<List<ParticipationsPersonDTO>>();
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
