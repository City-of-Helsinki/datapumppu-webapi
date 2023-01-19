namespace WebAPI.Controllers.DTOs
{
    public class StorageAgendaItemDTO
    {
        public int AgendaPoint { get; set; }

        public string? Section { get; set; }

        public string? Title { get; set; }

        public string? CaseIDLabel { get; set; }

        public string? Html { get; set; }

        public string? DecisionHistoryHTML { get; set; }

        public StorageAttachmentDTO[] Attachments { get; set; } = new StorageAttachmentDTO[0];

        public DateTime? Timestamp { get; set; }

        public int? VideoPosition { get; set; }
    }
}
