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
        HDocManager hDocManager;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            hDocManager = dependencyProvider.Get<HDocManager>();
        }

        public override async Task<OperationResult> Run()
        {
            OperationResult<HDocumentation> docResult = await hDocManager.ParseDocumentation();
            if (!docResult.IsSuccessful)
                return docResult;
            HDocumentation documentation = docResult.Payload;


            return OperationResult.Win();
        }

        //private static HDocMethodInfo ProcessMethod(MethodDeclarationSyntax methodDeclaration)
        //{
        //    if (!methodDeclaration.IsPublic())
        //        return null;

        //    return
        //        new HDocMethodInfo
        //        {
        //            Name = methodDeclaration.Identifier.Text,
        //            IsStatic = methodDeclaration.IsStatic(),
        //        };
        //}

        //private static HDocPropertyInfo ProcessProperty(PropertyDeclarationSyntax propertyDeclaration)
        //{
        //    if (!propertyDeclaration.IsPublic())
        //        return null;

        //    return
        //        new HDocPropertyInfo
        //        {
        //            Name = propertyDeclaration.Identifier.Text,
        //            IsStatic = propertyDeclaration.IsStatic(),
        //        };
        //}
    }
}
