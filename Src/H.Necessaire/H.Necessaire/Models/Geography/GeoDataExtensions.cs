using System;

namespace H.Necessaire
{
    public static class GeoDataExtensions
    {
        public static GeoAddressArea ToGeoAddressArea(this string value, string code)
        {
            return new GeoAddressArea { Name = value, Code = code };
        }

        public static void ToDMS(this double degrees, out int deg, out int min, out double sec, bool isUnsigned = false)
        {
            if (degrees == 0)
            {
                deg = 0;
                min = 0;
                sec = 0;
                return;
            }

            bool isPositive = degrees >= 0;

            degrees = Math.Abs(degrees);
            double remainingDegress = degrees;

            int wholeDegrees = (int)degrees;
            remainingDegress -= wholeDegrees;

            double minutes = remainingDegress * 60;
            int wholeMinutes = (int)minutes;
            remainingDegress -= wholeMinutes / 60d;

            double seconds = remainingDegress * 60 * 60;
            seconds = Math.Round(seconds, 4);

            deg = isUnsigned ? wholeDegrees : isPositive ? wholeDegrees : -wholeDegrees;
            min = wholeMinutes;
            sec = seconds;
        }

        public static double ToDegrees(this int deg, int min, double sec)
        {
            bool isPositive = deg >= 0;
            double result = Math.Abs(deg) + Math.Abs(min) / 60d + Math.Abs(sec) / 3600d;
            return isPositive ? result : -result;
        }
    }
}
