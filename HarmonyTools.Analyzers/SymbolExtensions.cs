using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HarmonyTools.Analyzers;

internal static class SymbolExtensions
{
    public static SyntaxNode? GetSyntax(this ISymbol symbol, SyntaxNode? reference = null) =>
        symbol.DeclaringSyntaxReferences
            .FirstOrDefault(syntaxReference => reference is null || syntaxReference.SyntaxTree == reference.SyntaxTree)?
            .GetSyntax();

    public static Location? GetIdentifierLocation(this SyntaxNode? syntax)
    {
        return syntax switch
        {
            BaseTypeDeclarationSyntax typeDeclarationSyntax => typeDeclarationSyntax.Identifier.GetLocation(),
            MethodDeclarationSyntax methodDeclarationSyntax => methodDeclarationSyntax.Identifier.GetLocation(),
            PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax.Identifier.GetLocation(),
            _ => null
        };
    }
}