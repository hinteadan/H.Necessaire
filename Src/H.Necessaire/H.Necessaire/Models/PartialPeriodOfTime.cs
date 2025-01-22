using System;
using System.Linq;

namespace H.Necessaire
{
    public class PartialPeriodOfTime : IEquatable<PartialPeriodOfTime>, IComparable<PartialPeriodOfTime>
    {
        PartialDateTime from;
        public PartialDateTime From { get => from; set { from = value; SwapFromAndToIfNecessary(); } }

        PartialDateTime to;
        public PartialDateTime To { get => to; set { to = value; SwapFromAndToIfNecessary(); } }

        public TimeSpan? Duration => CalculateDuration();


        public bool IsSinceForever => (From is null) || From.IsWhenever();
        public bool IsUntilForever => (To is null) || To.IsWhenever();
        public bool IsInfinite => IsSinceForever || IsUntilForever;
        public bool IsTimeless => IsSinceForever && IsUntilForever;


        public bool IsPrecise() => IsPreciseDate() && IsPreciseTime();
        public bool IsPreciseDate() => !IsTimeless && From.IsPreciseDate() && To.IsPreciseDate();
        public bool IsPartialDate() => !IsTimeless && From.IsPartialDate() && To.IsPartialDate();
        public bool IsPreciseTime() => !IsTimeless && From.IsPreciseTime() && To.IsPreciseTime();
        public bool IsPartialTime() => !IsTimeless && From.IsPartialTime() && To.IsPartialTime();
        public bool IsWheneverDate() => !IsTimeless && From.IsWheneverDate() && To.IsWheneverDate();
        public bool IsWheneverTime() => !IsTimeless && From.IsWheneverTime() && To.IsWheneverTime();
        public bool IsPreciseDateOnly() => IsPreciseDate() && IsWheneverTime();
        public bool IsPreciseTimeOnly() => IsWheneverDate() && IsPreciseTime();
        public bool IsPartialDateOnly() => IsPartialDate() && IsWheneverTime();
        public bool IsPartialTimeOnly() => IsWheneverDate() && IsPartialTime();


        public PeriodOfTime ToMinimumPeriodOfTime(int? fallbackYear = null) => From == To ? From.ToMinimumDateTime(fallbackYear) : new PeriodOfTime { From = From.ToMaximumDateTime(fallbackYear), To = To.ToMinimumDateTime(fallbackYear) };
        public PeriodOfTime ToMaximumPeriodOfTime(int? fallbackYear = null) => new PeriodOfTime { From = From.ToMinimumDateTime(fallbackYear), To = To.ToMaximumDateTime(fallbackYear) };

        public bool HasPossiblyEnded(DateTime? asOf = null, bool isIntervalMarginConsideredEnded = false)
        {
            DateTime referenceTime = asOf?.EnsureUtc() ?? DateTime.UtcNow;
            return
                IsUntilForever
                ? false
                : isIntervalMarginConsideredEnded
                    ? referenceTime >= To.ToMinimumDateTime().Value.EnsureUtc()
                    : referenceTime > To.ToMinimumDateTime().Value.EnsureUtc()
                ;
        }

        public bool HasSurelyEnded(DateTime? asOf = null, bool isIntervalMarginConsideredEnded = false)
        {
            DateTime referenceTime = asOf?.EnsureUtc() ?? DateTime.UtcNow;
            return
                IsUntilForever
                ? false
                : isIntervalMarginConsideredEnded
                    ? referenceTime >= To.ToMaximumDateTime().Value.EnsureUtc()
                    : referenceTime > To.ToMaximumDateTime().Value.EnsureUtc()
                ;
        }

        public bool HasPossiblyStarted(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true)
        {
            DateTime referenceTime = asOf?.EnsureUtc() ?? DateTime.UtcNow;
            return
                IsSinceForever
                ? true
                : isIntervalMarginConsideredStarted
                    ? referenceTime >= From.ToMinimumDateTime().Value.EnsureUtc()
                    : referenceTime > From.ToMinimumDateTime().Value.EnsureUtc()
                ;
        }

        public bool HasSurelyStarted(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true)
        {
            DateTime referenceTime = asOf?.EnsureUtc() ?? DateTime.UtcNow;
            return
                IsSinceForever
                ? true
                : isIntervalMarginConsideredStarted
                    ? referenceTime >= From.ToMaximumDateTime().Value.EnsureUtc()
                    : referenceTime > From.ToMaximumDateTime().Value.EnsureUtc()
                ;
        }

