namespace RaceDay.Models
{
    public class EventCategory
    {
        public int EventCategoryID { get; set; }

        public int EventID { get; set; }

        public int CategoryID { get; set; }

        public decimal EntryFee { get; set; }

        public int? MaximumParticipants { get; set; }

        public Event? Event { get; set; }

        public Category? Category { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; } =
            new List<Enrolment>();
    }
}