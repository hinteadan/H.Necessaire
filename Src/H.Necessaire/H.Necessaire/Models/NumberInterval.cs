using System;
using System.Globalization;

namespace H.Necessaire
{
    public struct NumberInterval
    {
        public NumberInterval(double min, double max)
        {
            if (min > max)
                throw new InvalidOperationException("min > max");

            Min = min;
            Max = max;
        }

        public double Min { get; set; }
        public double Max { get; set; }

        public override string ToString()
        {
            return $"[{Min.ToString(CultureInfo.InvariantCulture)},{Max.ToString(CultureInfo.InvariantCulture)}]";
        }
    }
}
