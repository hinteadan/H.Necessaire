using System;

namespace H.Necessaire
{
    public class PartialDateTime : IEquatable<PartialDateTime>, IComparable<PartialDateTime>
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

        public bool IsWhenever()
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

        /// <summary>
        /// Checks if the current instance is overlapping with the other instance.
        /// </summary>
        /// <remarks>
        /// Does not consider DateTimeKind.
        /// </remarks>
        /// <param name="other">The PartialDateTime to compare against</param>
        /// <returns>true if they're overlapping on any element, false otherwise</returns>
        public bool IsOverlapping(PartialDateTime other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (IsWhenever() || other.IsWhenever())
                return false;

            if (IsPrecise() && other.IsPrecise())
                return IsSameAs(other);

            return
                IsAnyOrSame(Year, other.Year)
                && IsAnyOrSame(Month, other.Month)
                && IsAnyOrSame(DayOfMonth, other.DayOfMonth)
                && IsAnyOrSame(Hour, other.Hour)
                && IsAnyOrSame(Minute, other.Minute)
                && IsAnyOrSame(Second, other.Second)
                && IsAnyOrSame(Millisecond, other.Millisecond)
                ;
        }

        public DateTime? ToDateTime(int? fallbackYear = null)
        {
            if (IsWhenever())
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
            if (IsWhenever())
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

        public DateTime? ToMaximumDateTime(int? fallbackYear = null)
        {
            if (IsWhenever())
                return null;

            return
                new DateTime(
                    Year ?? fallbackYear ?? DateTime.Today.Year,
                    Month ?? 12,
                    DayOfMonth ?? DateTime.DaysInMonth(Year ?? DateTime.Today.Year, Month ?? 12),
                    Hour ?? 23,
                    Minute ?? 59,
                    Second ?? 59,
                    Millisecond ?? 999,
                    DateTimeKind
                );
        }

        public PeriodOfTime ToPeriodOfTime(int? fallbackStartYear = null, int? fallbackEndYear = null)
        {
            return new PeriodOfTime
            {
                From = ToMinimumDateTime(fallbackStartYear),
                To = ToMaximumDateTime(fallbackEndYear)
            };
        }

        public bool Equals(PartialDateTime other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return IsSameAs(other);
        }

        /// <summary>
        /// Checks if the current instance is before the other instance.
        /// </summary>
        /// <remarks>
        /// Does not consider DateTimeKind.
        /// </remarks>
        /// <param name="other">The PartialDateTime to compare against</param>
        /// <returns>true if this instance is before the other, false otherwise</returns>
        public bool IsBefore(PartialDateTime other)
        {
            if (other is null)
                return false;

            if (IsWhenever() || other.IsWhenever())
                return false;

            if (IsPrecise() && other.IsPrecise())
                return Year < other.Year
                    || (Year == other.Year && Month < other.Month)
                    || (Year == other.Year && Month == other.Month && DayOfMonth < other.DayOfMonth)
                    || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour < other.Hour)
                    || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && Minute < other.Minute)
                    || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && Minute == other.Minute && Second < other.Second)
                    || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && Minute == other.Minute && Second == other.Second && Millisecond < other.Millisecond)
                    ;

            return
                IsAnyOr(Year, other.Year, Year < other.Year)
                || (Year == other.Year && IsAnyOr(Month, other.Month, Month < other.Month))
                || (Year == other.Year && Month == other.Month && IsAnyOr(DayOfMonth, other.DayOfMonth, DayOfMonth < other.DayOfMonth))
                || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && IsAnyOr(Hour, other.Hour, Hour < other.Hour))
                || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && IsAnyOr(Minute, other.Minute, Minute < other.Minute))
                || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && Minute == other.Minute && IsAnyOr(Second, other.Second, Second < other.Second))
                || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && Minute == other.Minute && Second == other.Second && IsAnyOr(Millisecond, other.Millisecond, Millisecond < other.Millisecond))
                ;
        }

        /// <summary>
        /// Checks if the current instance is after the other instance.
        /// </summary>
        /// <remarks>
        /// Does not consider DateTimeKind.
        /// </remarks>
        /// <param name="other">The PartialDateTime to compare against</param>
        /// <returns>true if this instance is after the other, false otherwise</returns>
        public bool IsAfter(PartialDateTime other)
        {
            if (other is null)
                return false;

            if (IsWhenever() || other.IsWhenever())
                return false;

            if (IsPrecise() && other.IsPrecise())
                return Year < other.Year
                    || (Year == other.Year && Month > other.Month)
                    || (Year == other.Year && Month == other.Month && DayOfMonth > other.DayOfMonth)
                    || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour > other.Hour)
                    || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && Minute > other.Minute)
                    || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && Minute == other.Minute && Second > other.Second)
                    || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && Minute == other.Minute && Second == other.Second && Millisecond > other.Millisecond)
                    ;

            return
                IsAnyOr(Year, other.Year, Year > other.Year)
                || (Year == other.Year && IsAnyOr(Month, other.Month, Month > other.Month))
                || (Year == other.Year && Month == other.Month && IsAnyOr(DayOfMonth, other.DayOfMonth, DayOfMonth > other.DayOfMonth))
                || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && IsAnyOr(Hour, other.Hour, Hour > other.Hour))
                || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && IsAnyOr(Minute, other.Minute, Minute > other.Minute))
                || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && Minute == other.Minute && IsAnyOr(Second, other.Second, Second > other.Second))
                || (Year == other.Year && Month == other.Month && DayOfMonth == other.DayOfMonth && Hour == other.Hour && Minute == other.Minute && Second == other.Second && IsAnyOr(Millisecond, other.Millisecond, Millisecond > other.Millisecond))
                ;
        }

        public bool IsBeforeOrEqual(PartialDateTime other)
        {
            return Equals(other) || IsBefore(other);
        }

        public bool IsAftereOrEqual(PartialDateTime other)
        {
            return Equals(other) || IsAfter(other);
        }

        public override bool Equals(object obj) => Equals(obj as PartialDateTime);

        public override int GetHashCode()
        {
            return Year.GetHashCode() ^ Month.GetHashCode() ^ DayOfMonth.GetHashCode() ^ Hour.GetHashCode() ^ Minute.GetHashCode() ^ Second.GetHashCode() ^ Millisecond.GetHashCode() ^ DateTimeKind.GetHashCode();
        }

        public int CompareTo(PartialDateTime other)
        {
            if (other is null)
                return 1;

            if (IsWhenever() && !other.IsWhenever())
                return 1;

            if (!IsWhenever() && other.IsWhenever())
                return -1;

            if (this == other)
                return 0;

            return this > other ? 1 : -1;
        }

        public override string ToString()
        {
            if (IsWhenever())
                return "∞";

            if (IsPrecise())
                return $"{ToDateTime()}";

            return $"{Year?.ToString() ?? "[*Y]"}-{Month?.ToString("00") ?? "[*M]"}-{DayOfMonth?.ToString("00") ?? "[*D]"} {Hour?.ToString("00") ?? "[*h]"}:{Minute?.ToString("00") ?? "[*m]"}:{Second?.ToString("00") ?? "[*s]"}.{Millisecond?.ToString("000") ?? "[*ms]"} {DateTimeKind}";
        }

        public PartialDateTime Duplicate()
        {
            return new PartialDateTime
            {
                Year = Year,
                Month = Month,
                DayOfMonth = DayOfMonth,
                Hour = Hour,
                Minute = Minute,
                Second = Second,
                Millisecond = Millisecond,
                DateTimeKind = DateTimeKind
            };
        }


        public static implicit operator DateTime?(PartialDateTime partialDateTime) => partialDateTime?.ToDateTime();
        public static implicit operator PartialDateTime(DateTime? dateTime) => dateTime is null ? new PartialDateTime() : dateTime.Value.ToPartialDateTime();
        public static implicit operator PartialDateTime(DateTime dateTime) => dateTime.ToPartialDateTime();
        public static implicit operator PeriodOfTime(PartialDateTime partialDateTime) => partialDateTime.ToPeriodOfTime();
        public static bool operator ==(PartialDateTime left, PartialDateTime right) => left is null ? right is null : left.IsSameAs(right);
        public static bool operator !=(PartialDateTime left, PartialDateTime right) => left is null ? !(right is null) : !left.IsSameAs(right);
        public static bool operator >(PartialDateTime left, PartialDateTime right) => left is null ? false : left.IsAfter(right);
        public static bool operator <(PartialDateTime left, PartialDateTime right) => left is null ? false : left.IsBefore(right);
        public static bool operator >=(PartialDateTime left, PartialDateTime right) => left is null ? right is null : left.IsAftereOrEqual(right);
        public static bool operator <=(PartialDateTime left, PartialDateTime right) => left is null ? right is null : left.IsBeforeOrEqual(right);

        bool IsSameAs(PartialDateTime other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return
                Year == other.Year
                && Month == other.Month
                && DayOfMonth == other.DayOfMonth
                && Hour == other.Hour
                && Minute == other.Minute
                && Second == other.Second
                && Millisecond == other.Millisecond
                && DateTimeKind == other.DateTimeKind
                ;
        }

        static bool IsAnyOrSame(int? a, int? b)
        {
            return IsAnyOr(a, b, a == b);
        }

        static bool IsAnyOr(int? a, int? b, bool or)
        {
            return a is null || b is null || or;
        }
    }
}
