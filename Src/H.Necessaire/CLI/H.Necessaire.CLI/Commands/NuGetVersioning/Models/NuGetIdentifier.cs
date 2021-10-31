namespace H.Necessaire.CLI.Commands.NuGetVersioning.Models
{
    class NuGetIdentifier
    {
        private VersionNumber? originalVersionNumber = null;

        public string ID { get; set; } = string.Empty;
        public VersionNumber VersionNumber { get; set; } = VersionNumber.Unknown;
        public VersionNumber OriginalVersionNumber => originalVersionNumber ?? VersionNumber;

        public bool IsDirtyVersion => originalVersionNumber != null && originalVersionNumber != VersionNumber;

        public void IncrementMajorVersion()
        {
            originalVersionNumber = originalVersionNumber ?? VersionNumber.Clone();

            VersionNumber.Major += 1;
        }

        public void IncrementMinorVersion()
        {
            originalVersionNumber = originalVersionNumber ?? VersionNumber.Clone();

            VersionNumber.Minor += 1;
        }

        public void IncrementPatchVersion()
        {
            originalVersionNumber = originalVersionNumber ?? VersionNumber.Clone();

            VersionNumber.Patch += 1;
        }

        public void IncrementBuildVersion()
        {
            originalVersionNumber = originalVersionNumber ?? VersionNumber.Clone();

            VersionNumber.Build += 1;
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
