using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal static class AttributeDataExtensions
{
    public static bool Is(this AttributeData attribute, ITypeSymbol? type)
    {
        for (var attributeType = attribute.AttributeClass; attributeType is not null; attributeType = attributeType.BaseType)
            if (attributeType.Equals(type, SymbolEqualityComparer.Default))
                return true;

        return false;
    }

    public static SyntaxNode? GetSyntax(this AttributeData attribute) => attribute.ApplicationSyntaxReference?.GetSyntax();
}