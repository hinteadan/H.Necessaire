using H.Necessaire.Runtime.CLI.Commands;
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
        static readonly string[] projectsToIgnore = new string[] {
            "H.Necessaire.Testicles.Unit",
            "H.Necessaire.AspNetCoreWebAppSample",
            "H.Necessaire.ReactAppSample",
            "H.Necessaire.CLI.Host",
        };
        const string srcFolderRelativePath = "Src/H.Necessaire";

        public override async Task<OperationResult> Run()
        {
            Versioning.Version version = H.Versioning.Version.Self.GetCurrent() ?? Versioning.Version.Unknown;

            Note[] args = (await GetArguments())?.Jump(1);
            DirectoryInfo srcFolder = new DirectoryInfo(args?.Get("src") ?? Path.Combine(GetCodebaseFolderPath(), "H.Necessaire"));
            if (!srcFolder.Exists)
                return OperationResult.Fail($"Source folder {srcFolder.FullName} doesn't exist");

            FileInfo[] csProjs = srcFolder.GetFiles("*.csproj", SearchOption.AllDirectories);
            ProjectInfo[] projectInfos = csProjs.Select(csProj => new ProjectInfo
            {
                ID = Path.GetFileNameWithoutExtension(csProj.Name),
                CsProj = csProj,
                Folder = csProj.Directory,
                CsFiles = csProj.Directory.GetFiles("*.cs", SearchOption.AllDirectories),
            }).ToArray();

            HTypeInfo[] hTypes = projectInfos.SelectMany(ProcessProjectTypes).ToArray();


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
            if (csFile?.Exists != true)
                return Array.Empty<HTypeInfo>();

            string sourceCode = File.ReadAllText(csFile.FullName);
            SyntaxTree syntaxTreesyntaxTree = CSharpSyntaxTree.ParseText(sourceCode, CSharpParseOptions.Default);
            CompilationUnitSyntax root = syntaxTreesyntaxTree.GetCompilationUnitRoot();

            IEnumerable<TypeDeclarationSyntax> allTypeDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>();

            if (allTypeDeclarations.Any() != true)
                return Array.Empty<HTypeInfo>();

            return
                allTypeDeclarations
                .Select(x => ProcessType(x, csFile, sourceCode, projectInfo))
                .ToNoNullsArray(nullIfEmpty: false)
                ;
        }

        private static HTypeInfo ProcessType(TypeDeclarationSyntax typeDeclaration, FileInfo csFile, string sourceCode, ProjectInfo projectInfo)
        {
            if (projectInfo.ID.In(projectsToIgnore))
                return null;

            if (!IsPublic(typeDeclaration))
                return null;

            if (typeDeclaration.Parent is ClassDeclarationSyntax parentClass && !IsPublic(parentClass))
                return null;

            NamespaceDeclarationSyntax ns = FindNamespaceFor(typeDeclaration);
            string nsName = ns?.Name.ToString();

            string category
                = csFile.Directory.FullName == projectInfo.CsProj.Directory.FullName
                ? null :
                csFile.Directory.FullName
                .Replace(projectInfo.CsProj.Directory.FullName, "")
                .Replace("\\", ".")
                .Substring(1)
                ;

            IEnumerable<ConstructorDeclarationSyntax> constructors = typeDeclaration.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
            IEnumerable<MethodDeclarationSyntax> methods = typeDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
            IEnumerable<PropertyDeclarationSyntax> properties = typeDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>();

            return
                new HTypeInfo
                {
                    ID = $"{(nsName.IsEmpty() ? "" : $"{nsName}.")}{typeDeclaration.Identifier.Text}",
                    Module = projectInfo.ID,
                    Name = typeDeclaration.Identifier.Text,
                    Namespace = nsName,
                    Category = category,
                    IsStatic = IsStatic(typeDeclaration),
                    Constructors = constructors.Select(ProcessConstructor).ToNoNullsArray(),
                    Methods = methods.Select(ProcessMethod).ToNoNullsArray(),
                    Properties = properties.Select(ProcessProperty).ToNoNullsArray(),
                };
        }

        private static HConstructorInfo ProcessConstructor(ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (!IsPublic(constructorDeclaration))
                return null;

            return
                new HConstructorInfo { };
        }

        private static HMethodInfo ProcessMethod(MethodDeclarationSyntax methodDeclaration)
        {
            if (!IsPublic(methodDeclaration))
                return null;

            return
                new HMethodInfo
                {
                    Name = methodDeclaration.Identifier.Text,
                    IsStatic = IsStatic(methodDeclaration),
                };
        }

        private static HPropertyInfo ProcessProperty(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!IsPublic(propertyDeclaration))
                return null;

            return
                new HPropertyInfo
                {
                    Name = propertyDeclaration.Identifier.Text,
                    IsStatic = IsStatic(propertyDeclaration),
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

        private static bool IsPublic(MemberDeclarationSyntax memberDeclaration)
        {
            return memberDeclaration?.Modifiers.Any(m => m.ToString() == "public") == true;
        }

        private static bool IsStatic(MemberDeclarationSyntax memberDeclaration)
        {
            return memberDeclaration?.Modifiers.Any(m => m.ToString() == "static") == true;
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
        public bool IsStatic { get; set; }

        public HConstructorInfo[] Constructors { get; set; }
        public HPropertyInfo[] Properties { get; set; }
        public HMethodInfo[] Methods { get; set; }
    }

    public class HConstructorInfo
    {
    }

    public class HMethodInfo
    {
        public string Name { get; set; }
        public bool IsStatic { get; set; }
    }

    public class HPropertyInfo
    {
        public string Name { get; set; }
        public bool IsStatic { get; set; }
    }
}
