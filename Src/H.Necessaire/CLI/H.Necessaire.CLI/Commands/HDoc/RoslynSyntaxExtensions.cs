using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace H.Necessaire.CLI.Commands.HDoc
{
    public static class RoslynSyntaxExtensions
    {
        public static bool IsPublic(this MemberDeclarationSyntax memberDeclaration)
        {
            return memberDeclaration?.Modifiers.Any(m => m.ToString() == "public") == true;
        }

        public static bool IsStatic(this MemberDeclarationSyntax memberDeclaration)
        {
            return memberDeclaration?.Modifiers.Any(m => m.ToString() == "static") == true;
        }

        public static bool IsParamsArray(this BaseParameterSyntax memberDeclaration)
        {
            return memberDeclaration?.Modifiers.Any(m => m.ToString() == "params") == true;
        }

        public static bool IsTheExtensionMethodValue(this BaseParameterSyntax memberDeclaration)
        {
            return memberDeclaration?.Modifiers.Any(m => m.ToString() == "this") == true;
        }

        public static bool IsOutput(this BaseParameterSyntax memberDeclaration)
        {
            return memberDeclaration?.Modifiers.Any(m => m.ToString() == "out") == true;
        }
    }
}
