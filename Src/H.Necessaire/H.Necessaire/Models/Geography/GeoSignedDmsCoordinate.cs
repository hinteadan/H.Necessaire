namespace H.Necessaire
{
    public struct GeoSignedDmsCoordinate
    {
        public double Degrees { get; set; }
        public double Minutes { get; set; }
        public double Seconds { get; set; }

        public static implicit operator GeoSignedDmsCoordinate((double d, double m, double s) parts)
            => new GeoSignedDmsCoordinate { Degrees = parts.d, Minutes = parts.m, Seconds = parts.s };
    }
}
