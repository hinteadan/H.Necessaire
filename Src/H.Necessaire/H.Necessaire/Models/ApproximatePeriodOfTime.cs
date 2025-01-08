using System;

namespace H.Necessaire
{
    public class ApproximatePeriodOfTime : IEquatable<ApproximatePeriodOfTime>, IComparable<ApproximatePeriodOfTime>
    {
        PartialPeriodOfTime startPeriod;
        public PartialPeriodOfTime StartPeriod { get => startPeriod; set { startPeriod = value; SwapFromAndToIfNecessary(); } }

        PartialPeriodOfTime endPeriod;
        public PartialPeriodOfTime EndPeriod { get => endPeriod; set { endPeriod = value; SwapFromAndToIfNecessary(); } }

        public DateTime? ClosestStartTime(DateTime? asOf = null, int? fallbackYear = null)
            => new PeriodOfTime
            {
                From = StartPeriod?.ToMinimumPeriodOfTime(fallbackYear)?.ClosestTime(asOf),
                To = StartPeriod?.ToMaximumPeriodOfTime(fallbackYear)?.ClosestTime(asOf),
            }.ClosestTime(asOf);

        public DateTime? FurthestStartTime(DateTime? asOf = null, int? fallbackYear = null)
            => new PeriodOfTime
            {
                From = StartPeriod?.ToMinimumPeriodOfTime(fallbackYear)?.ClosestTime(asOf),
                To = StartPeriod?.ToMaximumPeriodOfTime(fallbackYear)?.ClosestTime(asOf),
            }.FurthestTime(asOf);

        public DateTime? ClosestEndTime(DateTime? asOf = null, int? fallbackYear = null)
            => new PeriodOfTime
            {
                From = EndPeriod?.ToMinimumPeriodOfTime(fallbackYear)?.ClosestTime(asOf),
                To = EndPeriod?.ToMaximumPeriodOfTime(fallbackYear)?.ClosestTime(asOf),
            }.ClosestTime(asOf);

        public DateTime? FurthestEndTime(DateTime? asOf = null, int? fallbackYear = null)
            => new PeriodOfTime
            {
                From = EndPeriod?.ToMinimumPeriodOfTime(fallbackYear)?.ClosestTime(asOf),
                To = EndPeriod?.ToMaximumPeriodOfTime(fallbackYear)?.ClosestTime(asOf),
            }.FurthestTime(asOf);

        public PeriodOfTime ToMinimumPeriodOfTime(DateTime? asOf = null, int? fallbackYear = null)
            => new PeriodOfTime
            {
                From = ClosestStartTime(asOf, fallbackYear),
                To = ClosestEndTime(asOf, fallbackYear),
            };

        public PeriodOfTime ToMaximumPeriodOfTime(DateTime? asOf = null, int? fallbackYear = null)
            => new PeriodOfTime
            {
                From = FurthestStartTime(asOf, fallbackYear),
                To = FurthestEndTime(asOf, fallbackYear),
            };


        public bool IsSinceForever => (StartPeriod is null) || StartPeriod.IsTimeless;
        public bool IsUntilForever => (EndPeriod is null) || EndPeriod.IsTimeless;
        public bool IsInfinite => IsSinceForever || IsUntilForever;
        public bool IsTimeless => IsSinceForever && IsUntilForever;

        public TimeSpan? MinimumDuration => IsInfinite ? (null as TimeSpan?) : (ClosestStartTime(asOf: StartPeriod.To.ToMaximumDateTime(DateTime.Today.Year), DateTime.Today.Year).Value - ClosestEndTime(asOf: EndPeriod.From.ToMinimumDateTime(DateTime.Today.Year), DateTime.Today.Year).Value);
        public TimeSpan? MaximumDuration => IsInfinite ? (null as TimeSpan?) : (FurthestStartTime(asOf: StartPeriod.From.ToMinimumDateTime(DateTime.Today.Year), DateTime.Today.Year).Value - FurthestEndTime(asOf: EndPeriod.To.ToMaximumDateTime(DateTime.Today.Year), DateTime.Today.Year).Value);
        public TimeSpan? AverageDuration => IsInfinite ? (null as TimeSpan?) : TimeSpan.FromTicks((MaximumDuration.Value.Ticks - MinimumDuration.Value.Ticks) / 2);


