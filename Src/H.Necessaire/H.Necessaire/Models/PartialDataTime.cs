using System;

namespace H.Necessaire
{
    public class PartialDataTime
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? DayOfMonth { get; set; }

        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public int? Second { get; set; }

        public DateTimeKind DateTimeKind { get; set; } = DateTimeKind.Utc;

        public DateTime ToDateTime()
        {
            return new DateTime(
                Year ?? DateTime.Today.Year,
                Month ?? 1,
                DayOfMonth ?? 1,
                Hour ?? 0,
                Minute ?? 0,
                Second ?? 0,
                DateTimeKind
            );
        }
    }
}
