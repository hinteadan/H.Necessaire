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

        public GpsPoint Center() => Pins.IsEmpty() ? (0, 0) : (((NorthBoundary() - SouthBoundary()) / 2) + SouthBoundary(), ((WestBoundary() - EastBoundary()) / 2) + EastBoundary());

        public GpsPoint[] NorthMost() => Pins.Where(x => x.LatInDegrees == NorthBoundary()).ToArray();
        public GpsPoint[] SouthMost() => Pins.Where(x => x.LatInDegrees == SouthBoundary()).ToArray();
        public GpsPoint[] EastMost() => Pins.Where(x => x.LngInDegrees == EastBoundary()).ToArray();
        public GpsPoint[] WestMost() => Pins.Where(x => x.LngInDegrees == WestBoundary()).ToArray();

        /**
         As of https://wrfranklin.org/Research/Short_Notes/pnpoly.html

         int pnpoly(int nvert, float *vertx, float *verty, float testx, float testy)
            {
              int i, j, c = 0;
              for (i = 0, j = nvert-1; i < nvert; j = i++) {
                if ( ((verty[i]>testy) != (verty[j]>testy)) &&
	             (testx < (vertx[j]-vertx[i]) * (testy-verty[i]) / (verty[j]-verty[i]) + vertx[i]) )
                   c = !c;
              }
              return c;
            }
Argument	Meaning
nvert 	Number of vertices in the polygon. Whether to repeat the first vertex at the end is discussed below.
vertx, verty 	Arrays containing the x- and y-coordinates of the polygon's vertices.
testx, testy	X- and y-coordinate of the test point. 
         */
        public bool Contains(GpsPoint location)
        {
            if (Pins.IsEmpty())
                return false;

            bool result = false;

            for (int i = 0, j = Pins.Length - 1; i < Pins.Length; j = i++)
            {
                if (
                    ((Pins[i].LatInDegrees > location.LatInDegrees) != (Pins[j].LatInDegrees > location.LatInDegrees))
                    &&
                    (location.LngInDegrees < (Pins[j].LngInDegrees - Pins[i].LngInDegrees) * (location.LatInDegrees - Pins[i].LatInDegrees) / (Pins[j].LatInDegrees - Pins[i].LatInDegrees) + Pins[i].LngInDegrees)
                )
                    result = !result;
            }

            return result;
        }

        public override string ToString()
        {
            return $"{Pins?.Length ?? 0} point(s)";
        }

        public static implicit operator GpsArea(GpsPoint[] gpsPoints) => new GpsArea(gpsPoints);
        public static implicit operator GpsArea(TaggedValue<GpsPoint>[] gpsPoints) => new GpsArea(gpsPoints?.Select(x => x.Value).ToArray());
    }
}
