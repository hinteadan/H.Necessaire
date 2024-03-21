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
    }
}
