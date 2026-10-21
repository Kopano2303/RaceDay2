namespace RaceDay.Models
{
    public class Result
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? OverallPosition { get; set; }

        public int? CategoryPosition { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? RecordedAt { get; set; }

        public Enrolment? Enrolment { get; set; }
    }
}