        public bool IsPrecise() => !IsTimeless && StartPeriod.IsPrecise() && EndPeriod.IsPrecise();

        public bool HasPossiblyEnded(DateTime? asOf = null, bool isIntervalMarginConsideredEnded = false)
        {
            DateTime referenceTime = asOf ?? DateTime.UtcNow;
            return
                IsUntilForever
                ? false
                : EndPeriod.HasPossiblyEnded(asOf, isIntervalMarginConsideredEnded)
                ;
        }

        public bool HasSurelyEnded(DateTime? asOf = null, bool isIntervalMarginConsideredEnded = false)
        {
            DateTime referenceTime = asOf ?? DateTime.UtcNow;
            return
                IsUntilForever
                ? false
                : EndPeriod.HasSurelyEnded(asOf, isIntervalMarginConsideredEnded)
                ;
        }

        public bool HasPossiblyStarted(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true)
        {
            DateTime referenceTime = asOf ?? DateTime.UtcNow;
            return
                IsSinceForever
                ? true
                : StartPeriod.HasPossiblyStarted(asOf, isIntervalMarginConsideredStarted)
                ;
        }

        public bool HasSurelyStarted(DateTime? asOf = null, bool isIntervalMarginConsideredStarted = true)
        {
            DateTime referenceTime = asOf ?? DateTime.UtcNow;
            return
                IsSinceForever
                ? true
                : StartPeriod.HasSurelyStarted(asOf, isIntervalMarginConsideredStarted)
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

        public bool IsPossiblyOverlapping(ApproximatePeriodOfTime other)
        {
            if (other is null)
                return false;

            if (IsTimeless || other.IsTimeless)
                return true;

            if (IsSinceForever && other.IsSinceForever)
                return true;

            if (IsSinceForever && other.IsPossiblyActive(asOf: EndPeriod.To.ToMaximumDateTime().Value))
                return true;

            if (IsUntilForever && other.IsUntilForever)
                return true;

            if (IsUntilForever && other.IsPossiblyActive(asOf: StartPeriod.From.ToMinimumDateTime().Value))
                return true;

            return
                other.StartPeriod.From.ToMaximumDateTime().Value.IsBetweenInclusive(StartPeriod.From.ToMinimumDateTime().Value, EndPeriod.To.ToMaximumDateTime().Value)
                ||
                other.EndPeriod.To.ToMinimumDateTime().Value.IsBetweenInclusive(StartPeriod.From.ToMinimumDateTime().Value, EndPeriod.To.ToMaximumDateTime().Value)
                ||
                (
                    other.StartPeriod.From.ToMinimumDateTime().Value < StartPeriod.From.ToMaximumDateTime().Value
                    &&
                    other.EndPeriod.To.ToMaximumDateTime().Value > EndPeriod.To.ToMinimumDateTime().Value
                )
                ;
        }

        public bool IsSurelyOverlapping(ApproximatePeriodOfTime other)
        {
            if (other is null)
                return false;

            if (IsTimeless || other.IsTimeless)
                return true;

            if (IsSinceForever && other.IsSinceForever)
                return true;

            if (IsSinceForever && other.IsSurelyActive(asOf: EndPeriod.To.ToMinimumDateTime().Value))
                return true;

            if (IsUntilForever && other.IsUntilForever)
                return true;

            if (IsUntilForever && other.IsSurelyActive(asOf: StartPeriod.From.ToMaximumDateTime().Value))
                return true;

            return
                other.StartPeriod.From.ToMinimumDateTime().Value.IsBetweenInclusive(StartPeriod.From.ToMaximumDateTime().Value, EndPeriod.To.ToMinimumDateTime().Value)
                ||
                other.EndPeriod.To.ToMaximumDateTime().Value.IsBetweenInclusive(StartPeriod.From.ToMaximumDateTime().Value, EndPeriod.To.ToMinimumDateTime().Value)
                ||
                (
                    other.StartPeriod.From.ToMaximumDateTime().Value < StartPeriod.From.ToMinimumDateTime().Value
                    &&
                    other.EndPeriod.To.ToMinimumDateTime().Value > EndPeriod.To.ToMaximumDateTime().Value
                )
                ;
        }

        public bool IsCompletelyBefore(ApproximatePeriodOfTime other)
        {
            if (other is null || other.IsSinceForever)
                return false;

            if (IsSinceForever)
                return true;

            return StartPeriod.From.ToMaximumDateTime().Value < other.StartPeriod.From.ToMinimumDateTime().Value;
        }

        public bool IsBeforeOrIntersects(ApproximatePeriodOfTime other)
        {
            if (IsSinceForever)
                return true;

            if (other is null || other.IsSinceForever)
                return false;

            return StartPeriod.From.ToMinimumDateTime().Value <= other.StartPeriod.From.ToMaximumDateTime().Value;
        }

        public bool IsCompletelyAfter(ApproximatePeriodOfTime other)
        {
            if (other is null || other.IsUntilForever)
                return false;

            if (IsSinceForever)
                return false;

            return StartPeriod.From.ToMinimumDateTime().Value > other.EndPeriod.To.ToMaximumDateTime().Value;
        }

        public bool IsAfterOrIntersects(ApproximatePeriodOfTime other)
        {
            if (other is null)
                return false;

            if (other.IsSinceForever)
                return true;

            if (IsSinceForever)
                return false;

            return StartPeriod.From.ToMaximumDateTime().Value > other.StartPeriod.From.ToMinimumDateTime().Value;
        }

        public ApproximatePeriodOfTime Intersect(ApproximatePeriodOfTime other)
        {
            if (other is null)
                return null;

            if (!IsPossiblyOverlapping(other))
                return null;

            return
                new ApproximatePeriodOfTime
                {
                    StartPeriod
                        = (StartPeriod is null || StartPeriod.IsSinceForever) && (other.StartPeriod is null || other.StartPeriod.IsSinceForever)
                        ? null as PartialPeriodOfTime
                        : !StartPeriod.IsPossiblyOverlapping(other.StartPeriod)
                        ? ((StartPeriod.From >= other.StartPeriod.From) ? StartPeriod : other.StartPeriod)
                        : StartPeriod.Intersect(other.StartPeriod)
                        ,
                    EndPeriod
                        = (EndPeriod is null || EndPeriod.IsUntilForever) && (other.EndPeriod is null || other.EndPeriod.IsUntilForever)
                        ? null as PartialPeriodOfTime
                        : !EndPeriod.IsPossiblyOverlapping(other.EndPeriod)
                        ? ((EndPeriod.To <= other.EndPeriod.To) ? EndPeriod : other.EndPeriod)
                        : EndPeriod.Intersect(other.EndPeriod)
                };
        }

        public ApproximatePeriodOfTime Unite(ApproximatePeriodOfTime other, out ApproximatePeriodOfTime gapPeriodIfAny)
        {
            if (other is null)
            {
                gapPeriodIfAny = null;
                return this.Duplicate();
            }

            gapPeriodIfAny = IsPossiblyOverlapping(other) ? null : new ApproximatePeriodOfTime
            {
                StartPeriod = ((EndPeriod.From <= other.EndPeriod.From) ? EndPeriod : other.EndPeriod),
                EndPeriod = ((StartPeriod.To >= other.StartPeriod.To) ? StartPeriod : other.StartPeriod),
            };

            return
                new ApproximatePeriodOfTime
                {
                    StartPeriod = IsSinceForever || other.IsSinceForever ? null as PartialPeriodOfTime : (StartPeriod.From <= other.StartPeriod.From ? StartPeriod : other.StartPeriod),
                    EndPeriod = IsUntilForever || other.IsUntilForever ? null as PartialPeriodOfTime : (EndPeriod.To >= other.EndPeriod.To ? EndPeriod : other.EndPeriod),
                }
                ;
        }

        public ApproximatePeriodOfTime Unite(ApproximatePeriodOfTime other)
        {
            ApproximatePeriodOfTime gapPeriodIfAny;
            return Unite(other, out gapPeriodIfAny);
        }

        public bool Equals(ApproximatePeriodOfTime other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return IsSameAs(other);
        }
        public override bool Equals(object obj) => Equals(obj as ApproximatePeriodOfTime);
        public int CompareTo(ApproximatePeriodOfTime other)
        {
            if (other is null)
                return 1;

            if (IsSinceForever && !other.IsSinceForever)
                return 1;

            if (!IsSinceForever && other.IsSinceForever)
                return -1;

            if (IsSinceForever && other.IsSinceForever)
                return IsUntilForever ? 1 : other.IsUntilForever ? -1 : EndPeriod.From > other.EndPeriod.From ? 1 : EndPeriod.From < other.EndPeriod.From ? -1 : 0;

            if (this == other)
                return 0;

            return StartPeriod.From > other.StartPeriod.From ? 1 : -1;
        }
        public override int GetHashCode()
        {
            return StartPeriod.GetHashCode() ^ EndPeriod.GetHashCode();
        }
        public override string ToString()
        {
            if (IsTimeless)
                return "-∞ ~~ ∞";

            if (IsSinceForever)
                return $"-∞ ~~ [{EndPeriod}]";

            if (IsUntilForever)
                return $"[{StartPeriod}] ~~ ∞";

            return $"[{StartPeriod}] ~~ [{EndPeriod}]";
        }

        public ApproximatePeriodOfTime Duplicate() => new ApproximatePeriodOfTime { StartPeriod = StartPeriod?.Duplicate(), EndPeriod = EndPeriod?.Duplicate(), };

        public static implicit operator ApproximatePeriodOfTime(DateTime dateTime) => new ApproximatePeriodOfTime { StartPeriod = dateTime, EndPeriod = dateTime, };
        public static implicit operator ApproximatePeriodOfTime(DateTime? dateTime) => new ApproximatePeriodOfTime { StartPeriod = dateTime, EndPeriod = dateTime, };
        public static implicit operator ApproximatePeriodOfTime(PeriodOfTime periodOfTime) => new ApproximatePeriodOfTime { StartPeriod = periodOfTime, EndPeriod = periodOfTime, };
        public static implicit operator ApproximatePeriodOfTime(PartialDateTime partialDateTime) => new ApproximatePeriodOfTime { StartPeriod = partialDateTime, EndPeriod = partialDateTime, };
        public static implicit operator ApproximatePeriodOfTime(PartialPeriodOfTime partialPeriodOfTime) => new ApproximatePeriodOfTime { StartPeriod = partialPeriodOfTime, EndPeriod = partialPeriodOfTime, };
        public static implicit operator ApproximatePeriodOfTime((PartialPeriodOfTime, PartialPeriodOfTime) tuple) => new ApproximatePeriodOfTime { StartPeriod = tuple.Item1, EndPeriod = tuple.Item2, };
        public static implicit operator ApproximatePeriodOfTime((PeriodOfTime, PeriodOfTime) tuple) => new ApproximatePeriodOfTime { StartPeriod = tuple.Item1, EndPeriod = tuple.Item2, };
        public static implicit operator ApproximatePeriodOfTime((DateTime, DateTime) tuple) => new ApproximatePeriodOfTime { StartPeriod = tuple.Item1, EndPeriod = tuple.Item2, };
        public static implicit operator ApproximatePeriodOfTime((DateTime?, DateTime?) tuple) => new ApproximatePeriodOfTime { StartPeriod = tuple.Item1, EndPeriod = tuple.Item2, };
        public static implicit operator ApproximatePeriodOfTime((PartialDateTime, PartialDateTime) tuple) => new ApproximatePeriodOfTime { StartPeriod = tuple.Item1, EndPeriod = tuple.Item2, };

        public static bool operator ==(ApproximatePeriodOfTime left, ApproximatePeriodOfTime right) => left is null ? right is null : left.IsSameAs(right);
        public static bool operator !=(ApproximatePeriodOfTime left, ApproximatePeriodOfTime right) => left is null ? !(right is null) : !left.IsSameAs(right);


        bool IsSameAs(ApproximatePeriodOfTime other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return
                StartPeriod == other.StartPeriod
                && EndPeriod == other.EndPeriod
                ;
        }
        void SwapFromAndToIfNecessary()
        {
            if (startPeriod is null || endPeriod is null)
                return;

            if (startPeriod.From <= endPeriod.From)
                return;

            PartialPeriodOfTime tmpTo = endPeriod;
            endPeriod = startPeriod;
            startPeriod = tmpTo;
        }
    }
}
