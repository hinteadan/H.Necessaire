using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocPropertyProcessor
    {
        public OperationResult<HDocPropertyInfo> Process(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!propertyDeclaration.IsPublic())
                return OperationResult.Fail($"Property {propertyDeclaration} is not public").WithoutPayload<HDocPropertyInfo>();

            string defaultValue = propertyDeclaration.Initializer?.Value?.ToString().NullIfEmpty();

            AccessorDeclarationSyntax getter = propertyDeclaration.AccessorList?.Accessors.SingleOrDefault(x => x.IsKind(SyntaxKind.GetAccessorDeclaration));
            AccessorDeclarationSyntax setter = propertyDeclaration.AccessorList?.Accessors.SingleOrDefault(x => x.IsKind(SyntaxKind.SetAccessorDeclaration));
            bool isGetterPublic = (getter?.Modifiers.Any() == false) || (propertyDeclaration.ExpressionBody != null);
            bool isSetterPublic = setter?.Modifiers.Any() == false;

            //propertyDeclaration.AccessorList.Accessors

            return
                new HDocPropertyInfo
                {
                    Name = propertyDeclaration.Identifier.Text,
                    IsStatic = propertyDeclaration.IsStatic(),
                    DefaultsTo = defaultValue,
                    HasDefaultValue = defaultValue != null,
                    IsVirtual = propertyDeclaration.IsVirtual(),
                    IsAbstract = propertyDeclaration.Parent is InterfaceDeclarationSyntax || propertyDeclaration.IsAbstract(),
                    IsReadable = isGetterPublic,
                    IsWriteable = isSetterPublic,
                    Type = propertyDeclaration.Type?.ToString(),
                    Expression = propertyDeclaration.ExpressionBody?.ToString(),
                }
                .ToWinResult()
                ;
        }
    }
}
