using System;

namespace H.Necessaire
{
    public class PartialPeriodOfTime
    {
        public PartialDateTime From { get; set; }
        public PartialDateTime To { get; set; }

        public TimeSpan? MinimumDuration => IsInfinite ? (null as TimeSpan?) : (To.ToMinimumDateTime(DateTime.MaxValue.Year) - From.ToMaximumDateTime(DateTime.MinValue.Year));
        public TimeSpan? MaximumDuration => IsInfinite ? (null as TimeSpan?) : (To.ToMaximumDateTime(DateTime.MaxValue.Year) - From.ToMinimumDateTime(DateTime.MinValue.Year));
        public TimeSpan? AverageDuration => IsInfinite ? (null as TimeSpan?) : TimeSpan.FromTicks((MaximumDuration.Value.Ticks - MinimumDuration.Value.Ticks) / 2);

        public TimeSpan? AbsoluteMinimumDuration => IsInfinite ? (null as TimeSpan?) : (MinimumDuration.Value >= TimeSpan.Zero ? MinimumDuration.Value : -MinimumDuration.Value);
        public TimeSpan? AbsoluteMaximumDuration => IsInfinite ? (null as TimeSpan?) : (MaximumDuration.Value >= TimeSpan.Zero ? MaximumDuration.Value : -MaximumDuration.Value);
        public TimeSpan? AbsoluteAverageDuration => IsInfinite ? (null as TimeSpan?) : (AverageDuration.Value >= TimeSpan.Zero ? AverageDuration.Value : -AverageDuration.Value);


        public bool IsSinceForever => (From is null) || From.IsNever();
        public bool IsUntilForever => (To is null) || To.IsNever();
        public bool IsInfinite => IsSinceForever || IsUntilForever;
        public bool IsTimeless => IsSinceForever && IsUntilForever;


        public bool IsPrecise() => !IsTimeless && From.IsPrecise() && To.IsPrecise();


        public PeriodOfTime ToMinimumPeriodOfTime(int? fallbackYear = null) => new PeriodOfTime { From = From.ToMaximumDateTime(fallbackYear), To = To.ToMinimumDateTime(fallbackYear) };
        public PeriodOfTime ToMaximumPeriodOfTime(int? fallbackYear = null) => new PeriodOfTime { From = From.ToMinimumDateTime(fallbackYear), To = To.ToMaximumDateTime(fallbackYear) };
    }
}
