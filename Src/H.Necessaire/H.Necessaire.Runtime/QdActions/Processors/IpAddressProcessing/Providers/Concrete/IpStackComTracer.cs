using H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Abstract;
using System;
using System.Linq;

namespace H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Concrete
{
    internal class IpStackComTracer : IpTracerBase<IpStackComTracer.Model>
    {
        const string apiKey = "3765bf184464c33b52d6896a93943c4b";

        public override InternalIdentity About { get; } = new InternalIdentity
        {
            ID = Guid.Parse("312D0C54-57E7-4589-924D-669F04AA7CF2"),
            DisplayName = "IPStack",
            IDTag = nameof(IpStackComTracer),
            Notes = new Note[] {
                "https://ipstack.com".NoteAs("Website"),
                "https://ipstack.com/documentation".NoteAs("DetailsUrl"),
            },
        };

        protected override TimeSpan RequiredCooldown { get; } = TimeSpan.FromSeconds(5);

        protected override string UrlProvider(string networkAddress)
        {
            return $"http://api.ipstack.com/{networkAddress}?access_key={apiKey}";
        }

        protected override bool IsProviderModelSuccessful(Model model, out string message)
        {
            message = $"Response Status {model.error?.code.ToString() ?? "<<n/a>>"}: {model.error?.type?.ToString() ?? "<<noerrortype>>"}, {model.error?.info?.ToString() ?? "<<noerrorinfo>>"}";
            return model.success == false;
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
                            Continent = model.continent_name.ToGeoAddressArea(model.continent_code),
                            Country = model.country_name.ToGeoAddressArea(model.country_code),
                            City = model.city,
                            County = model.region_name.ToGeoAddressArea(model.region_code),
                            ZipCode = model.zip,
                            Notes = new Note[] {
                                model.type.NoteAs("NetworkAddressType"),
                                model.location?.capital?.NoteAs("Capital") ?? new Note(),
                                model.location?.geoname_id?.NoteAs("GeoNameID") ?? new Note(),
                                model.location?.country_flag?.NoteAs("CountryFlag") ?? new Note(),
                                model.location?.country_flag_emoji?.NoteAs("CountryFlagEmoji") ?? new Note(),
                                model.location?.country_flag_emoji_unicode?.NoteAs("CountryFlagEmojiUnicode") ?? new Note(),
                                model.location?.is_eu.ToString().NoteAs("IsEU") ?? new Note(),
                            }
                            .Where(x => !x.IsEmpty())
                            .ToArray(),
                        },
                        DialCodes = model.location?.calling_code != null ? model.location.calling_code.NoteAs(model.location.calling_code).AsArray() : null,
                        GpsPosition = new GpsPoint
                        {
                            LatInDegrees = model.latitude,
                            LngInDegrees = model.longitude,
                        },
                        Languages = model.location?.languages?.Any() ?? false ? model.location.languages.Select(l => $"{l.name} ({l.native})".NoteAs(l.code)).ToArray() : null,
                    }
                };
        }

        public class Model
        {
            public bool? success { get; set; }
            public Error error { get; set; }

            public string ip { get; set; }
            public string type { get; set; }
            public string continent_code { get; set; }
            public string continent_name { get; set; }
            public string country_code { get; set; }
            public string country_name { get; set; }
            public string region_code { get; set; }
            public string region_name { get; set; }
            public string city { get; set; }
            public string zip { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public Location location { get; set; }
            public class Location
            {
                public string geoname_id { get; set; }
                public string capital { get; set; }
                public Language[] languages { get; set; }
                public string country_flag { get; set; }
                public string country_flag_emoji { get; set; }
                public string country_flag_emoji_unicode { get; set; }
                public string calling_code { get; set; }
                public bool is_eu { get; set; }
                public class Language
                {
                    public string code { get; set; }
                    public string name { get; set; }
                    public string native { get; set; }
                }
            }
            public class Error
            {
                public int code { get; set; }
                public string type { get; set; }
                public string info { get; set; }
            }

            /*
             
             {
                "ip": "134.201.250.155",
                "type": "ipv4",
                "continent_code": "NA",
                "continent_name": "North America",
                "country_code": "US",
                "country_name": "United States",
                "region_code": "CA",
                "region_name": "California",
                "city": "Los Angeles",
                "zip": "90012",
                "latitude": 34.0655517578125,
                "longitude": -118.24053955078125,
                "location": {
                    "geoname_id": 5368361,
                    "capital": "Washington D.C.",
                    "languages": [
                        {
                            "code": "en",
                            "name": "English",
                            "native": "English"
                        }
                    ],
                    "country_flag": "http://assets.ipstack.com/flags/us.svg",
                    "country_flag_emoji": "🇺🇸",
                    "country_flag_emoji_unicode": "U+1F1FA U+1F1F8",
                    "calling_code": "1",
                    "is_eu": false
                }
            }
             
             */
        }
    }
}
