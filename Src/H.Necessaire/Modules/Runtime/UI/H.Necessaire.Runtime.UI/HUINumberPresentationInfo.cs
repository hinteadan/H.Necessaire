namespace H.Necessaire.Runtime.UI
{
    public class HUINumberPresentationInfo
    {
        public int NumberOfDecimals { get; set; } = 0;
        public decimal IncrementUnit { get; set; } = 1;
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}