        public bool IsPossiblyActive(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true, bool isIntervalMarginConsideredEnded = false)
        {
            if (IsTimeless)
                return true;

            DateTime referenceTime = asOf ?? DateTime.UtcNow;

            return
                HasPossiblyStarted(asOf: referenceTime, isIntervalMarginConsideredStarted)
                &&
                !HasSurelyEnded(asOf: referenceTime, isIntervalMarginConsideredEnded)
                ;
        }

        public bool IsSurelyActive(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true, bool isIntervalMarginConsideredEnded = false)
        {
            if (IsTimeless)
                return true;

            DateTime referenceTime = asOf ?? DateTime.UtcNow;

            return
                HasSurelyStarted(asOf: referenceTime, isIntervalMarginConsideredStarted)
                &&
                !HasPossiblyEnded(asOf: referenceTime, isIntervalMarginConsideredEnded)
                ;
        }

        public bool IsPossiblyInactive(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true, bool isIntervalMarginConsideredEnded = false)
        {
            if (IsTimeless)
                return false;

            DateTime referenceTime = asOf ?? DateTime.UtcNow;

            return
                !HasSurelyStarted(asOf: referenceTime, isIntervalMarginConsideredStarted)
                ||
                HasPossiblyEnded(asOf: referenceTime, isIntervalMarginConsideredEnded)
                ;
        }

        public bool IsSurelyInactive(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true, bool isIntervalMarginConsideredEnded = false)
        {
            if (IsTimeless)
                return false;

            DateTime referenceTime = asOf ?? DateTime.UtcNow;

            return
                !HasPossiblyStarted(asOf: referenceTime, isIntervalMarginConsideredStarted)
                ||
                HasSurelyEnded(asOf: referenceTime, isIntervalMarginConsideredEnded)
                ;
        }

        public bool IsPossiblyOverlapping(PartialPeriodOfTime other)
        {
            if (other is null)
                return false;

            if (IsTimeless || other.IsTimeless)
                return true;

            if (IsSinceForever && other.IsSinceForever)
                return true;

            if (IsSinceForever && other.IsPossiblyActive(asOf: To.ToMaximumDateTime().Value))
                return true;

            if (IsUntilForever && other.IsUntilForever)
                return true;

            if (IsUntilForever && other.IsPossiblyActive(asOf: From.ToMinimumDateTime().Value))
                return true;

            return
                other.From.ToMaximumDateTime().Value.IsBetweenInclusive(From.ToMinimumDateTime().Value, To.ToMaximumDateTime().Value)
                ||
                other.To.ToMinimumDateTime().Value.IsBetweenInclusive(From.ToMinimumDateTime().Value, To.ToMaximumDateTime().Value)
                ||
                (
                    other.From.ToMinimumDateTime().Value < From.ToMaximumDateTime().Value
                    &&
                    other.To.ToMaximumDateTime().Value > To.ToMinimumDateTime().Value
                )
                ;
        }

        public bool IsSurelyOverlapping(PartialPeriodOfTime other)
        {
            if (other is null)
                return false;

            if (IsTimeless || other.IsTimeless)
                return true;

            if (IsSinceForever && other.IsSinceForever)
                return true;

            if (IsSinceForever && other.IsSurelyActive(asOf: To.ToMinimumDateTime().Value))
                return true;

            if (IsUntilForever && other.IsUntilForever)
                return true;

            if (IsUntilForever && other.IsSurelyActive(asOf: From.ToMaximumDateTime().Value))
                return true;

            return
                other.From.ToMinimumDateTime().Value.IsBetweenInclusive(From.ToMaximumDateTime().Value, To.ToMinimumDateTime().Value)
                ||
                other.To.ToMaximumDateTime().Value.IsBetweenInclusive(From.ToMaximumDateTime().Value, To.ToMinimumDateTime().Value)
                ||
                (
                    other.From.ToMaximumDateTime().Value < From.ToMinimumDateTime().Value
                    &&
                    other.To.ToMinimumDateTime().Value > To.ToMaximumDateTime().Value
                )
                ;
        }

