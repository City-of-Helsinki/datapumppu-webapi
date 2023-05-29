﻿namespace WebAPI.StorageClient.DTOs
{
    public class StorageDecisionDTO
    {
        public string MeetingID { get; set; }

        public string NativeId { get; set; }

        public string? Title { get; set; }

        public string? CaseIDLabel { get; set; }

        public string? CaseID { get; set; }

        public string? Section { get; set; }

        public string? Html { get; set; }

        public string? DecisionHistoryHtml { get; set; }

        public string? Motion { get; set; }

        public string? ClassificationCode { get; set; }

        public string? ClassificationTitle { get; set; }

        public StorageAttachmentDTO Pdf { get; set; }

        public StorageAttachmentDTO DecisionHistoryPdf { get; set; }

        public List<StorageAttachmentDTO> Attachments { get; set; }
    }
}
