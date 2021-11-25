using H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Abstract;
using System;
using System.Globalization;

namespace H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Concrete
{
    internal class IpInfoDbTracer : IpTracerBase<IpInfoDbTracer.Model>
    {
        const string apiKey = "2fab3485c513a6ea8fb5d1821c07bc181201cb08c52ca6e5a5c4146bfecd6240";

        public override InternalIdentity About { get; } = new InternalIdentity
        {
            ID = Guid.Parse("6A50AE5F-7A28-4F1E-957A-C62152488BC7"),
            DisplayName = "IP Info DB",
            IDTag = nameof(IpInfoDbTracer),
            Notes = new Note[] {
                "https://ipinfodb.com".NoteAs("Website"),
                "https://ipinfodb.com/api".NoteAs("DetailsUrl"),
            },
        };

        protected override TimeSpan RequiredCooldown { get; } = TimeSpan.FromSeconds(1);

        protected override string UrlProvider(string networkAddress)
        {
            return $"http://api.ipinfodb.com/v3/ip-city/?key={apiKey}&ip={networkAddress}&format=json";
        }

        protected override bool IsProviderModelSuccessful(Model model, out string message)
        {
            message = $"Response Status {model.statusCode}: {model.statusMessage ?? "<<nomessage>>"}";
            return model.statusCode.ToUpperInvariant().Trim() == "OK";
        }

        protected override NetworkTrace Map(Model model)
        {
            return new NetworkTrace
            {
                GeoLocation = new GeoLocation
                {
                    GpsPosition = new GpsPoint
                    {
                        LatInDegrees = double.Parse(model.latitude, CultureInfo.InvariantCulture),
                        LngInDegrees = double.Parse(model.longitude, CultureInfo.InvariantCulture),
                    },
                    TimeZones = ParseTimeZone(model.timeZone).ToString().NoteAs(model.timeZone).AsArray(),
                    Address = new GeoAddress
                    {
                        Country = model.countryName.ToGeoAddressArea(model.countryCode),
                        County = model.regionName,
                        City = model.cityName,
                        ZipCode = model.zipCode,
                    },
                },
            };
        }

        private int ParseTimeZone(string timeZone)
        {
            int result = 0;

            new Action(() =>
            {
                string[] parts = timeZone.Split(':');
                int hours = 0;
                int.TryParse(parts[0], out hours);
                int minutes = 0;
                int.TryParse(parts[1], out minutes);

                result = -1 * (hours * 60 + minutes);
            }).TryOrFailWithGrace(numberOfTimes: 1);

            return result;
        }

        public class Model
        {
            public string statusCode { get; set; }
            public string statusMessage { get; set; }
            public string ipAddress { get; set; }
            public string countryCode { get; set; }
            public string countryName { get; set; }
            public string regionName { get; set; }
            public string cityName { get; set; }
            public string zipCode { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string timeZone { get; set; }

            /*
             * {
	        "statusCode": "OK",
	        "statusMessage": "",
	        "ipAddress": "89.137.112.253",
	        "countryCode": "RO",
	        "countryName": "Romania",
	        "regionName": "Cluj",
	        "cityName": "Cluj-Napoca",
	        "zipCode": "400930",
	        "latitude": "46.7667",
	        "longitude": "23.6",
	        "timeZone": "+02:00"
            }
             */
        }
    }
}
