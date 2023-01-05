﻿namespace WebAPI.Controllers.DTOs
{
    public class StatementDTO
    {
        public string Person { get; set; } = string.Empty;

        public DateTime Started { get; set; }

        public DateTime Ended { get; set; }

        public int SpeechType { get; set; }

        public int DurationSeconds { get; set; }

        public string AdditionalInfoFI { get; set; } = string.Empty;

        public string AdditionalInfoSV { get; set; } = string.Empty;
    }
}
