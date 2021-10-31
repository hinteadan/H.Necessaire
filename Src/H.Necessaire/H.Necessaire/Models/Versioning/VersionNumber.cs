﻿using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire
{
    public class VersionNumber
    {
        public static readonly VersionNumber Unknown = new VersionNumber
        {
            Major = 0,
            Minor = 0,
            Patch = 0,
            Build = 0,
            Suffix = "unknown",
        };

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

        public static bool operator ==(VersionNumber a, VersionNumber b)
        {
            if (a is null)
            {
                if (b is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.

            return a.Equals(b);
        }

        public static bool operator !=(VersionNumber a, VersionNumber b)
        {
            if (a is null)
            {
                if (b is null)
                {
                    return false;
                }

                // Only the left side is null.
                return true;
            }
            // Equals handles case of null on right side.
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            VersionNumber other = obj as VersionNumber;

            if (other == null)
                return false;

            return
                Major == other.Major
                && Minor == other.Minor
                && Patch == other.Patch
                && Build == other.Build
                && Suffix == other.Suffix
                ;
        }

        public override int GetHashCode()
        {
            int hashCode = 138437857;
            hashCode = hashCode * -1521134295 + Major.GetHashCode();
            hashCode = hashCode * -1521134295 + Minor.GetHashCode();
            hashCode = hashCode * -1521134295 + Patch.GetHashCode();
            hashCode = hashCode * -1521134295 + Build.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Suffix);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Semantic);
            return hashCode;
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
    }
}
