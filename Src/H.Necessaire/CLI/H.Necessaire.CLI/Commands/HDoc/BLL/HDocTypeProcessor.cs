﻿using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocTypeProcessor
    {
        public OperationResult<HDocTypeInfo> Process(TypeDeclarationSyntax typeDeclaration, FileInfo csFile, string sourceCode, HDocProjectInfo projectInfo)
        {
            if (typeDeclaration is null)
                return OperationResult.Fail("Type Declaration is NULL").WithoutPayload<HDocTypeInfo>();

            if (!typeDeclaration.IsPublic())
                return OperationResult.Fail($"{typeDeclaration.Identifier.Text} type is not public").WithoutPayload<HDocTypeInfo>();

            if (typeDeclaration.Parent is ClassDeclarationSyntax parentClass && !parentClass.IsPublic())
                return OperationResult.Fail($"{typeDeclaration.Identifier.Text}'s parent class is not public").WithoutPayload<HDocTypeInfo>();

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
                    //Constructors = constructors.Select(ProcessConstructor).ToNoNullsArray(),
                    //Methods = methods.Select(ProcessMethod).ToNoNullsArray(),
                    //Properties = properties.Select(ProcessProperty).ToNoNullsArray(),
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
