using H.Necessaire.CLI.Commands.HDoc;
using H.Necessaire.CLI.Commands.HDoc.Model;
using H.Necessaire.Runtime.CLI.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands
{
    [ID("h-doc")]
    public class HDocCommand : CommandBase
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
            HDocProjectInfo[] projectInfos = csProjs.Select(csProj => new HDocProjectInfo
            {
                ID = Path.GetFileNameWithoutExtension(csProj.Name),
                CsProj = csProj,
                Folder = csProj.Directory,
                CsFiles = csProj.Directory.GetFiles("*.cs", SearchOption.AllDirectories),
            }).ToArray();

            HDocTypeInfo[] hTypes = projectInfos.SelectMany(ProcessProjectTypes).ToArray();


            return OperationResult.Win();
        }

        private static HDocTypeInfo[] ProcessProjectTypes(HDocProjectInfo projectInfo)
        {
            if (projectInfo?.CsFiles?.Any() != true)
                return Array.Empty<HDocTypeInfo>();

            return
                projectInfo
                .CsFiles
                .SelectMany(
                    csFile => ProcessCsFile(csFile, projectInfo)
                )
                .ToNoNullsArray(nullIfEmpty: false)
                ;
        }

        private static HDocTypeInfo[] ProcessCsFile(FileInfo csFile, HDocProjectInfo projectInfo)
        {
            if (csFile?.Exists != true)
                return Array.Empty<HDocTypeInfo>();

            string sourceCode = File.ReadAllText(csFile.FullName);
            SyntaxTree syntaxTreesyntaxTree = CSharpSyntaxTree.ParseText(sourceCode, CSharpParseOptions.Default);
            CompilationUnitSyntax root = syntaxTreesyntaxTree.GetCompilationUnitRoot();

            IEnumerable<TypeDeclarationSyntax> allTypeDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>();

            if (allTypeDeclarations.Any() != true)
                return Array.Empty<HDocTypeInfo>();

            return
                allTypeDeclarations
                .Select(x => ProcessType(x, csFile, sourceCode, projectInfo))
                .ToNoNullsArray(nullIfEmpty: false)
                ;
        }

        private static HDocTypeInfo ProcessType(TypeDeclarationSyntax typeDeclaration, FileInfo csFile, string sourceCode, HDocProjectInfo projectInfo)
        {
            if (projectInfo.ID.In(projectsToIgnore))
                return null;

            if (!typeDeclaration.IsPublic())
                return null;

            if (typeDeclaration.Parent is ClassDeclarationSyntax parentClass && !parentClass.IsPublic())
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
                new HDocTypeInfo
                {
                    ID = $"{(nsName.IsEmpty() ? "" : $"{nsName}.")}{typeDeclaration.Identifier.Text}",
                    Module = projectInfo.ID,
                    Name = ProcessName(typeDeclaration),
                    Namespace = nsName,
                    Category = category,
                    IsStatic = typeDeclaration.IsStatic(),
                    Constructors = constructors.Select(ProcessConstructor).ToNoNullsArray(),
                    Methods = methods.Select(ProcessMethod).ToNoNullsArray(),
                    Properties = properties.Select(ProcessProperty).ToNoNullsArray(),
                };
        }

        private static string ProcessName(TypeDeclarationSyntax typeDeclaration)
        {
            if (typeDeclaration.Parent is ClassDeclarationSyntax parentClass)
                return $"{parentClass.Identifier.Text}.{typeDeclaration.Identifier.Text}";

            return typeDeclaration.Identifier.Text;
        }

        private static HDocConstructorInfo ProcessConstructor(ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (!constructorDeclaration.IsPublic())
                return null;

            return
                new HDocConstructorInfo { };
        }

        private static HDocMethodInfo ProcessMethod(MethodDeclarationSyntax methodDeclaration)
        {
            if (!methodDeclaration.IsPublic())
                return null;

            return
                new HDocMethodInfo
                {
                    Name = methodDeclaration.Identifier.Text,
                    IsStatic = methodDeclaration.IsStatic(),
                };
        }

        private static HDocPropertyInfo ProcessProperty(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!propertyDeclaration.IsPublic())
                return null;

            return
                new HDocPropertyInfo
                {
                    Name = propertyDeclaration.Identifier.Text,
                    IsStatic = propertyDeclaration.IsStatic(),
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
}
