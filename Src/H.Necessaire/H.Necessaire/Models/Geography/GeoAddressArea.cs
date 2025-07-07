using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire
{
    public struct GeoAddressArea : IEquatable<GeoAddressArea>
    {
        const string separator = "-";

        public string Name { get; set; }
        public string Code { get; set; }

        public bool IsSameAs(GeoAddressArea other)
        {
            return
                Name == other.Name
                && Code == other.Code
                ;
        }
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

        public override bool Equals(object obj)
        {
            return obj is GeoAddressArea area && Equals(area);
        }

        public bool Equals(GeoAddressArea other)
        {
            return Name == other.Name &&
                   Code == other.Code;
        }

        public override int GetHashCode()
        {
            int hashCode = -1719721206;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Code);
            return hashCode;
        }

        public static implicit operator GeoAddressArea(string value) => new GeoAddressArea { Name = value, Code = null };
        public static implicit operator string(GeoAddressArea value) => value.ToString().NullIfEmpty();

        public static implicit operator KeyValuePair<string, string>(GeoAddressArea geoArea)
        {
            return new KeyValuePair<string, string>(geoArea.Code, geoArea.Name);
        }
        public static implicit operator GeoAddressArea(KeyValuePair<string, string> kvp)
        {
            return new GeoAddressArea { Code = kvp.Key, Name = kvp.Value };
        }

        public static implicit operator Note(GeoAddressArea geoArea)
        {
            return new Note(geoArea.Code, geoArea.Name);
        }
        public static implicit operator Note(GeoAddressArea? geoArea)
        {
            return new Note(geoArea?.Code, geoArea?.Name);
        }
        public static implicit operator GeoAddressArea(Note note)
        {
            return new GeoAddressArea { Code = note.ID, Name = note.Value };
        }
        public static implicit operator GeoAddressArea?(Note note)
        {
            return note.IsEmpty() ? null : new GeoAddressArea { Code = note.ID, Name = note.Value };
        }

        public static bool operator ==(GeoAddressArea a, GeoAddressArea b) => a.IsSameAs(b);
        public static bool operator !=(GeoAddressArea a, GeoAddressArea b) => !a.IsSameAs(b);

    }
}
