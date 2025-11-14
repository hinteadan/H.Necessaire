using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire
{
    public class HMeasurement : EphemeralTypeBase, IStringIdentity
    {
        static readonly TimeSpan defaultValidity = TimeSpan.FromHours(1);

        readonly Dictionary<string, HCounter> counters = new Dictionary<string, HCounter>();
        readonly Dictionary<string, HTimeSeries> timeSeries = new Dictionary<string, HTimeSeries>();

        public HMeasurement() => ExpireIn(defaultValidity);

        public string ID { get; set; }
        public string Tag { get; set; }
        public string DisplayLabel { get; set; }

        public PeriodOfTime Period { get; set; }


        public IDictionary<string, HCounter> Counters() => counters;
        public HCounter[] AllCounters() => counters.Values.ToArray();
        public IDictionary<string, HTimeSeries> TimeSeries() => timeSeries;
        public HTimeSeries[] AllTimeSeries() => timeSeries.Values.ToArray();


        public Note[] Notes { get; set; }
    }
}
