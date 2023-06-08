namespace WebAPI.StorageClient.DTOs
{
    public class ParticipationsPersonDTO
    {
        public string Person { get; set; } = string.Empty;

        public List<ParticipationsMeetingDTO> Meetings { get; set; } = new List<ParticipationsMeetingDTO>();
    }
}
