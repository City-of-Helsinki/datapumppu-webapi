﻿namespace WebAPI.Controllers.DTOs
{
    public class ReservationDTO
    {
        public string MeetingID { get; set; }

        public int AgendaPoint { get; set; }

        public DateTime? Timestamp { get; set; }

        public string? Person { get; set; }

        public int? Ordinal { get; set; }

        public string? SeatID { get; set; }

        public string? AdditionalInfoFI { get; set; }

        public string? AdditionalInfoSV { get; set; }

        public bool? Active { get; set; }
    }
}
