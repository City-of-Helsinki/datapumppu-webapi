﻿namespace WebAPI.Controllers.DTOs
{
    public class EditAgendaPointDTO
    {
        public string MeetingId { get; set; } = string.Empty;

        public int AgendaPoint { get; set; }

        public string EditorUserName { get; set; } = string.Empty;

        public string Html { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;
    }
}
