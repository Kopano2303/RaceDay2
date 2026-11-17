
namespace RaceDay.Mvc.ViewModels
{
    public class ResultViewModel
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public int ParticipantID { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public TimeSpan FinishTime { get; set; }

        public int? OverallPosition { get; set; }

        public int? CategoryPosition { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime RecordedAt { get; set; }
    }
}
