using System;
using System.Globalization;

namespace H.Necessaire
{
    public struct NumberInterval
    {
        public static readonly NumberInterval Percent = new NumberInterval(0, 100);

        public NumberInterval(double? min, double? max, bool isMinIncluded = true, bool isMaxIncluded = true)
        {
            Min = (min != null && max != null) ? Math.Min(min.Value, max.Value) : min;
            IsMinIncluded = Min == null ? false : isMinIncluded;
            Max = (min != null && max != null) ? Math.Max(min.Value, max.Value) : max;
            IsMaxIncluded = Max == null ? false : isMaxIncluded;
        }

        public double? Min { get; set; }
        public double? Max { get; set; }

        public bool IsMinIncluded { get; set; }
        public bool IsMaxIncluded { get; set; }

        public bool IsPositivelyInfinite => Max == null;
        public bool IsNegativelyInfinite => Min == null;
        public bool IsInfinite => IsPositivelyInfinite || IsNegativelyInfinite;
        public bool IsSingle => Min != null && Max != null && Min == Max;

        public bool Contains(double value)
        {
            if (IsNegativelyInfinite && IsPositivelyInfinite)
                return true;

            if (IsNegativelyInfinite)
                return IsMaxIncluded ? value <= Max : value < Max;

            if (IsPositivelyInfinite)
                return IsMinIncluded ? value >= Min : value > Min;

            return
                (IsMinIncluded ? value >= Min : value > Min)
                &&
                (IsMaxIncluded ? value <= Max : value < Max)
                ;
        }

        public override string ToString()
        {
            return $"{(IsMinIncluded ? "[" : "(")} {Min?.ToString(CultureInfo.InvariantCulture) ?? "-∞"} ; {Max?.ToString(CultureInfo.InvariantCulture) ?? "∞"} {(IsMaxIncluded ? "]" : ")")}";
        }

        public static implicit operator NumberInterval((double? min, double? max) parts)
            => new NumberInterval(parts.min, parts.max);
    }
}
