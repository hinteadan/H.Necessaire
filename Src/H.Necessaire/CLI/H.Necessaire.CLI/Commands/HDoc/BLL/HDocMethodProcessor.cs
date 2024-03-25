using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocMethodProcessor : ImADependency
    {
        HDocParameterProcessor parameterProcessor;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            parameterProcessor = dependencyProvider.Get<HDocParameterProcessor>();
        }

        public OperationResult<HDocMethodInfo> Process(MethodDeclarationSyntax methodDeclaration)
        {
            if (!methodDeclaration.IsPublic())
                return OperationResult.Fail($"Method {methodDeclaration} is not public").WithoutPayload<HDocMethodInfo>();

            return
                new HDocMethodInfo
                {
                    Name = methodDeclaration.Identifier.Text,
                    IsStatic = methodDeclaration.IsStatic(),
                    IsVirtual = methodDeclaration.IsVirtual(),
                    IsAbstract = methodDeclaration.Parent is InterfaceDeclarationSyntax || methodDeclaration.IsAbstract(),
                    ReturnType = methodDeclaration.ReturnType?.ToString(),
                    Parameters = methodDeclaration.ParameterList?.Parameters.Select(parameterProcessor.Process).Where(x => x.IsSuccessful).Select(x => x.Payload).ToArrayNullIfEmpty(),
                }
                .And(methodInfo => {
                    if (methodInfo.ReturnType == "void")
                        methodInfo.ReturnType = null;
                })
                .ToWinResult()
                ;
        }
    }
}
