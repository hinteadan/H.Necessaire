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

        public static GpsPoint FromDMS(GeoDmsLatCoordinate lat, GeoDmsLngCoordinate lng, double? altFromSeaLevelInMeters = null)
        {
            return new GpsPoint
            {
                LatInDegrees = lat.ToDegrees(),
                LngInDegrees = lng.ToDegrees(),
                AltFromSeaLevelInMeters = altFromSeaLevelInMeters,
            };
        }
        public static GpsPoint FromDMS(GeoDmsCoordinates dmsCoordinates, double? altFromSeaLevelInMeters = null)
        {
            return new GpsPoint
            {
                LatInDegrees = dmsCoordinates.Lat.ToDegrees(),
                LngInDegrees = dmsCoordinates.Lng.ToDegrees(),
                AltFromSeaLevelInMeters = altFromSeaLevelInMeters,
            };
        }

        public static implicit operator GpsPoint((double lat, double lng, double? altFromSeaLevelInMeters) parts)
            => new GpsPoint
            {
                LatInDegrees = parts.lat,
                LngInDegrees = parts.lng,
                AltFromSeaLevelInMeters = parts.altFromSeaLevelInMeters,
            };
        public static implicit operator GpsPoint((double lat, double lng) parts)
            => new GpsPoint
            {
                LatInDegrees = parts.lat,
                LngInDegrees = parts.lng,
                AltFromSeaLevelInMeters = null,
            };
        public static implicit operator GpsPoint((GeoDmsLatCoordinate lat, GeoDmsLngCoordinate lng, double? altFromSeaLevelInMeters) parts)
            => FromDMS(parts.lat, parts.lng, parts.altFromSeaLevelInMeters);
        public static implicit operator GpsPoint((GeoDmsLatCoordinate lat, GeoDmsLngCoordinate lng) parts)
            => FromDMS(parts.lat, parts.lng);
        public static implicit operator GpsPoint((GeoDmsCoordinates geoDmsCoordinates, double? altFromSeaLevelInMeters) parts)
            => FromDMS(parts.geoDmsCoordinates, parts.altFromSeaLevelInMeters);
        public static implicit operator GpsPoint(GeoDmsCoordinates geoDmsCoordinates)
            => FromDMS(geoDmsCoordinates);
    }
}
