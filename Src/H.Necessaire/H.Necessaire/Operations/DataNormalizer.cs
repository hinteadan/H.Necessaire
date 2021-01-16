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
            double locationPercent =
                value <= From.Min
                ? 0
                : value >= From.Max
                ? 1
                : (value - From.Min) / (From.Max - From.Min)
                ;

            double targetValue = locationPercent * (To.Max - To.Min) + To.Min;

            return targetValue;
        }
    }
}
