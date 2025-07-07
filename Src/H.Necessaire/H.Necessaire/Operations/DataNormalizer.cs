using System;

namespace H.Necessaire
{
    public class DataNormalizer
    {
        public readonly NumberInterval From;
        public readonly NumberInterval To;
        public DataNormalizer(NumberInterval fromInterval, NumberInterval toInterval)
        {
            this.From = fromInterval;
            this.To = toInterval;
        }

        public double Do(double value)
        {
            if (From.IsInfinite)
                throw new InvalidOperationException("From interval is infinite");

            if (To.IsInfinite)
                throw new InvalidOperationException("To interval is infinite");

            if (From.IsSingle)
                return From.Min.Value;

            if (To.IsSingle)
                return To.Min.Value;

            double locationPercent =
                value <= From.Min.Value
                ? 0
                : value >= From.Max.Value
                ? 1
                : (value - From.Min.Value) / (From.Max.Value - From.Min.Value);

            double targetValue = (locationPercent * (To.Max.Value - To.Min.Value)) + To.Min.Value;

            return targetValue;
        }

        public DataNormalizer Reverse() => new DataNormalizer(To, From);

        public static DataNormalizer Percent(NumberInterval fromInterval) => new DataNormalizer(fromInterval, NumberInterval.Percent);
    }
}
