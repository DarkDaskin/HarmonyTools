﻿using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal static class AttributeDataExtensions
{
    public static bool Is(this AttributeData attribute, ITypeSymbol? type) =>
        attribute.AttributeClass?.Equals(type, SymbolEqualityComparer.Default) ?? false;

    public static SyntaxNode? GetSyntax(this AttributeData attribute) => attribute.ApplicationSyntaxReference?.GetSyntax();
}