using System;

namespace H.Necessaire
{
    public struct GpsLocation
    {
        const string separator = " - ";

        public GpsPoint Point { get; set; }

        public DateTime Timestamp { get; set; }

        public double? GndAccuracyInMeters { get; set; }

        public double? AltAccuracyInMeters { get; set; }

        public double? HeadingInDegrees { get; set; }

        public double? SpeedInMetersPerSecond { get; set; }

        public override string ToString()
        {
            return
                string.Join(
                    separator,
                    Timestamp.ToString(DataPrintingExtensions.ParsableTimeStampFormat),
                    Point.ToString()
                );
        }
    }
}
