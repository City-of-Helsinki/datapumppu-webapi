namespace WebAPI.LiveMeetings
{
    public class StorageEventDTO
    {
        public string MeetingId { get; set; } = string.Empty;

        public string CaseNumber { get; set; } = string.Empty;

        public bool IsLiveEvent { get; set; } = true;
    }
}
