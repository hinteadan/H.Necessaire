using System;

namespace H.Necessaire
{
    public static class GeoDataExtensions
    {
        public static GeoAddressArea ToGeoAddressArea(this string value, string code)
        {
            return new GeoAddressArea { Name = value, Code = code };
        }

        public static (int deg, int min, double sec) ToDMS(this double degrees)
        {
            if (degrees == 0)
                return (0, 0, 0);

            degrees = Math.Abs(degrees);

            int wholeDegrees = (int)degrees;
            double remainingDegress = degrees - wholeDegrees;
            double minutes = remainingDegress * 60;
            int wholeMinutes = (int)minutes;
            double seconds = minutes * 60;
            seconds = Math.Round(seconds, 4);

            return (wholeDegrees, wholeMinutes, seconds);
        }
    }
}
