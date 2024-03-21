using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocMethodProcessor
    {
        public OperationResult<HDocMethodInfo> Process(MethodDeclarationSyntax methodDeclaration)
        {
            if (!methodDeclaration.IsPublic())
                return OperationResult.Fail($"Method {methodDeclaration} is not public").WithoutPayload<HDocMethodInfo>();

            return
                new HDocMethodInfo
                {
                    Name = methodDeclaration.Identifier.Text,
                    IsStatic = methodDeclaration.IsStatic(),
                }
                .ToWinResult()
                ;
        }
    }
}
