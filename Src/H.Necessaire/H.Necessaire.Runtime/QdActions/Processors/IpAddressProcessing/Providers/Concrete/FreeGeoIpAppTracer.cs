using H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Abstract;
using System;

namespace H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Concrete
{
    internal class FreeGeoIpAppTracer : IpTracerBase<FreeGeoIpAppTracer.Model>
    {
        public override InternalIdentity About { get; } = new InternalIdentity
        {
            ID = Guid.Parse("30CE891A-2F72-4556-959E-82EFF8110055"),
            DisplayName = "Free IP Geolocation API",
            IDTag = nameof(FreeGeoIpAppTracer),
            Notes = new Note[] {
                "https://freegeoip.app".NoteAs("Website"),
                "https://freegeoip.app".NoteAs("DetailsUrl"),
            },
        };

        protected override string UrlProvider(string networkAddress)
        {
            return $"https://freegeoip.app/json/{networkAddress}";
        }

        protected override bool IsProviderModelSuccessful(Model model, out string message)
        {
            message = null;
            return true; //On error returns 404 HTTP Status which is caught in the upper layer
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
                            Country = model.country_name.ToGeoAddressArea(model.country_code),
                            County = model.region_name.ToGeoAddressArea(model.region_code),
                            Notes = model.metro_code.ToString().NoteAs("MetroCode").AsArray(),
                            ZipCode = model.zip_code,
                        },
                        GpsPosition = new GpsPoint
                        {
                            LatInDegrees = model.latitude,
                            LngInDegrees = model.longitude,
                        },
                        TimeZones = string.Empty.NoteAs(model.time_zone).AsArray(),
                    }
                };
        }

        public class Model
        {

            public string ip { get; set; }
            public string country_code { get; set; }
            public string country_name { get; set; }
            public string region_code { get; set; }
            public string region_name { get; set; }
            public string city { get; set; }
            public string zip_code { get; set; }
            public string time_zone { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public int metro_code { get; set; }

            /*
             
            {
                "ip": "89.137.112.253",
                "country_code": "RO",
                "country_name": "Romania",
                "region_code": "CJ",
                "region_name": "Cluj",
                "city": "Cluj-Napoca",
                "zip_code": "400001",
                "time_zone": "Europe/Bucharest",
                "latitude": 46.769,
                "longitude": 23.5978,
                "metro_code": 0
            }
             
             */
        }
    }
}
