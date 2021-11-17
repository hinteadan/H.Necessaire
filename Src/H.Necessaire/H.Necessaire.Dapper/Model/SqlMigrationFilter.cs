using System;
using System.Linq;

namespace H.Necessaire.Dapper
{
    public class SqlMigrationFilter
    {
        public string[] IDs { get; set; }
        public string[] ResourceIdentifiers { get; set; }
        public VersionNumber[] VersionNumbers { get; set; }
        public string[] VersionNumbersAsString => VersionNumbers?.Select(x => x.ToString()).ToArray();
        public DateTime? FromInclusive { get; set; }
        public long? FromInclusiveAsTicks => FromInclusive?.Ticks;
        public DateTime? ToInclusive { get; set; }
        public long? ToInclusiveAsTicks => ToInclusive?.Ticks;
    }
}
