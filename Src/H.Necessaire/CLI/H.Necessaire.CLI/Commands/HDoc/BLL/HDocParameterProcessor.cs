using H.Necessaire.CLI.Commands.HDoc.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class HDocParameterProcessor
    {
        public OperationResult<HDocParameterInfo> Process(ParameterSyntax parameter)
        {
            if (parameter == null)
                return OperationResult.Fail("Parameter is NULL").WithoutPayload<HDocParameterInfo>();

            string defaultValue = parameter.Default?.Value?.ToString();

            return
                new HDocParameterInfo
                {
                    Name = parameter.Identifier.ToString(),
                    Type = parameter.Type?.ToString(),
                    IsParamsArray = parameter.IsParamsArray(),
                    IsTheExtensionMethodValue = parameter.IsTheExtensionMethodValue(),
                    DefaultsTo = defaultValue,
                    IsOptional = defaultValue != null,
                    IsOutput = parameter.IsOutput(),
                }
                .ToWinResult();
        }
    }
}
