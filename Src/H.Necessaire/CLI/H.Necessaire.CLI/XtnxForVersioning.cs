using System.Linq;

namespace H.Necessaire
{
    public static class XtnxForVersioning
    {
        public static ReleaseVersion ToHsReleaseVersion(this H.Versioning.ReleaseVersion releaseVersion)
        {
            if (releaseVersion is null)
                return null;

            return
                new ReleaseVersion
                {
                    ID = releaseVersion.ID,
                    Description = releaseVersion.Description,
                    Name = releaseVersion.Name,
                    Notes = releaseVersion.Notes?.Select(x => (Note)x).Where(x => !x.IsEmpty()).ToArrayNullIfEmpty(),
                    Version = releaseVersion.Version.ToHsVersion() ,
                };
        }

        public static Version ToHsVersion(this H.Versioning.Version version)
        {
            if (version is null)
                return null;

            return
                new Version
                {
                    Branch = version.Branch,
                    Commit = version.Commit,
                    Timestamp = version.Timestamp,
                    Number = version.Number.ToHsVersionNumber(),
                };
        }

        private static VersionNumber ToHsVersionNumber(this H.Versioning.VersionNumber versionNumber)
        {
            if (versionNumber is null)
                return VersionNumber.Unknown;

            return new VersionNumber
            {
                Build = versionNumber.Build,
                Major = versionNumber.Major,
                Minor = versionNumber.Minor,
                Patch = versionNumber.Patch,
                Suffix = versionNumber.Suffix,
            };
        }
    }
}
