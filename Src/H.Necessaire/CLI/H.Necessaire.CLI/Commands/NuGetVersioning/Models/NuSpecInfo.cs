namespace H.Necessaire.CLI.Commands.NuGetVersioning.Models
{
    class NuSpecInfo : NuGetIdentifier
    {
        public NuSpecInfo(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        public string Title { get; set; } = string.Empty;
        public FileInfo FileInfo { get; }
        public NuGetIdentifier[] Dependencies { get; set; } = new NuGetIdentifier[0];
    }
}
