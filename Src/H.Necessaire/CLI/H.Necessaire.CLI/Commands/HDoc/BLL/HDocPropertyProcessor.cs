using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocPropertyProcessor
    {
        public OperationResult<HDocPropertyInfo> Process(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!propertyDeclaration.IsPublic())
                return OperationResult.Fail($"Property {propertyDeclaration} is not public").WithoutPayload<HDocPropertyInfo>();

            return
                new HDocPropertyInfo
                {
                    Name = propertyDeclaration.Identifier.Text,
                    IsStatic = propertyDeclaration.IsStatic(),
                }
                .ToWinResult()
                ;
        }
    }
}
