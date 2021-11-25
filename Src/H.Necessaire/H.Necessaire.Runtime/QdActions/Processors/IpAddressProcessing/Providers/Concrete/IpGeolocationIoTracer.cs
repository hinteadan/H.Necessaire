using H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Abstract;
using System;
using System.Globalization;

namespace H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Concrete
{
    internal class IpGeolocationIoTracer : IpTracerBase<IpGeolocationIoTracer.Model>
    {
        //1 req. at 87 seconds to not excede the daily quota
        //30k req. / month
        //1k req. / day

        const string apiKey = "5b845dc25212495595d5153fd65988de";

        public override InternalIdentity About { get; } = new InternalIdentity
        {
            ID = Guid.Parse("7F45D854-C7CA-4F8B-9920-8A2CA6EB55D3"),
            DisplayName = "IpGeolocation.io",
            IDTag = nameof(IpGeolocationIoTracer),
            Notes = new Note[] {
                "https://ipgeolocation.io".NoteAs("Website"),
                "https://ipgeolocation.io/documentation.html".NoteAs("DetailsUrl"),
            },
        };

        protected override TimeSpan RequiredCooldown { get; } = TimeSpan.FromSeconds(5);

        protected override string UrlProvider(string networkAddress)
        {
            return $"https://api.ipgeolocation.io/ipgeo?apiKey={apiKey}&ip={networkAddress}";
        }

        protected override bool IsProviderModelSuccessful(Model model, out string message)
        {
            message = model.message;
            return !string.IsNullOrWhiteSpace(model.ip);
        }

        protected override NetworkTrace Map(Model model)
        {
            return
                new NetworkTrace
                {
                    ClusterName = model.organization,
                    ClusterNumber = model.asn,
                    Organization = model.organization,
                    NetworkServiceProvider = model.isp,
                    GeoLocation = new GeoLocation
                    {
                        Address = new GeoAddress
                        {
                            City = model.city,
                            Continent = model.continent_name.ToGeoAddressArea(model.continent_code),
                            Country = model.country_name.ToGeoAddressArea(model.country_code2),
                            County = model.state_prov,
                            CityArea = model.district,
                            ZipCode = model.zipcode,
                            Notes = new Note[]{
                                model.country_code3.NoteAs("CountryCode3"),
                                model.country_capital.NoteAs("CountryCapital"),
                                model.is_eu.ToString().NoteAs("IsEU"),
                                model.country_tld.ToString().NoteAs("CountryTLD"),
                                model.country_flag.ToString().NoteAs("CountryFlag"),
                                model.geoname_id.ToString().NoteAs("GeoNameID"),
                                model.connection_type.ToString().NoteAs("ConnectionType"),
                            }
                        },
                        Currencies = model.currency?.code?.NoteAs($"{model.currency?.name} ({model.currency?.symbol})").AsArray(),
                        DialCodes = model.calling_code.NoteAs(model.calling_code).AsArray(),
                        GpsPosition = new GpsPoint
                        {
                            LatInDegrees = double.Parse(model.latitude, CultureInfo.InvariantCulture),
                            LngInDegrees = double.Parse(model.longitude, CultureInfo.InvariantCulture),
                        },
                        Languages = model.languages.NoteAs(model.languages).AsArray(),
                        TimeZones = (-(int)TimeSpan.FromHours(model.time_zone?.offset ?? 0).TotalMinutes).ToString().NoteAs(model.time_zone?.name).AsArray(),
                    },
                };
        }

        public class Model
        {
            public class Currency
            {
                public string code { get; set; }
                public string name { get; set; }
                public string symbol { get; set; }
            }

            public class TimeZone
            {
                public string name { get; set; }
                public double offset { get; set; }
                public string current_time { get; set; }
                public decimal current_time_unix { get; set; }
                public bool is_dst { get; set; }
                public int dst_savings { get; set; }
            }

            public string message { get; set; } = null;
            public string ip { get; set; }
            public string continent_code { get; set; }
            public string continent_name { get; set; }
            public string country_code2 { get; set; }
            public string country_code3 { get; set; }
            public string country_name { get; set; }
            public string country_capital { get; set; }
            public string state_prov { get; set; }
            public string district { get; set; }
            public string city { get; set; }
            public string zipcode { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public bool is_eu { get; set; }
            public string calling_code { get; set; }
            public string country_tld { get; set; }
            public string languages { get; set; }
            public string country_flag { get; set; }
            public string geoname_id { get; set; }
            public string isp { get; set; }
            public string connection_type { get; set; }
            public string organization { get; set; }
            public string asn { get; set; }
            public Currency currency { get; set; }
            public TimeZone time_zone { get; set; }

            /*
             
             {
                "ip": "89.137.112.253",
                "continent_code": "EU",
                "continent_name": "Europe",
                "country_code2": "RO",
                "country_code3": "ROU",
                "country_name": "Romania",
                "country_capital": "Bucharest",
                "state_prov": "Cluj",
                "district": "Cluj-Napoca",
                "city": "Cluj-Napoca",
                "zipcode": "400157",
                "latitude": "46.77720",
                "longitude": "23.59990",
                "is_eu": true,
                "calling_code": "+40",
                "country_tld": ".ro",
                "languages": "ro,hu,rom",
                "country_flag": "https://ipgeolocation.io/static/flags/ro_64.png",
                "geoname_id": "681290",
                "isp": "UPC Romania",
                "connection_type": "",
                "organization": "Liberty Global B.V.",
                "currency": {
                    "code": "RON",
                    "name": "Romanian Leu",
                    "symbol": "lei"
                },
                "time_zone": {
                    "name": "Europe/Bucharest",
                    "offset": 2,
                    "current_time": "2019-11-26 23:43:26.530+0200",
                    "current_time_unix": 1574804606.53,
                    "is_dst": false,
                    "dst_savings": 1
                }
            }
             
             */
        }
    }
}
