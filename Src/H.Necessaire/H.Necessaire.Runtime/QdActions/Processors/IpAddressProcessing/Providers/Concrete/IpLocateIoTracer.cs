using H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Abstract;
using System;

namespace H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Concrete
{
    internal class IpLocateIoTracer : IpTracerBase<IpLocateIoTracer.Model>
    {
        //Fully Free, no api KEY, 1K req/day
        //Does not contain network cluster info, just geo location, and just Country

        public override InternalIdentity About { get; } = new InternalIdentity
        {
            ID = Guid.Parse("76B24C5D-034D-41BF-8948-D26DC14C4F6C"),
            DisplayName = "IPLocate",
            IDTag = nameof(IpLocateIoTracer),
            Notes = new Note[] {
                "https://www.iplocate.io".NoteAs("Website"),
                "https://iplocate.docs.apiary.io/#".NoteAs("DetailsUrl"),
            },
        };

        protected override bool IsProviderModelSuccessful(Model model, out string message)
        {
            message = null;
            return true; //On error returns 400 HTTP Status which is caught in the upper layer
        }

        protected override string UrlProvider(string networkAddress)
        {
            return $"https://www.iplocate.io/api/lookup/{networkAddress}";
        }

        protected override NetworkTrace Map(Model model)
        {
            return
                new NetworkTrace
                {
                    ClusterName = $"{model.asn} - {model.org}",
                    ClusterNumber = model.asn,
                    GeoLocation = new GeoLocation
                    {
                        Address = new GeoAddress
                        {
                            Continent = model.continent,
                            Country = model.country.ToGeoAddressArea(model.country_code),
                            County = model.subdivision,
                            City = model.city,
                            CityArea = model.subdivision2,
                            ZipCode = model.postal_code,
                        },
                        GpsPosition = new GpsPoint
                        {
                            LatInDegrees = model.latitude,
                            LngInDegrees = model.longitude,
                        },
                        TimeZones = string.Empty.NoteAs(model.time_zone).AsArray(),
                    },
                };
        }

        public class Model
        {
            public string ip { get; set; }
            public string country { get; set; }
            public string country_code { get; set; }
            public string city { get; set; }
            public string continent { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string time_zone { get; set; }
            public string postal_code { get; set; }
            public string org { get; set; }
            public string asn { get; set; }
            public string subdivision { get; set; }
            public string subdivision2 { get; set; }

            /*
             
             {
                "ip": "89.137.112.253",
                "country": "Romania",
                "country_code": "RO",
                "city": "Cluj-Napoca",
                "continent": "Europe",
                "latitude": 46.7667,
                "longitude": 23.6,
                "time_zone": "Europe/Bucharest",
                "postal_code": "400001",
                "org": "Liberty Global B.V.",
                "asn": "AS6830",
                "subdivision": "Cluj",
                "subdivision2": null
            }
             
             */
        }
    }
}
