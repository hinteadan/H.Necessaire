using System;

namespace H.Necessaire
{
    public static class GeoDataExtensions
    {
        public static GeoAddressArea ToGeoAddressArea(this string value, string code)
        {
            return new GeoAddressArea { Name = value, Code = code };
        }

        public static void ToDMS(this double degrees, out int deg, out int min, out double sec)
        {
            if (degrees == 0)
            {
                deg = 0;
                min = 0;
                sec = 0;
                return;
            }

            degrees = Math.Abs(degrees);

            int wholeDegrees = (int)degrees;
            double remainingDegress = degrees - wholeDegrees;
            double minutes = remainingDegress * 60;
            int wholeMinutes = (int)minutes;
            double seconds = minutes * 60;
            seconds = Math.Round(seconds, 4);

            deg = wholeDegrees;
            min = wholeMinutes;
            sec = seconds;
        }

        public static double ToDegrees(int deg, int min, double sec)
        {
            return deg + min / 60d + sec / 3600d; 
        }
    }
}
