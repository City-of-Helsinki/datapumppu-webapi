﻿namespace WebAPI.StorageClient.DTOs
{
    public class ParticipationsMeetingDTO
    {
        public string MeetingId { get; set; } = string.Empty;

        public List<int> AgendaPoint { get; set; } = new List<int>();
    }
}
