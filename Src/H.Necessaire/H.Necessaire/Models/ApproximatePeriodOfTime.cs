using System;

namespace H.Necessaire
{
    public class ApproximatePeriodOfTime
    {
        public PartialPeriodOfTime StartPeriod { get; set; }
        public PartialPeriodOfTime EndPeriod { get; set; }

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


        public bool IsSinceForever => (StartPeriod is null) || StartPeriod.IsTimeless;
        public bool IsUntilForever => (EndPeriod is null) || EndPeriod.IsTimeless;
        public bool IsInfinite => IsSinceForever || IsUntilForever;
        public bool IsTimeless => IsSinceForever && IsUntilForever;


        public bool IsPrecise() => !IsTimeless && StartPeriod.IsPrecise() && EndPeriod.IsPrecise();
    }
}
