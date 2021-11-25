namespace H.Necessaire
{
    public static class GeoDataExtensions
    {
        public static GeoAddressArea ToGeoAddressArea(this string value, string code)
        {
            return new GeoAddressArea { Name = value, Code = code };
        }
    }
}
