using System.IO;

namespace H.Necessaire.CLI.Commands.NuGetVersioning.Models
{
    class CsprojInfo : IStringIdentity
    {
        public CsprojInfo(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
            ID = fileInfo.Name;
        }

        public string ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TargetFramework { get; set; }
        public bool IsNetFxFormat { get; set; } = false;
        public FileInfo FileInfo { get; }
        public NuGetIdentifier[] NuGets { get; set; } = new NuGetIdentifier[0];

        public override string ToString()
        {
            return ID;
        }
    }
}
