using System;

namespace H.Necessaire
{
    public class PeriodOfTime : IEquatable<PeriodOfTime>, IComparable<PeriodOfTime>
    {
        DateTime? from;
        public DateTime? From { get => from; set { from = value; SwapFromAndToIfNecessary(); } }

        DateTime? to;
        public DateTime? To { get => to; set { to = value; SwapFromAndToIfNecessary(); } }

        public TimeSpan? Duration => IsInfinite ? (null as TimeSpan?) : (To.Value - From.Value);
        public bool IsSinceForever => From is null;
        public bool IsUntilForever => To is null;
        public bool IsInfinite => IsSinceForever || IsUntilForever;
        public bool IsTimeless => IsSinceForever && IsUntilForever;

        public DateTime? ClosestTime(DateTime? asOf = null)
        {
            DateTime referenceTime = asOf ?? DateTime.UtcNow;
            TimeSpan? fromDistance = From is null ? (null as TimeSpan?) : referenceTime - From.Value;
            fromDistance = fromDistance is null ? (null as TimeSpan?) : (fromDistance.Value >= TimeSpan.Zero ? fromDistance.Value : -fromDistance.Value);
            TimeSpan? toDistance = To is null ? (null as TimeSpan?) : referenceTime - To.Value;
            toDistance = toDistance is null ? (null as TimeSpan?) : (toDistance.Value >= TimeSpan.Zero ? toDistance.Value : -toDistance.Value);

            if (fromDistance is null && toDistance is null)
                return null;
            if (!(fromDistance is null) && toDistance is null)
                return From;
            if (fromDistance is null && !(toDistance is null))
                return To;

            return
                fromDistance.Value <= toDistance.Value ? From : To;
        }

        public DateTime? FurthestTime(DateTime? asOf = null)
        {
            DateTime referenceTime = asOf ?? DateTime.UtcNow;
            TimeSpan? fromDistance = From is null ? (null as TimeSpan?) : referenceTime - From.Value;
            fromDistance = fromDistance is null ? (null as TimeSpan?) : (fromDistance.Value >= TimeSpan.Zero ? fromDistance.Value : -fromDistance.Value);
            TimeSpan? toDistance = To is null ? (null as TimeSpan?) : referenceTime - To.Value;
            toDistance = toDistance is null ? (null as TimeSpan?) : (toDistance.Value >= TimeSpan.Zero ? toDistance.Value : -toDistance.Value);

            if (fromDistance is null && toDistance is null)
                return null;
            if (!(fromDistance is null) && toDistance is null)
                return From;
            if (fromDistance is null && !(toDistance is null))
                return To;

            return
                fromDistance.Value >= toDistance.Value ? From : To;
        }

        public bool HasEnded(DateTime? asOf = null, bool isIntervalMarginConsideredEnded = false)
        {
            DateTime referenceTime = asOf ?? DateTime.UtcNow;
            return
                IsUntilForever
                ? false
                : isIntervalMarginConsideredEnded
                    ? referenceTime >= To.Value.EnsureUtc()
                    : referenceTime > To.Value.EnsureUtc()
                ;
        }

        public bool HasStarted(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true)
        {
            DateTime referenceTime = asOf ?? DateTime.UtcNow;
            return
                IsSinceForever
                ? true
                : isIntervalMarginConsideredStarted
                    ? referenceTime >= From.Value.EnsureUtc()
                    : referenceTime > From.Value.EnsureUtc()
                ;
        }

        public bool IsActive(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true, bool isIntervalMarginConsideredEnded = false)
        {
            if (IsTimeless)
                return true;

            DateTime referenceTime = asOf ?? DateTime.UtcNow;

            return
                HasStarted(asOf: referenceTime, isIntervalMarginConsideredStarted)
                &&
                !HasEnded(asOf: referenceTime, isIntervalMarginConsideredEnded)
                ;
        }

        public bool IsInactive(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true, bool isIntervalMarginConsideredEnded = false)
        {
            if (IsTimeless)
                return false;

            DateTime referenceTime = asOf ?? DateTime.UtcNow;

            return
                !HasStarted(asOf: referenceTime, isIntervalMarginConsideredStarted)
                ||
                HasEnded(asOf: referenceTime, isIntervalMarginConsideredEnded)
                ;
        }

        public bool IsOverlapping(PeriodOfTime other)
        {
            if (other is null)
                return false;

            if (IsTimeless || other.IsTimeless)
                return true;

            if (IsSinceForever && other.IsSinceForever)
                return true;

            if (IsSinceForever && (other.From.Value >= To.Value))
                return true;

            if (IsUntilForever && other.IsUntilForever)
                return true;

            if (IsUntilForever && (other.To.Value <= From.Value))
                return true;

            return
                other.From.Value.IsBetweenInclusive(From, To)
                ||
                other.To.Value.IsBetweenInclusive(From, To)
                ||
                (
                    other.From.Value < From.Value
                    &&
                    other.To.Value > To.Value
                )
                ;
        }

        public bool IsCompletelyBefore(PeriodOfTime other)
        {
            if (other is null || other.IsSinceForever)
                return false;

            if (IsSinceForever)
                return true;

            return From.Value < other.From.Value;
        }

        public bool IsBeforeOrIntersects(PeriodOfTime other)
        {
            if (IsSinceForever)
                return true;

            if (other is null || other.IsSinceForever)
                return false;

            return From.Value <= other.From.Value;
        }

        public bool IsCompletelyAfter(PeriodOfTime other)
        {
            if (other is null || other.IsUntilForever)
                return false;

            if (IsSinceForever)
                return false;

            return From.Value > other.To.Value;
        }

        public bool IsAfterOrIntersects(PeriodOfTime other)
        {
            if (other is null)
                return false;

            if (other.IsSinceForever)
                return true;

            if (IsSinceForever)
                return false;

            return From.Value > other.From.Value;
        }

