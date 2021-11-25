using System.Linq;

namespace H.Necessaire
{
    public struct GeoAddressArea
    {
        const string separator = "-";

        public string Name { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return
                string.Join(
                    separator,
                    new string[] {
                        Code,
                        Name,
                    }
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                );
        }

        public static implicit operator GeoAddressArea(string value) => new GeoAddressArea { Name = value, Code = null };

    }
}
