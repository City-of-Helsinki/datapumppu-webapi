﻿namespace WebAPI.StorageClient.DTOs
{
    public class StorageVotingDTO
    {
        public string? ForTitleFI { get; set; }

        public string? AgainstTitleFI { get; set; }

        public string? ForTitleSV { get; set; }

        public string? AgainstTitleSV { get; set; }

        public string? ForTextFI { get; set; }

        public string? ForTextSV { get; set; }

        public string? AgainstTextFI { get; set; }

        public string? AgainstTextSV { get; set; }

        public int ForCount { get; set; }

        public int AgainstCount { get; set; }

        public int EmptyCount { get; set; }

        public int AbsentCount { get; set; }

        public StorageVoteDTO[] Votes { get; set; } = new StorageVoteDTO[0];
    }
}
