using System.Text;

namespace H.Necessaire.Analytics
{
    public class NetworkTraceGeoLocation
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string CityName { get; set; }

        public override string ToString()
        {
            StringBuilder printer = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(CityName))
                printer
                    .Append(CityName)
                    .Append((!string.IsNullOrWhiteSpace(CountryName) || !string.IsNullOrWhiteSpace(CountryCode)) ? ", " : string.Empty)
                    ;

            if (!string.IsNullOrWhiteSpace(CountryName))
                printer
                    .Append(CountryName)
                    ;

            if (!string.IsNullOrWhiteSpace(CountryCode))
                printer
                    .Append(string.IsNullOrWhiteSpace(CountryName) ? string.Empty : " - ")
                    .Append(CountryCode);

            return printer.ToString();
        }
    }
}