        public PeriodOfTime Intersect(PeriodOfTime other)
        {
            if (other is null)
                return null;

            if (!IsOverlapping(other))
                return null;

            return
                new PeriodOfTime
                {
                    From = IsSinceForever && other.IsSinceForever ? null as DateTime? : (From ?? other.From).Value >= (other.From ?? From).Value ? (From ?? other.From).Value : (other.From ?? From).Value,
                    To = IsUntilForever && other.IsUntilForever ? null as DateTime? : (To ?? other.To).Value <= (other.To ?? To).Value ? (To ?? other.To).Value : (other.To ?? To).Value,
                }
                ;
        }

        public PeriodOfTime Unite(PeriodOfTime other, out PeriodOfTime gapPeriodIfAny)
        {
            if (other is null)
            {
                gapPeriodIfAny = null;
                return this;
            }

            gapPeriodIfAny = IsOverlapping(other) ? null : new PeriodOfTime
            {
                From = (To ?? other.To).Value <= (other.To ?? To).Value ? (To ?? other.To).Value : (other.To ?? To).Value,
                To = (From ?? other.From).Value >= (other.From ?? From).Value ? (From ?? other.From).Value : (other.From ?? From).Value,
            };

            return
                new PeriodOfTime
                {
                    From = IsSinceForever || other.IsSinceForever ? null as DateTime? : From.Value <= other.From.Value ? From.Value : other.From.Value,
                    To = IsUntilForever || other.IsUntilForever ? null as DateTime? : To.Value >= other.To.Value ? To.Value : other.To.Value,
                }
                ;
        }

        public PeriodOfTime Unite(PeriodOfTime other)
        {
            PeriodOfTime gapPeriodIfAny;
            return Unite(other, out gapPeriodIfAny);
        }

        public bool Equals(PeriodOfTime other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return IsSameAs(other);
        }

        public int CompareTo(PeriodOfTime other)
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

            return (From ?? DateTime.MinValue) > (other.From ?? DateTime.MinValue) ? 1 : -1;
        }

        public override bool Equals(object obj) => Equals(obj as PeriodOfTime);

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

        public static implicit operator PeriodOfTime(DateTime dateTime) => new PeriodOfTime { From = dateTime, To = dateTime, };
        public static implicit operator PeriodOfTime(DateTime? dateTime) => new PeriodOfTime { From = dateTime, To = dateTime, };
        public static implicit operator PeriodOfTime((DateTime?, DateTime?) tuple) => new PeriodOfTime { From = tuple.Item1, To = tuple.Item2, };
        public static implicit operator PeriodOfTime((DateTime, DateTime) tuple) => new PeriodOfTime { From = tuple.Item1, To = tuple.Item2, };
        public static implicit operator PeriodOfTime((DateTime, TimeSpan?) tuple) => new PeriodOfTime { From = tuple.Item1, To = tuple.Item2 is null ? null as DateTime? : tuple.Item1 + tuple.Item2.Value, };
        public static implicit operator PeriodOfTime((DateTime, TimeSpan) tuple) => new PeriodOfTime { From = tuple.Item1, To = tuple.Item1 + tuple.Item2, };
        public static implicit operator PeriodOfTime((TimeSpan?, DateTime) tuple) => new PeriodOfTime { From = tuple.Item1 is null ? null as DateTime? : tuple.Item2 - tuple.Item1.Value, To = tuple.Item2, };
        public static implicit operator PeriodOfTime((TimeSpan, DateTime) tuple) => new PeriodOfTime { From = tuple.Item2 - tuple.Item1, To = tuple.Item2, };

        public static bool operator ==(PeriodOfTime left, PeriodOfTime right) => left is null ? right is null : left.IsSameAs(right);
        public static bool operator !=(PeriodOfTime left, PeriodOfTime right) => left is null ? !(right is null) : !left.IsSameAs(right);
        public static bool operator >(PeriodOfTime left, PeriodOfTime right) => left is null ? false : left.IsCompletelyAfter(right);
        public static bool operator <(PeriodOfTime left, PeriodOfTime right) => left is null ? false : left.IsCompletelyBefore(right);
        public static bool operator >=(PeriodOfTime left, PeriodOfTime right) => left is null ? right is null : left.IsAfterOrIntersects(right);
        public static bool operator <=(PeriodOfTime left, PeriodOfTime right) => left is null ? right is null : left.IsBeforeOrIntersects(right);




        public static bool operator ==(DateTime dateTime, PeriodOfTime periodOfTime) => periodOfTime?.IsActive(asOf: dateTime) == true;
        public static bool operator !=(DateTime dateTime, PeriodOfTime periodOfTime) => periodOfTime?.IsInactive(asOf: dateTime) == true;
        public static bool operator >(DateTime dateTime, PeriodOfTime periodOfTime) => periodOfTime?.HasEnded(asOf: dateTime) == true;
        public static bool operator <(DateTime dateTime, PeriodOfTime periodOfTime) => periodOfTime?.HasStarted(asOf: dateTime) == false;
        public static bool operator >=(DateTime dateTime, PeriodOfTime periodOfTime) => periodOfTime?.IsActive(asOf: dateTime) == true || periodOfTime?.HasEnded(asOf: dateTime) == true;
        public static bool operator <=(DateTime dateTime, PeriodOfTime periodOfTime) => periodOfTime?.IsActive(asOf: dateTime) == true || periodOfTime?.HasStarted(asOf: dateTime) == false;

        bool IsSameAs(PeriodOfTime other)
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

            if (from.Value <= to.Value)
                return;

            DateTime tmpTo = to.Value;
            to = from;
            from = tmpTo;
        }
    }
}
