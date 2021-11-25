using System.Globalization;

namespace H.Necessaire
{
    public struct GpsPoint
    {
        const string separator = ",";

        public double LatInDegrees { get; set; }

        public double LngInDegrees { get; set; }

        public double? AltFromSeaLevelInMeters { get; set; }

        public override string ToString()
        {
            return
                string.Join(
                    separator,
                    LatInDegrees.ToString(CultureInfo.InvariantCulture),
                    LngInDegrees.ToString(CultureInfo.InvariantCulture)
                );
        }
    }
}
