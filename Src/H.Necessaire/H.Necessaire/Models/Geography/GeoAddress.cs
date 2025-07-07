using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire
{
    public class GeoAddress : IEquatable<GeoAddress>
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

        public static implicit operator GeoAddress(string address)
        {
            return new GeoAddress
            {
                StreetAddress = address,
                Notes = address.NoteAs("SourceAddress").AsArray(),
            };
        }

        public GeoAddress Clone()
            => new GeoAddress
            {
                Continent = Continent,
                Country = Country,
                State = State,
                County = County,
                City = City,
                CityArea = CityArea,
                ZipCode = ZipCode,
                StreetAddress = StreetAddress,
                Notes = Notes.IsEmpty() ? null : (null as Note[]).Push(Notes),
            };

        public bool IsSameAs(GeoAddress other)
        {
            if (other is null)
                return false;

            return
                Continent == other.Continent
                && Country == other.Country
                && State == other.State
                && County == other.County
                && City == other.City
                && CityArea == other.CityArea
                && ZipCode == other.ZipCode
                && StreetAddress == other.StreetAddress
                && (
                    Notes.IsEmpty() && (other.Notes).IsEmpty()
                    ||
                    Notes.All(note => note.In(other.Notes))
                )
                ;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GeoAddress);
        }

        public bool Equals(GeoAddress other) => IsSameAs(other);

        public override int GetHashCode()
        {
            int hashCode = -564145069;
            hashCode = hashCode * -1521134295 + Continent.GetHashCode();
            hashCode = hashCode * -1521134295 + Country.GetHashCode();
            hashCode = hashCode * -1521134295 + State.GetHashCode();
            hashCode = hashCode * -1521134295 + County.GetHashCode();
            hashCode = hashCode * -1521134295 + City.GetHashCode();
            hashCode = hashCode * -1521134295 + CityArea.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ZipCode);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StreetAddress);
            hashCode = hashCode * -1521134295 + EqualityComparer<Note[]>.Default.GetHashCode(Notes);
            return hashCode;
        }

        public static bool operator ==(GeoAddress a, GeoAddress b) => a?.IsSameAs(b) == true;
        public static bool operator !=(GeoAddress a, GeoAddress b) => a?.IsSameAs(b) != true;
    }
}
