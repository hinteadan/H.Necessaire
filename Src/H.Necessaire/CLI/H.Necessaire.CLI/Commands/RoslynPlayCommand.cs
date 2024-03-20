﻿using H.Necessaire.Runtime.CLI.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
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

            HTypeInfo[] hTypes = projectInfos.SelectMany(ProcessProjectTypes).ToArray();

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

        private static HTypeInfo[] ProcessProjectTypes(ProjectInfo projectInfo)
        {
            if (projectInfo?.CsFiles?.Any() != true)
                return Array.Empty<HTypeInfo>();

            return
                projectInfo
                .CsFiles
                .SelectMany(
                    csFile => ProcessCsFile(csFile, projectInfo)
                )
                .ToNoNullsArray(nullIfEmpty: false)
                ;
        }

        private static HTypeInfo[] ProcessCsFile(FileInfo csFile, ProjectInfo projectInfo)
        {
            if(csFile?.Exists != true)
                return Array.Empty<HTypeInfo>();

            string sourceCode = File.ReadAllText(csFile.FullName);
            SyntaxTree syntaxTreesyntaxTree = CSharpSyntaxTree.ParseText(sourceCode, CSharpParseOptions.Default);
            CompilationUnitSyntax root = syntaxTreesyntaxTree.GetCompilationUnitRoot();

            IEnumerable<TypeDeclarationSyntax> allTypeDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>();

            if(allTypeDeclarations.Any() != true)
                return Array.Empty<HTypeInfo>();

            return
                allTypeDeclarations
                .Select(x => ProcessType(x, csFile, sourceCode, projectInfo))
                .ToNoNullsArray(nullIfEmpty: false)
                ;
        }

        private static HTypeInfo ProcessType(TypeDeclarationSyntax typeDeclaration, FileInfo csFile, string sourceCode, ProjectInfo projectInfo)
        {
            NamespaceDeclarationSyntax ns = FindNamespaceFor(typeDeclaration);
            string nsName = ns?.Name.ToString();

            string category 
                = csFile.Directory.FullName == projectInfo.CsProj.Directory.FullName
                ? null :
                csFile.Directory.FullName
                .Replace(projectInfo.CsProj.Directory.FullName, "")
                ;

            return
                new HTypeInfo
                {
                    ID = $"{(nsName.IsEmpty() ? "" : $"{nsName}.")}{typeDeclaration.Identifier.Text}",
                    Module = projectInfo.ID,
                    Name = typeDeclaration.Identifier.Text,
                    Namespace = nsName,
                    Category = category,
                };
        }

        private static NamespaceDeclarationSyntax FindNamespaceFor(SyntaxNode syntaxNode)
        {
            if (syntaxNode == null)
                return null;

            if (syntaxNode is NamespaceDeclarationSyntax self)
                return self;

            if (syntaxNode.Parent is null)
                return null;

            if (syntaxNode.Parent is NamespaceDeclarationSyntax parent)
                return parent;

            return FindNamespaceFor(syntaxNode.Parent);
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

    public class HTypeInfo : IStringIdentity
    {
        public string ID { get; set; }
        public string Module { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Namespace { get; set; }

        public HConstructorInfo[] Constructors { get; set; }
        public HPropertyInfo[] Properties { get; set; }
        public HMethodInfo[] Methods { get; set; }
    }

    public class HConstructorInfo
    {
    }

    public class HMethodInfo
    { 
    }

    public class HPropertyInfo
    {
    }
}
