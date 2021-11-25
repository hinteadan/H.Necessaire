using System.Linq;

namespace H.Necessaire
{
    public class GpsArea
    {
        public GpsArea()
        {
            Pins = new GpsPoint[0];
        }

        public GpsArea(params GpsPoint[] pins)
        {
            Pins = pins ?? new GpsPoint[0];
        }

        public GpsPoint[] Pins { get; set; } = new GpsPoint[0];

        public double NorthBoundary() => Pins.Max(p => p.LatInDegrees);
        public double SouthBoundary() => Pins.Min(p => p.LatInDegrees);
        public double EastBoundary() => Pins.Max(p => p.LngInDegrees);
        public double WestBoundary() => Pins.Min(p => p.LngInDegrees);

        public GpsPoint NorthWestBoundary() => new GpsPoint { LatInDegrees = NorthBoundary(), LngInDegrees = WestBoundary() };
        public GpsPoint NorthEastBoundary() => new GpsPoint { LatInDegrees = NorthBoundary(), LngInDegrees = EastBoundary() };
        public GpsPoint SouthEastBoundary() => new GpsPoint { LatInDegrees = SouthBoundary(), LngInDegrees = EastBoundary() };
        public GpsPoint SouthWestBoundary() => new GpsPoint { LatInDegrees = SouthBoundary(), LngInDegrees = WestBoundary() };

        public GpsPoint[] NorthMost() => Pins.Where(x => x.LatInDegrees == NorthBoundary()).ToArray();
        public GpsPoint[] SouthMost() => Pins.Where(x => x.LatInDegrees == SouthBoundary()).ToArray();
        public GpsPoint[] EastMost() => Pins.Where(x => x.LngInDegrees == EastBoundary()).ToArray();
        public GpsPoint[] WestMost() => Pins.Where(x => x.LngInDegrees == WestBoundary()).ToArray();

        public override string ToString()
        {
            return $"{Pins?.Length ?? 0} point(s)";
        }
    }
}
