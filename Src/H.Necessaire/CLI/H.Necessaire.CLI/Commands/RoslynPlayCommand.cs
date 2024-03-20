using H.Necessaire.Runtime.CLI.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands
{
    [ID("play-roslyn")]
    public class RoslynPlayCommand : CommandBase
    {
        const string srcFolderRelativePath = "Src/H.Necessaire";

        public override async Task<OperationResult> Run()
        {
            Note[] args = (await GetArguments())?.Jump(1);
            DirectoryInfo srcFolder = new DirectoryInfo(args?.Get("src") ?? Path.Combine(GetCodebaseFolderPath(), "H.Necessaire"));
            if (!srcFolder.Exists)
                return OperationResult.Fail($"Source folder {srcFolder.FullName} doesn't exist");

            FileInfo[] csProjs = srcFolder.GetFiles("*.csproj", SearchOption.AllDirectories);
            ProjectInfo[] projectInfos = csProjs.Select(csProj => new ProjectInfo { 
                ID = Path.GetFileNameWithoutExtension(csProj.Name),
                CsProj = csProj,
                Folder = csProj.Directory,
                CsFiles = csProj.Directory.GetFiles("*.cs", SearchOption.AllDirectories),
            }).ToArray();



            //FileInfo sourceCodeFile = new FileInfo(@"C:\H\H.Necessaire\Src\H.Necessaire\CLI\H.Necessaire.CLI\Commands\PingCommand.cs");
            //string sourceCode = await sourceCodeFile.OpenRead().ReadAsStringAsync(isStreamLeftOpen: false);
            //SyntaxTree syntaxTreesyntaxTree = CSharpSyntaxTree.ParseText(sourceCode, CSharpParseOptions.Default);
            //CompilationUnitSyntax root = syntaxTreesyntaxTree.GetCompilationUnitRoot();
            //ClassDeclarationSyntax[] classes
            //    = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToArrayNullIfEmpty();
            //MethodDeclarationSyntax[] methods
            //    = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArrayNullIfEmpty();


            return OperationResult.Win();
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

    public class ProjectInfo : IStringIdentity
    {
        public string ID { get; set; }
        public FileInfo CsProj { get; set; }
        public DirectoryInfo Folder { get; set; }
        public FileInfo[] CsFiles { get; set; }

        public override string ToString() => ID;
    }
}