        public bool IsCompletelyBefore(PartialPeriodOfTime other)
        {
            if (other is null || other.IsSinceForever)
                return false;

            if (IsSinceForever)
                return true;

            return From.ToMaximumDateTime().Value < other.From.ToMinimumDateTime().Value;
        }
        public bool IsBeforeOrIntersects(PartialPeriodOfTime other)
        {
            if (IsSinceForever)
                return true;

            if (other is null || other.IsSinceForever)
                return false;

            return From.ToMinimumDateTime().Value <= other.From.ToMaximumDateTime().Value;
        }
        public bool IsCompletelyAfter(PartialPeriodOfTime other)
        {
            if (other is null || other.IsUntilForever)
                return false;

            if (IsSinceForever)
                return false;

            return From.ToMinimumDateTime().Value > other.To.ToMaximumDateTime().Value;
        }
        public bool IsAfterOrIntersects(PartialPeriodOfTime other)
        {
            if (other is null)
                return false;

            if (other.IsSinceForever)
                return true;

            if (IsSinceForever)
                return false;

            return From.ToMaximumDateTime().Value > other.From.ToMinimumDateTime().Value;
        }

        public PartialPeriodOfTime Intersect(PartialPeriodOfTime other)
        {
            if (other is null)
                return null;

            if (!IsPossiblyOverlapping(other))
                return null;

            return
                new PartialPeriodOfTime
                {
                    From
                        = IsSinceForever && other.IsSinceForever
                        ? null as PartialDateTime
                        : new PartialDateTime
                        {
                            Year = IntersectFromPart(From.Year, other.From.Year),
                            Month = IntersectFromPart(From.Month, other.From.Month),
                            DayOfMonth = IntersectFromPart(From.DayOfMonth, other.From.DayOfMonth),
                            Hour = IntersectFromPart(From.Hour, other.From.Hour),
                            Minute = IntersectFromPart(From.Minute, other.From.Minute),
                            Second = IntersectFromPart(From.Second, other.From.Second),
                            Millisecond = IntersectFromPart(From.Millisecond, other.From.Millisecond),
                            DateTimeKind = IntersectKind(From.DateTimeKind, other.From.DateTimeKind),
                            DaysOfWeek = From.DaysOfWeek.IsEmpty() || other.From.DaysOfWeek.IsEmpty() ? null : From.DaysOfWeek.Intersect(other.From.DaysOfWeek).ToArray(),
                        },
                    To
                        = IsUntilForever && other.IsUntilForever
                        ? null as PartialDateTime
                        : new PartialDateTime
                        {
                            Year = IntersectToPart(To.Year, other.To.Year),
                            Month = IntersectToPart(To.Month, other.To.Month),
                            DayOfMonth = IntersectToPart(To.DayOfMonth, other.To.DayOfMonth),
                            Hour = IntersectToPart(To.Hour, other.To.Hour),
                            Minute = IntersectToPart(To.Minute, other.To.Minute),
                            Second = IntersectToPart(To.Second, other.To.Second),
                            Millisecond = IntersectToPart(To.Millisecond, other.To.Millisecond),
                            DateTimeKind = IntersectKind(To.DateTimeKind, other.To.DateTimeKind),
                            DaysOfWeek = To.DaysOfWeek.IsEmpty() || other.To.DaysOfWeek.IsEmpty() ? null : To.DaysOfWeek.Intersect(other.To.DaysOfWeek).ToArray(),
                        },
                };
        }

        public PartialPeriodOfTime Unite(PartialPeriodOfTime other, out PartialPeriodOfTime gapPeriodIfAny)
        {
            if (other is null)
            {
                gapPeriodIfAny = null;
                return this.Duplicate();
            }

            gapPeriodIfAny = IsPossiblyOverlapping(other) ? null : new PartialPeriodOfTime
            {
                From = (To ?? other.To) <= (other.To ?? To) ? (To ?? other.To) : (other.To ?? To),
                To = (From ?? other.From) >= (other.From ?? From) ? (From ?? other.From) : (other.From ?? From),
            };

            return
                new PartialPeriodOfTime
                {
                    From = IsSinceForever || other.IsSinceForever ? null as PartialDateTime : From <= other.From ? From : other.From,
                    To = IsUntilForever || other.IsUntilForever ? null as PartialDateTime : To >= other.To ? To : other.To,
                }
                ;
        }

        public PartialPeriodOfTime Unite(PartialPeriodOfTime other)
        {
            PartialPeriodOfTime gapPeriodIfAny;
            return Unite(other, out gapPeriodIfAny);
        }

