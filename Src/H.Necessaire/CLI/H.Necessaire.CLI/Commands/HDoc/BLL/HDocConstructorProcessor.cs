using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocConstructorProcessor
    {
        public OperationResult<HDocConstructorInfo> Process(ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (!constructorDeclaration.IsPublic())
                return OperationResult.Fail($"Constructor {constructorDeclaration} is not public").WithoutPayload<HDocConstructorInfo>();

            return
                new HDocConstructorInfo
                {
                }
                .ToWinResult()
                ;
        }
    }
}
