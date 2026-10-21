namespace RaceDay.DTOs
{
    public class CreateResultRequest
    {
        public int EnrolmentID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? OverallPosition { get; set; }

        public int? CategoryPosition { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}