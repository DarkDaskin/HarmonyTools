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

    public static bool Is(this INamedTypeSymbol? type, ITypeSymbol? otherType)
    {
        for (var currentType = type; currentType is not null; currentType = currentType.BaseType)
            if (currentType.Equals(otherType, SymbolEqualityComparer.Default))
                return true;

        return false;
    }

    public static bool IsMatch(this IMethodSymbol? method, params ITypeSymbol[] argumentTypes)
    {
        if (method is null || method.Parameters.Length != argumentTypes.Length)
            return false;

        for (var i = 0; i < argumentTypes.Length; i++)
            if (!method.Parameters[i].Type.Equals(argumentTypes[i], SymbolEqualityComparer.Default))
                return false;

        return true;
    }
}