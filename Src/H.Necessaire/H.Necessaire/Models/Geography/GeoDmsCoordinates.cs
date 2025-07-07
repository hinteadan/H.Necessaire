namespace H.Necessaire
{
    public struct GeoDmsCoordinates
    {
        const string separator = " ";

        public GeoDmsLatCoordinate Lat { get; set; }
        public GeoDmsLngCoordinate Lng { get; set; }

        public override string ToString()
            => string.Join(
                    separator,
                    Lat,
                    Lng
                );

        public static implicit operator GeoDmsCoordinates((GeoDmsLatCoordinate lat, GeoDmsLngCoordinate lng) parts)
            => new GeoDmsCoordinates { Lat = parts.lat, Lng = parts.lng };
        public static implicit operator GeoDmsCoordinates(GpsPoint gpsPoint)
            => new GeoDmsCoordinates { Lat = gpsPoint, Lng = gpsPoint };
    }
}
