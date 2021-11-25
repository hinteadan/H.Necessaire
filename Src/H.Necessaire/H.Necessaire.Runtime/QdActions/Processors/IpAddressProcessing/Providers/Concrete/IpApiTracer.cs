using H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Abstract;
using System;

namespace H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Concrete
{
    internal class IpApiTracer : IpTracerBase<IpApiTracer.Model>
    {
        public override InternalIdentity About { get; } = new InternalIdentity
        {
            ID = Guid.Parse("B264DE78-3CE7-4569-86F5-08F482D8B4B9"),
            DisplayName = "ip-api",
            IDTag = nameof(IpApiTracer),
            Notes = new Note[] {
                "https://ip-api.com".NoteAs("Website"),
                "https://ip-api.com/docs".NoteAs("DetailsUrl"),
            },
        };

        protected override string UrlProvider(string networkAddress)
        {
            return $"http://ip-api.com/json/{networkAddress}?fields=status,message,continent,continentCode,country,countryCode,region,regionName,city,district,zip,lat,lon,timezone,currency,isp,org,as,asname,reverse,mobile,proxy,query";
        }

        protected override bool IsProviderModelSuccessful(Model model, out string message)
        {
            message = $"Response Status {model.status}";
            return model.status.ToLowerInvariant().Trim() == "success";
        }

        protected override NetworkTrace Map(Model model)
        {
            return new NetworkTrace
            {
                ClusterName = model.asname,
                ClusterNumber = model.@as,
                NetworkServiceProvider = model.isp,
                Organization = model.org,
                GeoLocation = new GeoLocation
                {
                    Address = new GeoAddress
                    {
                        Continent = model.continent.ToGeoAddressArea(model.continentCode),
                        Country = model.country.ToGeoAddressArea(model.countryCode),
                        County = model.regionName.ToGeoAddressArea(model.region),
                        City = model.city,
                        CityArea = model.district,
                        ZipCode = model.zip,
                    },
                    GpsPosition = new GpsPoint
                    {
                        LatInDegrees = model.lat,
                        LngInDegrees = model.lon,
                    },
                    TimeZones = string.Empty.NoteAs(model.timezone).AsArray(),
                    Currencies = model.currency.NoteAs(model.currency).AsArray(),
                },
            };
        }

        public class Model
        {
            public string status { get; set; }
            public string continent { get; set; }
            public string continentCode { get; set; }
            public string country { get; set; }
            public string countryCode { get; set; }
            public string region { get; set; }
            public string regionName { get; set; }
            public string city { get; set; }
            public string district { get; set; }
            public string zip { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
            public string timezone { get; set; }
            public string currency { get; set; }
            public string isp { get; set; }
            public string org { get; set; }
            public string @as { get; set; }
            public string asname { get; set; }
            public string reverse { get; set; }
            public string mobile { get; set; }
            public string proxy { get; set; }
            public string query { get; set; }

            /*

            {
                "status": "success",
                "continent": "Europe",
                "continentCode": "EU",
                "country": "Romania",
                "countryCode": "RO",
                "region": "CJ",
                "regionName": "Cluj",
                "city": "Cluj-Napoca",
                "district": "",
                "zip": "400001",
                "lat": 46.769,
                "lon": 23.5978,
                "timezone": "Europe/Bucharest",
                "currency": "RON",
                "isp": "UPC Romania",
                "org": "",
                "as": "AS6830 Liberty Global B.V.",
                "asname": "LGI-UPC",
                "reverse": "",
                "mobile": false,
                "proxy": false,
                "query": "89.137.112.253"
            }

             */
        }
    }
}