        public PartialPeriodOfTime OnYear(int year) => Duplicate().And(x => { x.From = x.From?.OnYear(year); x.To = x.To?.OnYear(year); });
        public PartialPeriodOfTime OnMonth(int month) => Duplicate().And(x => { x.From = x.From?.OnMonth(month); x.To = x.To?.OnMonth(month); });
        public PartialPeriodOfTime OnDayOfMonth(int dayOfMonth) => Duplicate().And(x => { x.From = x.From?.OnDayOfMonth(dayOfMonth); x.To = x.To?.OnDayOfMonth(dayOfMonth); });
        public PartialPeriodOfTime OnHour(int hour) => Duplicate().And(x => { x.From = x.From?.OnHour(hour); x.To = x.To?.OnHour(hour); });
        public PartialPeriodOfTime OnMinute(int minute) => Duplicate().And(x => { x.From = x.From?.OnMinute(minute); x.To = x.To?.OnMinute(minute); });
        public PartialPeriodOfTime OnSecond(int second) => Duplicate().And(x => { x.From = x.From?.OnSecond(second); x.To = x.To?.OnSecond(second); });
        public PartialPeriodOfTime OnMillisecond(int millisecond) => Duplicate().And(x => { x.From = x.From?.OnMillisecond(millisecond); x.To = x.To?.OnMillisecond(millisecond); });
        public PartialPeriodOfTime OnDate(DateTime date) => Duplicate().And(x => { x.From = x.From?.OnDate(date); x.To = x.To?.OnDate(date); });
        public PartialPeriodOfTime OnTime(DateTime time) => Duplicate().And(x => { x.From = x.From?.OnTime(time); x.To = x.To?.OnTime(time); });
        public PartialPeriodOfTime OnDateAndTime(DateTime dateAndTime) => Duplicate().And(x => { x.From = x.From?.OnDateAndTime(dateAndTime); x.To = x.To?.OnDateAndTime(dateAndTime); });

        public bool Equals(PartialPeriodOfTime other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return IsSameAs(other);
        }

        public override bool Equals(object obj) => Equals(obj as PartialPeriodOfTime);

        public int CompareTo(PartialPeriodOfTime other)
        {
            if (other is null)
                return 1;

            if (IsSinceForever && !other.IsSinceForever)
                return 1;

            if (!IsSinceForever && other.IsSinceForever)
                return -1;

            if (IsSinceForever && other.IsSinceForever)
                return IsUntilForever ? 1 : other.IsUntilForever ? -1 : To > other.To ? 1 : To < other.To ? -1 : 0;

            if (this == other)
                return 0;

            return From > other.From ? 1 : -1;
        }

        public override int GetHashCode()
        {
            return From.GetHashCode() ^ To.GetHashCode();
        }

        public override string ToString()
        {
            if (IsTimeless)
                return "-∞ ~ ∞";

            if (IsSinceForever)
                return $"-∞ ~ {To}";

            if (IsUntilForever)
                return $"{From} ~ ∞";

            return $"{From} ~ {To}";
        }

        public PartialPeriodOfTime Duplicate() => new PartialPeriodOfTime { From = From?.Duplicate(), To = To?.Duplicate(), };

        public static implicit operator PartialPeriodOfTime(PartialDateTime partialDateTime) => new PartialPeriodOfTime { From = partialDateTime?.Duplicate(), To = partialDateTime?.Duplicate(), };
        public static implicit operator PartialPeriodOfTime((PartialDateTime, PartialDateTime) tuple) => new PartialPeriodOfTime { From = tuple.Item1?.Duplicate(), To = tuple.Item2?.Duplicate(), };
        public static implicit operator PeriodOfTime(PartialPeriodOfTime partialPeriodOfTime) => new PeriodOfTime { From = partialPeriodOfTime.From, To = partialPeriodOfTime.To, };

