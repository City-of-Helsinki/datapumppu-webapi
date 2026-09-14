namespace WebAPI.LiveMeetings
{
    /// <summary>
    /// Data transfer object representing a storage event received from Kafka.
    /// </summary>
    public class StorageEventDTO
    {
        public string MeetingId { get; set; } = string.Empty;

        public string CaseNumber { get; set; } = string.Empty;

        public bool IsLiveEvent { get; set; } = true;
    }
}
