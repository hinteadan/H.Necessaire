using System;

namespace H.Necessaire
{
    public class Version
    {
        public static readonly Version Unknown = new Version
        {
            Number = VersionNumber.Unknown,
            Timestamp = DateTime.MinValue,
            Branch = "N/A",
            Commit = "N/A",
        };

        public VersionNumber Number { get; set; } = VersionNumber.Unknown;
        public DateTime Timestamp { get; set; }
        public string Branch { get; set; }
        public string Commit { get; set; }
    }
}