        public static bool operator ==(PartialPeriodOfTime left, PartialPeriodOfTime right) => left is null ? right is null : left.IsSameAs(right);
        public static bool operator !=(PartialPeriodOfTime left, PartialPeriodOfTime right) => left is null ? !(right is null) : !left.IsSameAs(right);
        public static bool operator ==(DateTime dateTime, PartialPeriodOfTime partialPeriodOfTime) => partialPeriodOfTime?.IsSurelyActive(asOf: dateTime) == true;
        public static bool operator !=(DateTime dateTime, PartialPeriodOfTime partialPeriodOfTime) => partialPeriodOfTime?.IsSurelyInactive(asOf: dateTime) == true;
        public static bool operator >(DateTime dateTime, PartialPeriodOfTime partialPeriodOfTime) => partialPeriodOfTime?.HasSurelyEnded(asOf: dateTime) == true;
        public static bool operator <(DateTime dateTime, PartialPeriodOfTime partialPeriodOfTime) => partialPeriodOfTime?.HasPossiblyStarted(asOf: dateTime) == false;
        public static bool operator >=(DateTime dateTime, PartialPeriodOfTime partialPeriodOfTime) => partialPeriodOfTime?.IsSurelyActive(asOf: dateTime) == true || partialPeriodOfTime?.HasSurelyEnded(asOf: dateTime) == true;
        public static bool operator <=(DateTime dateTime, PartialPeriodOfTime partialPeriodOfTime) => partialPeriodOfTime?.IsSurelyActive(asOf: dateTime) == true || partialPeriodOfTime?.HasPossiblyStarted(asOf: dateTime) == false;

        TimeSpan? CalculateDuration()
        {
            if (IsInfinite)
                return null;

            DateTime now = DateTime.UtcNow;

            int toYear = To.Year ?? now.Year;
            int toMonth = To.Month ?? (To.IsYearOnly() ? 12 : now.Month);
            DateTime to = To.OnDateAndTime(new DateTime(
                toYear,
                toMonth,
                To.DayOfMonth ?? (To.IsPartialTimeOnly() ? now.Day : DateTime.DaysInMonth(toYear, toMonth)),
                To.Hour ?? (To.IsPartialTimeOnly() ? now.Hour : 23),
                To.Minute ?? (To.IsPartialTimeOnly() && To.Hour is null ? now.Minute : 59),
                To.Second ?? (To.IsPartialTimeOnly() && To.Hour is null && To.Minute is null ? now.Second : 59),
                To.Millisecond ?? (To.IsPartialTimeOnly() && To.Hour is null && To.Minute is null && To.Second is null ? now.Millisecond : 999),
                DateTimeKind.Utc)
            ).ToDateTime().Value;

            DateTime from = From.OnDateAndTime(new DateTime(
                From.Year ?? now.Year,
                From.Month ?? (From.IsYearOnly() ? 1 : now.Month),
                From.DayOfMonth ?? (From.IsPartialTimeOnly() ? now.Day : 1),
                From.Hour ?? (From.IsPartialTimeOnly() ? now.Hour : 0),
                From.Minute ?? (From.IsPartialTimeOnly() && From.Hour is null ? now.Minute : 0),
                From.Second ?? (From.IsPartialTimeOnly() && From.Hour is null && From.Minute is null ? now.Second : 0),
                From.Millisecond ?? (From.IsPartialTimeOnly() && From.Hour is null && From.Minute is null && From.Second is null ? now.Millisecond : 0),
                DateTimeKind.Utc)
            ).ToDateTime().Value;

            return to - from;
        }

        bool IsSameAs(PartialPeriodOfTime other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return
                From == other.From
                && To == other.To
                ;
        }

        void SwapFromAndToIfNecessary()
        {
            if (from is null || to is null)
                return;

            if (from <= to)
                return;

            PartialDateTime tmpTo = to;
            to = from;
            from = tmpTo;
        }

        static int? IntersectFromPart(int? left, int? right)
        {
            return
                (left is null && right is null)
                ? null
                : (!(left is null) && !(right is null))
                ? Math.Max(left.Value, right.Value)
                : (left is null ? right : left)
                ;
        }

        static int? IntersectToPart(int? left, int? right)
        {
            return
                (left is null && right is null)
                ? null
                : (!(left is null) && !(right is null))
                ? Math.Min(left.Value, right.Value)
                : (left is null ? right : left)
                ;
        }

        static DateTimeKind IntersectKind(DateTimeKind left, DateTimeKind right)
        {
            return
                left == right
                ? left
                : (left == DateTimeKind.Utc || right == DateTimeKind.Utc)
                ? DateTimeKind.Utc
                : (left == DateTimeKind.Local || right == DateTimeKind.Local)
                ? DateTimeKind.Local
                : DateTimeKind.Unspecified
                ;
        }
    }
}
