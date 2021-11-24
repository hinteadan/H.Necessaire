using System;
using System.Globalization;

namespace H.Necessaire
{
    public class Version
    {
        private static readonly string[] possibleVersionPartsSeparators = new string[] { "\r", "\n", "\t", "; ", ", ", "| " };

        public static readonly Version Unknown = new Version
        {
            Number = VersionNumber.Unknown,
            Timestamp = DateTime.MinValue,
            Branch = "N/A",
            Commit = "N/A",
        };

        public Version()
        {
        }

        public Version(VersionNumber number, DateTime timestamp, string branch, string commit) : this()
        {
            this.Number = number;
            this.Timestamp = timestamp;
            this.Branch = branch;
            this.Commit = commit;
        }

        public VersionNumber Number { get; set; } = VersionNumber.Unknown;
        public DateTime Timestamp { get; set; }
        public string Branch { get; set; }
        public string Commit { get; set; }

        public override string ToString()
        {
            return ToString(Environment.NewLine);
        }

        public string ToString(string separator)
        {
            return $"{Number}{separator}{Timestamp.ToUniversalTime().ToString(DataPrintingExtensions.ParsableTimeStampFormat)}{separator}{Branch}{separator}{Commit}";
        }

        public static Version Parse(string versionString)
        {
            if (string.IsNullOrWhiteSpace(versionString) || !versionString.Contains("."))
            {
                throw new InvalidOperationException("The given version string does not have the expected format");
            }

            string[] parts = versionString.Split(possibleVersionPartsSeparators, StringSplitOptions.RemoveEmptyEntries);

            VersionNumber number = VersionNumber.Parse(parts[0].Trim());
            DateTime timestamp = parts.Length > 1 ? DateTime.ParseExact(parts[1].Trim(), DataPrintingExtensions.ParsableTimeStampFormat, CultureInfo.InvariantCulture).EnsureUtc() : DateTime.MinValue;
            string branch = parts.Length > 2 ? parts[2].Trim() : null;
            string commit = parts.Length > 3 ? parts[3].Trim() : null;

            return new Version(number, timestamp, branch, commit);
        }
    }
}
