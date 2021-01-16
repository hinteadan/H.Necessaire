namespace H.Necessaire
{
    public class VersionNumber
    {
        public static readonly VersionNumber Unknown = new VersionNumber
        {
            Major = 0,
            Minor = 0,
            Build = 0,
            Patch = 0,
            Suffix = "unknown",
            Semantic = "0.0.0.0-unknown",
        };

        public int Major { get; set; } = 0;
        public int Minor { get; set; } = 0;
        public int? Patch { get; set; }
        public int? Build { get; set; }
        public string Suffix { get; set; }

        public string Semantic { get; set; }
    }
}
