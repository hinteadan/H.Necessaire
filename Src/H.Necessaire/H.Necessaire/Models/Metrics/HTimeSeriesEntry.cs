using System;

namespace H.Necessaire
{
    public class HTimeSeriesEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public double Value { get; set; }
        public double[] AddonValues { get; set; }
        public bool IsMultiValue => !AddonValues.IsEmpty();
        public double[] AllValues() => !IsMultiValue ? Value.AsArray() : Value.AsArray().Push(AddonValues, checkDistinct: false);
    }
}
