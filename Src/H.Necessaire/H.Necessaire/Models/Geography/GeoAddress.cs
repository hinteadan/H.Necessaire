using System.Linq;

namespace H.Necessaire
{
    public class GeoAddress
    {
        const string separator = "; ";

        public GeoAddressArea? Continent { get; set; }

        public GeoAddressArea? Country { get; set; }

        public GeoAddressArea? State { get; set; }

        public GeoAddressArea? County { get; set; }

        public GeoAddressArea? City { get; set; }

        public GeoAddressArea? CityArea { get; set; }

        public string ZipCode { get; set; }

        public string StreetAddress { get; set; }

        public Note[] Notes { get; set; }

        public override string ToString()
        {
            return
                string.Join(
                    separator,
                    new string[] {
                        StreetAddress,
                        ZipCode,
                        City?.ToString(),
                        County?.ToString(),
                        State?.ToString(),
                        Country?.ToString(),
                    }
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                );
        }
    }
}
