namespace H.Necessaire.CLI.Commands.NuGetVersioning.Models
{
    class NuGetIdentifier : IStringIdentity
    {
        private VersionNumber originalVersionNumber = null;

        public string ID { get; set; } = string.Empty;
        public VersionNumber VersionNumber { get; set; } = VersionNumber.Unknown;
        public VersionNumber OriginalVersionNumber => originalVersionNumber ?? VersionNumber;

        public bool IsDirtyVersion => originalVersionNumber != null && !originalVersionNumber.IsEqualWith(VersionNumber);

        public void IncrementMajorVersion()
        {
            originalVersionNumber = originalVersionNumber ?? VersionNumber.Clone();

            VersionNumber.Major += 1;
            VersionNumber.Minor = 0;
            VersionNumber.Patch = 0;
            VersionNumber.Build = null;
            VersionNumber.Suffix = null;
        }

        public void IncrementMinorVersion()
        {
            originalVersionNumber = originalVersionNumber ?? VersionNumber.Clone();

            VersionNumber.Minor += 1;
            VersionNumber.Patch = 0;
            VersionNumber.Build = null;
            VersionNumber.Suffix = null;
        }

        public void IncrementPatchVersion()
        {
            originalVersionNumber = originalVersionNumber ?? VersionNumber.Clone();

            VersionNumber.Patch = (VersionNumber.Patch ?? 0) + 1;
            VersionNumber.Build = null;
            VersionNumber.Suffix = null;
        }

        public void IncrementBuildVersion()
        {
            originalVersionNumber = originalVersionNumber ?? VersionNumber.Clone();

            VersionNumber.Build = (VersionNumber.Build ?? 0) + 1;
            VersionNumber.Suffix = null;
        }

        public void SetVersionSuffix(string suffix)
        {
            originalVersionNumber = originalVersionNumber ?? VersionNumber.Clone();

            VersionNumber.Suffix = suffix;
        }

        public void UpdateVersionTo(VersionNumber newVersion)
        {
            originalVersionNumber = originalVersionNumber ?? VersionNumber.Clone();

            VersionNumber = newVersion;
        }

        public override string ToString()
        {
            return $"{(IsDirtyVersion ? "** " : null)}{ID} v{VersionNumber}";
        }
    }
}
