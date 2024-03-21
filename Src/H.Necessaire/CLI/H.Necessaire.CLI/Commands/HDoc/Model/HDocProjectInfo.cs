using System.IO;

namespace H.Necessaire.CLI.Commands.HDoc.Model
{
    public class HDocProjectInfo : IStringIdentity
    {
        public string ID { get; set; }
        public FileInfo CsProj { get; set; }
        public DirectoryInfo Folder { get; set; }
        public FileInfo[] CsFiles { get; set; }

        public override string ToString() => ID;
    }
}
