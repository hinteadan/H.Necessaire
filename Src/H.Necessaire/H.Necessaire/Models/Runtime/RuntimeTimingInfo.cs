using System;

namespace H.Necessaire
{
    public class RuntimeTimingInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public PeriodOfTime Connect { get; set; }
        public PeriodOfTime DomainLookup { get; set; }
        public PeriodOfTime DOM { get; set; }
        public PeriodOfTime Redirect { get; set; }
        public PeriodOfTime RequestResponse { get; set; }
        public PeriodOfTime Response { get; set; }

    }
}
