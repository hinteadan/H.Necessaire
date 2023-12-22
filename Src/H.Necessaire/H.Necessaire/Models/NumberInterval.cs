using System;
using System.Globalization;

namespace H.Necessaire
{
    public struct NumberInterval
    {
        public static readonly NumberInterval Percent = new NumberInterval(0, 100);

        public NumberInterval(decimal? min, decimal? max, bool isMinIncluded = true, bool isMaxIncluded = true)
        {
            if (min != null && max != null && min > max)
                throw new InvalidOperationException("min > max");

            Min = min;
            IsMinIncluded = min == null ? false : isMinIncluded;
            Max = max;
            IsMaxIncluded = max == null ? false : isMaxIncluded;
        }

        public decimal? Min { get; set; }
        public decimal? Max { get; set; }

        public bool IsMinIncluded { get; set; }
        public bool IsMaxIncluded { get; set; }

        public bool IsPositivelyInfinite => Max == null;
        public bool IsNegativelyInfinite => Min == null;
        public bool IsInfinite => IsPositivelyInfinite || IsNegativelyInfinite;
        public bool IsSingle => Min != null && Max != null && Min == Max;

        public override string ToString()
        {
            return $"{(IsMinIncluded ? "[" : "(")} {Min?.ToString(CultureInfo.InvariantCulture) ?? "-∞"} ; {Max?.ToString(CultureInfo.InvariantCulture) ?? "∞"} {(IsMaxIncluded ? "]" : ")")}";
        }
    }
}
