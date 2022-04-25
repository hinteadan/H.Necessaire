using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire
{
    public class VersionNumber : IEquatable<VersionNumber>, IComparable<VersionNumber>
    {
        public static readonly IComparer<VersionNumber> Comparer = new VersionNumberComparer();

        public static readonly VersionNumber Unknown = new VersionNumber
        {
            Major = 0,
            Minor = 0,
            Patch = 0,
            Build = 0,
            Suffix = null,
        };

        public VersionNumber() { }
        public VersionNumber(int major = 0, int minor = 0, int? patch = null, int? build = null, string suffix = null)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Build = build;
            this.Suffix = suffix;
        }

        public int Major { get; set; } = 0;
        public int Minor { get; set; } = 0;
        public int? Patch { get; set; }
        public int? Build { get; set; }
        public string Suffix { get; set; }

        public string Semantic => ToString();

        public override string ToString()
        {
            StringBuilder versionString = new StringBuilder($"{this.Major}.{this.Minor}");

            if (this.Patch != null)
            {
                versionString.Append($".{this.Patch}");

                if (this.Build != null)
                {
                    versionString.Append($".{this.Build}");
                }
            }

            if (!string.IsNullOrWhiteSpace(this.Suffix))
            {
                versionString.Append($"-{this.Suffix}");
            }

            return versionString.ToString();
        }

        public static VersionNumber Parse(string versionNumber)
        {
            if (string.IsNullOrWhiteSpace(versionNumber) || !versionNumber.Contains("."))
            {
                throw new InvalidOperationException("The given version is not in semantic format");
            }

            string[] versionAndSuffixParts = versionNumber.Split(new char[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var versionString = versionAndSuffixParts[0].ToLower().Replace("v", string.Empty).Trim();
            string suffix = versionAndSuffixParts.Length > 1 ? versionAndSuffixParts[1].Trim() : null;

            var versionParts = versionString.Split(new char[] { '.' }, 4, StringSplitOptions.RemoveEmptyEntries);
            var major = int.Parse(versionParts[0].Trim());
            var minor = int.Parse(versionParts[1].Trim());
            var patch = versionParts.Length > 2 ? (int?)int.Parse(versionParts[2].Trim()) : null;
            var build = versionParts.Length > 3 ? (int?)int.Parse(versionParts[3].Trim()) : null;

            return new VersionNumber
            {
                Major = major,
                Minor = minor,
                Patch = patch,
                Build = build,
                Suffix = suffix,
            };
        }

        public bool IsEqualWith(VersionNumber other)
        {
            if ((object)other is null)
                return false;

            return
                Major == other.Major
                && Minor == other.Minor
                && Patch == other.Patch
                && Build == other.Build
                && Suffix == other.Suffix
                ;
        }

        public VersionNumber Clone()
        {
            return
                new VersionNumber
                {
                    Major = Major,
                    Minor = Minor,
                    Patch = Patch,
                    Build = Build,
                    Suffix = Suffix,
                };
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3049;
                hash = hash * 5039 + Major.GetHashCode();
                hash = hash * 883 + Minor.GetHashCode();
                hash = hash * 9719 + Patch.GetHashCode();
                hash = hash * 1607 + Build.GetHashCode();
                hash = hash * 1543 + EqualityComparer<string>.Default.GetHashCode(Suffix);
                return hash;
            }
        }
        public int CompareTo(VersionNumber other) => Comparer.Compare(this, other);
        public override bool Equals(object other) => IsEqualWith(other as VersionNumber);
        public bool Equals(VersionNumber other) => IsEqualWith(other);
        public static bool operator ==(VersionNumber a, VersionNumber b) => (object)a is null ? (object)b is null : a.Equals(b);
        public static bool operator !=(VersionNumber a, VersionNumber b) => (object)a is null ? !((object)b is null) : !a.Equals(b);
        public static bool operator >(VersionNumber a, VersionNumber b) => Comparer.Compare(a, b) > 0;
        public static bool operator <(VersionNumber a, VersionNumber b) => Comparer.Compare(a, b) < 0;
        public static bool operator >=(VersionNumber a, VersionNumber b) => Comparer.Compare(a, b) >= 0;
        public static bool operator <=(VersionNumber a, VersionNumber b) => Comparer.Compare(a, b) <= 0;

        private class VersionNumberComparer : IComparer<VersionNumber>
        {
            public int Compare(VersionNumber x, VersionNumber y)
            {
                if (x is null && y is null)
                    return 0;
                if (x is null && !(y is null))
                    return -1;
                if (!(x is null) && y is null)
                    return 1;

                if (x.Major < y.Major)
                    return -1;
                if (x.Major > y.Major)
                    return 1;

                if (x.Minor < y.Minor)
                    return -1;
                if (x.Minor > y.Minor)
                    return 1;

                if ((x.Patch ?? 0) < (y.Patch ?? 0))
                    return -1;
                if ((x.Patch ?? 0) > (y.Patch ?? 0))
                    return 1;

                if ((x.Build ?? 0) < (y.Build ?? 0))
                    return -1;
                if ((x.Build ?? 0) > (y.Build ?? 0))
                    return 1;

                return string.Compare(x.Suffix, y.Suffix, ignoreCase: true);
            }
        }
    }
}
