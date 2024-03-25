using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocConstructorProcessor : ImADependency
    {
        HDocParameterProcessor parameterProcessor;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            parameterProcessor = dependencyProvider.Get<HDocParameterProcessor>();
        }

        public OperationResult<HDocConstructorInfo> Process(ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (!constructorDeclaration.IsPublic() && !constructorDeclaration.IsProtected())
                return OperationResult.Fail($"Constructor {constructorDeclaration} is private").WithoutPayload<HDocConstructorInfo>();

            return
                new HDocConstructorInfo
                {
                    IsProtected = constructorDeclaration.IsProtected(),
                    Parameters = constructorDeclaration.ParameterList?.Parameters.Select(parameterProcessor.Process).Where(x => x.IsSuccessful).Select(x => x.Payload).ToArrayNullIfEmpty(),
                }
                .ToWinResult()
                ;
        }
    }
}
