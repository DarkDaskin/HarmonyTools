using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HarmonyTools.Analyzers;

internal abstract class HarmonyPatchDescription
{
    public abstract int HarmonyVersion { get; }

    public ImmutableArray<DetailWithSyntax<INamedTypeSymbol?>> PatchedTypes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<string?>> PatchedMethodNames { get; protected set; } = [];

    protected static TPatchDescription? Parse<TPatchDescription>(INamedTypeSymbol type, Compilation compilation, string harmonyNamespace)
        where TPatchDescription : HarmonyPatchDescription,  new()
    {
        var wellKnownTypes = new WellKnownTypes(compilation, harmonyNamespace);

        TPatchDescription? pd = null;

        var attributes = type.GetAttributes();
        foreach (var attribute in attributes)
        {
            if (!(attribute.AttributeClass?.Equals(wellKnownTypes.HarmonyPatchAttribute, SymbolEqualityComparer.Default) ?? false))
                continue;

            pd ??= new TPatchDescription();

            pd.ProcessHarmonyPatchAttribute(attribute, wellKnownTypes);
        }

        return pd;
    }

    protected virtual void ProcessHarmonyPatchAttribute(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type))
        {
            PatchedTypes = PatchedTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
        }

        if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.String))
        {
            PatchedTypes = PatchedTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            PatchedMethodNames = PatchedMethodNames.Add(GetDetailWithSyntax<string?>(attribute, 1));
        }

        if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.String))
        {
            PatchedMethodNames = PatchedMethodNames.Add(GetDetailWithSyntax<string?>(attribute, 0));
        }
    }

    protected static bool IsMatch(IMethodSymbol? method, params ITypeSymbol[] argumentTypes)
    {
        if (method is null || method.Parameters.Length != argumentTypes.Length)
            return false;

        for (var i = 0; i < argumentTypes.Length; i++)
            if (!method.Parameters[i].Type.Equals(argumentTypes[i], SymbolEqualityComparer.Default))
                return false;

        return true;
    }

    protected static DetailWithSyntax<T?> GetDetailWithSyntax<T>(AttributeData attribute, int constructorParameterIndex)
    {
        var value = (T?)attribute.ConstructorArguments[constructorParameterIndex].Value;
        var attributeSyntax = (AttributeSyntax?)attribute.ApplicationSyntaxReference?.GetSyntax();
        var argumentSyntax = attributeSyntax?.ArgumentList?.Arguments[constructorParameterIndex];
        return new DetailWithSyntax<T?>(value, argumentSyntax);
    }
}