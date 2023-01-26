namespace WebAPI.Controllers.DTOs
{
    public class StorageMeetingDTO
    {
        public DateTime MeetingDate { get; set; }

        public string MeetingID { get; set; }

        public string Name { get; set; }

        public int MeetingSequenceNumber { get; set; }

        public string Location { get; set; }

        public string? MeetingTitleFI { get; set; }

        public string? MeetingTitleSV { get; set; }

        public DateTime? MeetingStarted { get; set; }

        public Guid? MeetingStartedEventID { get; set; }

        public DateTime? MeetingEnded { get; set; }

        public Guid? MeetingEndedEventID { get; set; }

        public List<StorageAgendaItemDTO>? Agendas { get; set; }

        public List<StorageDecisionDTO>? Decisions { get; set; }
    }
}
