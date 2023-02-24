using System;

namespace H.Necessaire
{
    public class PartialDateTime
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? DayOfMonth { get; set; }

        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public int? Second { get; set; }

        public int? Millisecond { get; set; }

        public DateTimeKind DateTimeKind { get; set; } = DateTimeKind.Utc;

        public bool IsPrecise()
        {
            return
                !(Year is null)
                && !(Month is null)
                && !(DayOfMonth is null)
                && !(Hour is null)
                && !(Minute is null)
                && !(Second is null)
                && !(Millisecond is null)
                ;
        }

        public bool IsNever()
        {
            return
                Year is null
                && Month is null
                && DayOfMonth is null
                && Hour is null
                && Minute is null
                && Second is null
                && Millisecond is null
                ;
        }

        public DateTime? ToDateTime(int? fallbackYear = null)
        {
            if (IsNever())
                return null;

            return
                new DateTime(
                    Year ?? fallbackYear ?? DateTime.Today.Year,
                    Month ?? 1,
                    DayOfMonth ?? 1,
                    Hour ?? 0,
                    Minute ?? 0,
                    Second ?? 0,
                    Millisecond ?? 0,
                    DateTimeKind
                );
        }

        public DateTime? ToMinimumDateTime(int? fallbackYear = null)
        {
            if (IsNever())
                return null;

            return
                new DateTime(
                    Year ?? fallbackYear ?? DateTime.MinValue.Year,
                    Month ?? 1,
                    DayOfMonth ?? 1,
                    Hour ?? 0,
                    Minute ?? 0,
                    Second ?? 0,
                    Millisecond ?? 0,
                    DateTimeKind
                );
        }

        public DateTime? ToMaximumDateTime(int? fallbackYear = null)
        {
            if (IsNever())
                return null;

            return
                new DateTime(
                    Year ?? fallbackYear ?? DateTime.MaxValue.Year,
                    Month ?? 12,
                    DayOfMonth ?? DateTime.DaysInMonth(Year ?? DateTime.MaxValue.Year, Month ?? 12),
                    Hour ?? 23,
                    Minute ?? 59,
                    Second ?? 59,
                    Millisecond ?? 0,
                    DateTimeKind
                );
        }
    }
}
