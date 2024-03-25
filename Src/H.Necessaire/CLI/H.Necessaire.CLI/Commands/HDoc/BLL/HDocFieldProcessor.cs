using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocFieldProcessor
    {
        public OperationResult<HDocFieldInfo> Process(FieldDeclarationSyntax fieldDeclaration)
        {
            if (!fieldDeclaration.IsPublic() && !fieldDeclaration.IsProtected())
                return OperationResult.Fail($"Field {fieldDeclaration} is private").WithoutPayload<HDocFieldInfo>();



            string defaultValue = fieldDeclaration.Declaration.Variables.Single().Initializer?.Value?.ToString().NullIfEmpty();

            return
                new HDocFieldInfo
                {
                    Name = fieldDeclaration.Declaration.Variables.Single().Identifier.Text,
                    IsConst = fieldDeclaration.IsConst(),
                    IsStatic = fieldDeclaration.IsStatic() || fieldDeclaration.IsConst(),
                    IsReadonly = fieldDeclaration.IsReadonly(),
                    Type = fieldDeclaration.Declaration.Type?.ToString(),
                    DefaultsTo = defaultValue,
                    HasDefaultValue = defaultValue != null,
                }
                .ToWinResult()
                ;
        }
    }
}
