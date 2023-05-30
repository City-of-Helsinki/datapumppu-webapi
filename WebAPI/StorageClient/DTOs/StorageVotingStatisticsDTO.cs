namespace WebAPI.StorageClient.DTOs
{
    public class StorageVotingStatisticsDTO
    {
        public string Person { get; set; } = string.Empty;

        public string AdditionalInfoFi { get; set; } = string.Empty;

        public int For { get; set; }

        public int Against { get; set; }

        public int Empty { get; set; }

        public int Absent { get; set; }

        public int Sum { get; set; }

    }
}
