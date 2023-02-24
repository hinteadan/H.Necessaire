using System;

namespace H.Necessaire
{
    public class PeriodOfTime
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public TimeSpan? Duration => IsInfinite ? (null as TimeSpan?) : (To.Value - From.Value);
        public TimeSpan? AbsoluteDuration => IsInfinite ? (null as TimeSpan?) : (Duration.Value >= TimeSpan.Zero ? Duration.Value : -Duration.Value);
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
    }
}
