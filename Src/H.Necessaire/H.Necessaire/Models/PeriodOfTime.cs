using System;

namespace H.Necessaire
{
    public class PeriodOfTime
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public TimeSpan Duration => To - From;
        public TimeSpan AbsoluteDuration => Duration >= TimeSpan.Zero ? Duration : -Duration;
    }
}
