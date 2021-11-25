using H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Abstract;
using System;
using System.Linq;

namespace H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Concrete
{
    internal class IpFindComTracer : IpTracerBase<IpFindComTracer.Model>
    {
        const string apiKey = "2264dd7c-f41b-4054-a45c-8cc7b495884a";

        public override InternalIdentity About { get; } = new InternalIdentity
        {
            ID = Guid.Parse("583011CA-C15B-47CA-9795-74BAE472A48C"),
            DisplayName = "IP Find",
            IDTag = nameof(IpFindComTracer),
            Notes = new Note[] {
                "https://ipfind.com".NoteAs("Website"),
                "https://ipfind.com/docs".NoteAs("DetailsUrl"),
            },
        };

        protected override TimeSpan RequiredCooldown { get; } = TimeSpan.FromSeconds(5);

        protected override string UrlProvider(string networkAddress)
        {
            return $"https://api.ipfind.com?ip={networkAddress}&auth={apiKey}";
        }

        protected override bool IsProviderModelSuccessful(Model model, out string message)
        {
            message = null;
            return true; //On error returns 400 HTTP Status which is caught in the upper layer
        }

        protected override NetworkTrace Map(Model model)
        {
            return
                new NetworkTrace
                {
                    GeoLocation = new GeoLocation
                    {
                        Address = new GeoAddress
                        {
                            City = model.city,
                            Continent = model.continent.ToGeoAddressArea(model.continent_code),
                            Country = model.country.ToGeoAddressArea(model.country_code),
                            County = model.county,
                            CityArea = model.region.ToGeoAddressArea(model.region_code),
                            Notes = model.owner.NoteAs("Owner").AsArray(),
                        },
                        Currencies = model.currency.NoteAs(model.currency).AsArray(),
                        GpsPosition = new GpsPoint
                        {
                            LatInDegrees = model.latitude,
                            LngInDegrees = model.longitude,
                        },
                        Languages = model.languages?.Select(l => l.NoteAs(l)).ToArray(),
                        TimeZones = string.Empty.NoteAs(model.timezone).AsArray(),
                    }
                };
        }

        public class Model
        {

            public string ip_address { get; set; }
            public string country { get; set; }
            public string country_code { get; set; }
            public string continent { get; set; }
            public string continent_code { get; set; }
            public string city { get; set; }
            public string county { get; set; }
            public string region { get; set; }
            public string region_code { get; set; }
            public string timezone { get; set; }
            public string owner { get; set; }
            public double longitude { get; set; }
            public double latitude { get; set; }
            public string currency { get; set; }
            public string[] languages { get; set; }

            /*
             
            {
                "ip_address": "89.137.112.253",
                "country": "Romania",
                "country_code": "RO",
                "continent": "Europe",
                "continent_code": "EU",
                "city": "Cluj-Napoca",
                "county": "Municipiul Cluj-Napoca",
                "region": "Cluj",
                "region_code": "13",
                "timezone": "Europe/Bucharest",
                "owner": null,
                "longitude": 23.5978,
                "latitude": 46.769,
                "currency": "RON",
                "languages": [
                    "ro",
                    "hu",
                    "rom"
                ]
            }
             
             */

        }
    }
}
