﻿using System;

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

        public DayOfWeek[] WeekDays { get; set; } = null;

        public bool IsPrecise() => IsPreciseDate() && IsPreciseTime();

        public bool IsPreciseDate()
        {
            return
                !(Year is null)
                && !(Month is null)
                && !(DayOfMonth is null)
                ;
        }

        public bool IsPartialDate()
        {
            return
                !(Year is null)
                || !(Month is null)
                || !(DayOfMonth is null)
                ;
        }

        public bool IsPreciseTime()
        {
            return
                !(Hour is null)
                && !(Minute is null)
                && !(Second is null)
                ;
        }

        public bool IsPartialTime()
        {
            return
                !(Hour is null)
                || !(Minute is null)
                || !(Second is null)
                || !(Millisecond is null)
                ;
        }

        public bool IsWhenever() => IsWheneverDate() && IsWheneverTime() && WeekDays.IsEmpty();

        public bool IsWheneverDate()
        {
            return
                Year is null
                && Month is null
                && DayOfMonth is null
                ;
        }

        public bool IsWheneverTime()
        {
            return
                Hour is null
                && Minute is null
                && Second is null
                && Millisecond is null
                ;
        }

        public bool IsPreciseDateOnly() => IsPreciseDate() && IsWheneverTime();
        public bool IsPreciseTimeOnly() => IsWheneverDate() && IsPreciseTime();
        public bool IsPartialDateOnly() => IsPartialDate() && IsWheneverTime();
        public bool IsPartialTimeOnly() => IsWheneverDate() && IsPartialTime();
        public bool IsYearOnly() => IsPartialDateOnly() && (Month is null && DayOfMonth is null);

        public bool IsInSpecificWeekDays() => !WeekDays.IsEmpty();

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

        public DateTime? ToDateTime(int? fallbackYear = null, bool? isEntireDayIncluded = false)
        {
            if (IsWhenever())
                return null;

            DateTime now = DateTime.Now;
            DateTime utcNow = DateTime.UtcNow;
            int year = Year ?? fallbackYear ?? (DateTimeKind == DateTimeKind.Local ? now.Year : utcNow.Year);
            int month = Month ?? (DateTimeKind == DateTimeKind.Local ? now.Month : utcNow.Month);
            int dayOfMonth = DayOfMonth ?? (DateTimeKind == DateTimeKind.Local ? now.Day : utcNow.Day);


            if (IsWheneverTime() && isEntireDayIncluded != null)
            {
                return
                    new DateTime(
                        year,
                        month,
                        dayOfMonth,
                        hour: isEntireDayIncluded == false ? 0 : 23,
                        minute: isEntireDayIncluded == false ? 0 : 59,
                        second: isEntireDayIncluded == false ? 0 : 59,
                        millisecond: isEntireDayIncluded == false ? 0 : 999,
                        kind: DateTimeKind
                    );
            }

            return
                new DateTime(
                    year,
                    month,
                    dayOfMonth,
                    Hour ?? (isEntireDayIncluded is null ? (DateTimeKind == DateTimeKind.Local ? now.Hour : utcNow.Hour) : isEntireDayIncluded == false ? 0 : 23),
                    Minute ?? (isEntireDayIncluded is null ? (DateTimeKind == DateTimeKind.Local ? now.Minute : utcNow.Minute) : isEntireDayIncluded == false ? 0 : 59),
                    Second ?? (isEntireDayIncluded is null ? (DateTimeKind == DateTimeKind.Local ? now.Second : utcNow.Second) : isEntireDayIncluded == false ? 0 : 59),
                    Millisecond ?? (isEntireDayIncluded is null ? (DateTimeKind == DateTimeKind.Local ? now.Millisecond : utcNow.Millisecond) : isEntireDayIncluded == false ? 0 : 999),
                    DateTimeKind
                );
        }

        public DateTime? ToDateTimeAtStartOfDay(int? fallbackYear = null) => ToDateTime(fallbackYear, isEntireDayIncluded: false);
        public DateTime? ToDateTimeAtEndOfDay(int? fallbackYear = null) => ToDateTime(fallbackYear, isEntireDayIncluded: true);

        public DateTime? ToMinimumDateTime(int? fallbackYear = null)
        {
            if (IsWhenever())
                return null;

            DateTime now = DateTime.Now;
            DateTime utcNow = DateTime.UtcNow;
            int year = Year ?? fallbackYear ?? (DateTimeKind == DateTimeKind.Local ? now.Year : utcNow.Year);



            return
                new DateTime(
                    year,
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

            var now = DateTime.Now;
            var utcNow = DateTime.UtcNow;

            return
                new DateTime(
                    Year ?? fallbackYear ?? (DateTimeKind == DateTimeKind.Local ? now.Year : utcNow.Year),
                    Month ?? 12,
                    DayOfMonth ?? DateTime.DaysInMonth(Year ?? (DateTimeKind == DateTimeKind.Local ? now.Year : utcNow.Year), Month ?? 12),
                    Hour ?? 23,
                    Minute ?? 59,
                    Second ?? 59,
                    Millisecond ?? 999,
                    DateTimeKind
                );
        }

        public bool IsMatchingDateTime(DateTime? dateTime)
        {
            if (dateTime is null)
                return false;

            if (IsWhenever())
                return true;

            return
                IsAnyOrSame(Year, dateTime.Value.Year)
                && IsAnyOrSame(Month, dateTime.Value.Month)
                && IsAnyOrSame(DayOfMonth, dateTime.Value.Day)
                && IsAnyOrSame(Hour, dateTime.Value.Hour)
                && IsAnyOrSame(Minute, dateTime.Value.Minute)
                && IsAnyOrSame(Second, dateTime.Value.Second)
                && DateTimeKind == dateTime.Value.Kind
                && (WeekDays.IsEmpty() ? true : dateTime.Value.DayOfWeek.In(WeekDays))
                ;
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

            if (IsPreciseDateOnly())
                return $"{Year.Value.ToString("0000")}-{Month.Value.ToString("00")}-{DayOfMonth.Value.ToString("00")}";

            if (IsPartialDateOnly())
                return $"{Year?.ToString("0000") ?? "<Y>"}-{Month?.ToString("00") ?? "<M>"}-{DayOfMonth?.ToString("00") ?? "<D>"}";

            if (IsPreciseTimeOnly())
                return $"{Hour.Value.ToString("00")}:{Minute.Value.ToString("00")}:{Second.Value.ToString("00")}{(Millisecond is null ? "" : $".{Millisecond.Value.ToString("000")}")}";

            if (IsPartialTimeOnly())
                return $"{Hour?.ToString("00") ?? "<h>"}:{Minute?.ToString("00") ?? "<m>"}:{Second?.ToString("00") ?? "<s>"}{(Millisecond is null ? "" : $".{Millisecond.Value.ToString("000")}")}";

            return $"{Year?.ToString("0000") ?? "<Y>"}-{Month?.ToString("00") ?? "<M>"}-{DayOfMonth?.ToString("00") ?? "<D>"} {Hour?.ToString("00") ?? "<h>"}:{Minute?.ToString("00") ?? "<m>"}:{Second?.ToString("00") ?? "<s>"}{(Millisecond is null ? "" : $".{Millisecond.Value.ToString("000")}")}";
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
                WeekDays = WeekDays,
                DateTimeKind = DateTimeKind,
            };
        }

        public PartialDateTime OnYear(int year) => Duplicate().And(x => x.Year = year);
        public PartialDateTime OnMonth(int month) => Duplicate().And(x => x.Month = month);
        public PartialDateTime OnDayOfMonth(int dayOfMonth) => Duplicate().And(x => x.DayOfMonth = dayOfMonth);
        public PartialDateTime OnHour(int hour) => Duplicate().And(x => x.Hour = hour);
        public PartialDateTime OnMinute(int minute) => Duplicate().And(x => x.Minute = minute);
        public PartialDateTime OnSecond(int second) => Duplicate().And(x => x.Second = second);
        public PartialDateTime OnMillisecond(int millisecond) => Duplicate().And(x => x.Millisecond = millisecond);
        public PartialDateTime OnDate(DateTime date) => Duplicate().And(x =>
        {
            x.Year = x.Year ?? date.Year;
            x.Month = x.Month ?? date.Month;
            x.DayOfMonth = x.DayOfMonth ?? date.Day;
            x.DateTimeKind = x.DateTimeKind == DateTimeKind.Unspecified ? date.Kind : x.DateTimeKind;
        });
        public PartialDateTime OnTime(DateTime time) => Duplicate().And(x =>
        {
            x.Hour = x.Hour ?? time.Hour;
            x.Minute = x.Minute ?? time.Minute;
            x.Second = x.Second ?? time.Second;
            x.Millisecond = x.Millisecond ?? time.Millisecond;
            x.DateTimeKind = x.DateTimeKind == DateTimeKind.Unspecified ? time.Kind : x.DateTimeKind;
        });
        public PartialDateTime OnDateAndTime(DateTime dateAndTime) => Duplicate().And(x =>
        {
            x.Year = x.Year ?? dateAndTime.Year;
            x.Month = x.Month ?? dateAndTime.Month;
            x.DayOfMonth = x.DayOfMonth ?? dateAndTime.Day;
            x.Hour = x.Hour ?? dateAndTime.Hour;
            x.Minute = x.Minute ?? dateAndTime.Minute;
            x.Second = x.Second ?? dateAndTime.Second;
            x.Millisecond = x.Millisecond ?? dateAndTime.Millisecond;
            x.DateTimeKind = x.DateTimeKind == DateTimeKind.Unspecified ? dateAndTime.Kind : x.DateTimeKind;
        });
        public PartialDateTime OnWeekDays(params DayOfWeek[] daysOfWeek) => Duplicate().And(x => x.WeekDays = daysOfWeek);


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

        public static bool operator ==(PartialDateTime left, DateTime? right) => left is null ? right is null : left.IsMatchingDateTime(right);
        public static bool operator !=(PartialDateTime left, DateTime? right) => left is null ? !(right is null) : !left.IsMatchingDateTime(right);
        public static bool operator ==(DateTime? left, PartialDateTime right) => right is null ? left is null : right.IsMatchingDateTime(left);
        public static bool operator !=(DateTime? left, PartialDateTime right) => right is null ? !(left is null) : !right.IsMatchingDateTime(left);

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
