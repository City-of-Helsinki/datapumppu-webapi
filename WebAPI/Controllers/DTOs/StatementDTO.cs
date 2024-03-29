﻿namespace WebAPI.Controllers.DTOs
{
    public class StatementDTO
    {
        public string Person { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int SpeechType { get; set; }

        public int DurationSeconds { get; set; }

        public string AdditionalInfoFI { get; set; } = string.Empty;

        public string AdditionalInfoSV { get; set; } = string.Empty;

        public int VideoPosition { get; set; }

        public string VideoLink { get; set; } = string.Empty;

        public int? CaseNumber { get; set; }

        public string ItemNumber { get; set; } = "0";

        public string? Title { get; set; }

        public string? MeetingId { get; set; }
    }
}
