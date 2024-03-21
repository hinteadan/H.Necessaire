using H.Necessaire.CLI.Commands.HDoc.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocCsProjParser
    {
        static readonly string[] projectsToIgnore = new string[] {
            "H.Necessaire.Testicles.Unit",
            "H.Necessaire.AspNetCoreWebAppSample",
            "H.Necessaire.ReactAppSample",
            "H.Necessaire.CLI.Host",
        };
        const string srcFolderRelativePath = "Src/H.Necessaire";

        public OperationResult<HDocProjectInfo[]> Parse(DirectoryInfo srcFolder = null)
        {
            if (srcFolder == null)
                srcFolder = new DirectoryInfo(Path.Combine(GetCodebaseFolderPath(), "H.Necessaire"));

            if (!srcFolder.Exists)
                return OperationResult.Fail($"Source folder {srcFolder.FullName} doesn't exist").WithoutPayload<HDocProjectInfo[]>();

            IEnumerable<FileInfo> csProjs = srcFolder.EnumerateFiles("*.csproj", SearchOption.AllDirectories);

            HDocProjectInfo[] projectInfos
                = csProjs
                .Select(csProj => new HDocProjectInfo
                {
                    ID = Path.GetFileNameWithoutExtension(csProj.Name),
                    CsProj = csProj,
                    Folder = csProj.Directory,
                    CsFiles = csProj.Directory.GetFiles("*.cs", SearchOption.AllDirectories),
                })
                .Where(proj => proj.ID.NotIn(projectsToIgnore))
                .ToArray()
                ;

            return projectInfos.ToWinResult();
        }

        private static string GetCodebaseFolderPath()
        {
            string codeBase = Assembly.GetExecutingAssembly()?.Location ?? string.Empty;
            UriBuilder uri = new UriBuilder(codeBase);
            string dllPath = Uri.UnescapeDataString(uri.Path);
            int srcFolderIndex = dllPath.ToLowerInvariant().IndexOf(srcFolderRelativePath, StringComparison.InvariantCultureIgnoreCase);
            if (srcFolderIndex < 0)
                return string.Empty;
            string srcFolderPath = Path.GetDirectoryName(dllPath.Substring(0, srcFolderIndex + srcFolderRelativePath.Length)) ?? string.Empty;
            return srcFolderPath;
        }
    }
}
