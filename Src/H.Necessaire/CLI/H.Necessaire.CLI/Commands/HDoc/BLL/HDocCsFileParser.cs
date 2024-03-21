using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocCsFileParser : ImADependency
    {
        HDocTypeProcessor hDocTypeProcessor;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            hDocTypeProcessor = dependencyProvider.Get<HDocTypeProcessor>();
        }

        public async Task<OperationResult<HDocTypeInfo[]>> Parse(FileInfo csFile, HDocProjectInfo projectInfo)
        {
            if (csFile?.Exists != true)
                return OperationResult.Fail($"CS File {csFile?.FullName} doesn't exist").WithoutPayload<HDocTypeInfo[]>();

            string sourceCode = await csFile.OpenRead().ReadAsStringAsync(isStreamLeftOpen: false);

            SyntaxTree syntaxTreesyntaxTree = CSharpSyntaxTree.ParseText(sourceCode, CSharpParseOptions.Default);
            CompilationUnitSyntax root = syntaxTreesyntaxTree.GetCompilationUnitRoot();

            IEnumerable<TypeDeclarationSyntax> allTypeDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>();

            if (allTypeDeclarations.Any() != true)
                return Array.Empty<HDocTypeInfo>().ToWinResult();

            IEnumerable<OperationResult<HDocTypeInfo>> typesProcessingResults
                = allTypeDeclarations
                .Select(x => hDocTypeProcessor.Process(x, csFile, sourceCode, projectInfo))
                ;

            return
                typesProcessingResults
                .Where(x => x.IsSuccessful)
                .Select(x => x.Payload)
                .ToArray()
                .ToWinResult()
                ;
        }
    }
}
