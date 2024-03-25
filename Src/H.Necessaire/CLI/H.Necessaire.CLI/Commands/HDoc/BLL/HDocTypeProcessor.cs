using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocTypeProcessor : ImADependency
    {
        #region Construct
        static string pathRootKey = Path.Combine("Src", "H.Necessaire");
        HDocConstructorProcessor constructorProcessor;
        HDocMethodProcessor methodProcessor;
        HDocPropertyProcessor propertyProcessor;
        HDocFieldProcessor fieldProcessor;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            constructorProcessor = dependencyProvider.Get<HDocConstructorProcessor>();
            methodProcessor = dependencyProvider.Get<HDocMethodProcessor>();
            propertyProcessor = dependencyProvider.Get<HDocPropertyProcessor>();
            fieldProcessor = dependencyProvider.Get<HDocFieldProcessor>();
        }
        #endregion

        public OperationResult<HDocTypeInfo> Process(TypeDeclarationSyntax typeDeclaration, FileInfo csFile, string sourceCode, HDocProjectInfo projectInfo)
        {
            if (typeDeclaration is null)
                return OperationResult.Fail("Type Declaration is NULL").WithoutPayload<HDocTypeInfo>();

            if (!typeDeclaration.IsPublic())
                return OperationResult.Fail($"{typeDeclaration.Identifier.Text} type is not public").WithoutPayload<HDocTypeInfo>();

            if (typeDeclaration.Parent is ClassDeclarationSyntax parentClass && !parentClass.IsPublic())
                return OperationResult.Fail($"{typeDeclaration.Identifier.Text}'s parent class is not public").WithoutPayload<HDocTypeInfo>();

            NamespaceDeclarationSyntax ns = FindNamespaceFor(typeDeclaration);
            string name = ProcessName(typeDeclaration);
            string nsName = ns?.Name.ToString();
            string category
                = csFile.Directory.FullName == projectInfo.CsProj.Directory.FullName
                ? null :
                csFile.Directory.FullName
                .Replace(projectInfo.CsProj.Directory.FullName, "")
                .Replace("\\", ".")
                .Substring(1)
                ;
            
            int filePathRootIndex = csFile.FullName.IndexOf(pathRootKey) + pathRootKey.Length + 1;
            string[] filePath = csFile.FullName.Substring(filePathRootIndex).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string[] folderPath = filePath.Take(filePath.Length - 1).ToArrayNullIfEmpty();

            IEnumerable<ConstructorDeclarationSyntax> constructors = typeDeclaration.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
            IEnumerable<FieldDeclarationSyntax> fields = typeDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>();
            IEnumerable<MethodDeclarationSyntax> methods = typeDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
            IEnumerable<PropertyDeclarationSyntax> properties = typeDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>();

            return
                new HDocTypeInfo
                {
                    ID = $"{(nsName.IsEmpty() ? "" : $"{nsName}.")}{name}",
                    Module = projectInfo.ID,
                    FilePath = filePath,
                    FolderPath = folderPath,
                    Name = name,
                    Namespace = nsName,
                    Category = category,
                    IsStatic = typeDeclaration.IsStatic(),
                    IsSealed = typeDeclaration.IsSealed(),
                    IsInterface = typeDeclaration is InterfaceDeclarationSyntax,
                    IsAbstract = typeDeclaration is InterfaceDeclarationSyntax || typeDeclaration.IsAbstract(),
                    Constructors = constructors.Select(constructorProcessor.Process).Where(x => x.IsSuccessful).Select(x => x.Payload).ToArrayNullIfEmpty(),
                    Fields = fields.Select(fieldProcessor.Process).Where(x => x.IsSuccessful).Select(x => x.Payload).ToArrayNullIfEmpty(),
                    Methods = methods.Select(methodProcessor.Process).Where(x => x.IsSuccessful).Select(x => x.Payload).ToArrayNullIfEmpty(),
                    Properties = properties.Select(propertyProcessor.Process).Where(x => x.IsSuccessful).Select(x => x.Payload).ToArrayNullIfEmpty(),
                    InheritedTypeNames = typeDeclaration.BaseList?.Types.Select(b => b.ToString()).ToArrayNullIfEmpty(),
                }
                .ToWinResult()
                ;
        }

        private static string ProcessName(TypeDeclarationSyntax typeDeclaration)
        {
            if (typeDeclaration.Parent is ClassDeclarationSyntax parentClass)
                return $"{parentClass.Identifier.Text}.{typeDeclaration.Identifier.Text}";

            return typeDeclaration.Identifier.Text;
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


    }
}
