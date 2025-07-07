namespace H.Necessaire.Runtime.MAUI.Extensions
{
    public static class GeoLocationExtensions
    {
        public static GpsPoint ToGpsPoint(this Location mauiLocation)
            => (mauiLocation.Latitude, mauiLocation.Longitude);
    }
}
