using System.IO;

namespace H.Necessaire.CLI.Commands.BridgeDotNet.Model
{
    class BridgeDotNetCopyParams
    {
        public DirectoryInfo SourceProjectRoot { get; set; }
        public DirectoryInfo[] DestinationProjectsRoots { get; set; } = new DirectoryInfo[0];
    }
}
