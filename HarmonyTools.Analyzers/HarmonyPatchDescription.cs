using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HarmonyTools.Analyzers;

internal abstract class HarmonyPatchDescription
{
    public abstract int HarmonyVersion { get; }

    public ImmutableArray<SyntaxNode> AttrubuteSyntaxes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<INamedTypeSymbol?>> TargetTypes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<string?>> MethodNames { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<MethodType>> MethodTypes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<ImmutableArray<ITypeSymbol?>>> ArgumentTypes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<ImmutableArray<ArgumentType>>> ArgumentVariations { get; protected set; } = [];

    protected static TPatchDescription? Parse<TPatchDescription>(INamedTypeSymbol type, Compilation compilation, string harmonyNamespace)
        where TPatchDescription : HarmonyPatchDescription,  new()
    {
        var wellKnownTypes = new WellKnownTypes(compilation, harmonyNamespace);

        TPatchDescription? pd = null;

        var attributes = type.GetAttributes();
        foreach (var attribute in attributes)
        {
            if (!(attribute.AttributeClass?.Equals(wellKnownTypes.HarmonyPatch, SymbolEqualityComparer.Default) ?? false))
                continue;

            pd ??= new TPatchDescription();

            pd.ProcessHarmonyPatchAttribute(attribute, wellKnownTypes);
        }

        return pd;
    }

    protected virtual void ProcessHarmonyPatchAttribute(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        if (attribute.ApplicationSyntaxReference is not null)
            AttrubuteSyntaxes = AttrubuteSyntaxes.Add(attribute.ApplicationSyntaxReference.GetSyntax());

        if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.ArrayOfType))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 1));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.String))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 1));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.String, wellKnownTypes.ArrayOfType))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 1));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 2));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.String, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 1));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 2));
            ArgumentVariations = ArgumentVariations.Add(GetDetailWithSyntaxForArray<ArgumentType>(attribute, 3));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.MethodType!))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodTypes = MethodTypes.Add(GetDetailWithSyntax<MethodType>(attribute, 1));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.MethodType!, wellKnownTypes.ArrayOfType))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodTypes = MethodTypes.Add(GetDetailWithSyntax<MethodType>(attribute, 1));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 2));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.MethodType!, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodTypes = MethodTypes.Add(GetDetailWithSyntax<MethodType>(attribute, 1));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 2));
            ArgumentVariations = ArgumentVariations.Add(GetDetailWithSyntaxForArray<ArgumentType>(attribute, 3));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.String, wellKnownTypes.MethodType!))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 1));
            MethodTypes = MethodTypes.Add(GetDetailWithSyntax<MethodType>(attribute, 2));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.String))
        {
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 0));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.String, wellKnownTypes.ArrayOfType))
        {
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 0));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 1));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.String, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 0));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 1));
            ArgumentVariations = ArgumentVariations.Add(GetDetailWithSyntaxForArray<ArgumentType>(attribute, 2));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.String, wellKnownTypes.MethodType!))
        {
           MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 0));
           MethodTypes = MethodTypes.Add(GetDetailWithSyntax<MethodType>(attribute, 1));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.MethodType!))
        {
            MethodTypes = MethodTypes.Add(GetDetailWithSyntax<MethodType>(attribute, 0));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.MethodType!, wellKnownTypes.ArrayOfType))
        {
            MethodTypes = MethodTypes.Add(GetDetailWithSyntax<MethodType>(attribute, 0));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 1));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.MethodType!, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            MethodTypes = MethodTypes.Add(GetDetailWithSyntax<MethodType>(attribute, 0));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 1));
            ArgumentVariations = ArgumentVariations.Add(GetDetailWithSyntaxForArray<ArgumentType>(attribute, 2));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.ArrayOfType))
        {
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 0));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol>(attribute, 0));
            ArgumentVariations = ArgumentVariations.Add(GetDetailWithSyntaxForArray<ArgumentType>(attribute, 1));
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

    protected static DetailWithSyntax<ImmutableArray<T?>> GetDetailWithSyntaxForArray<T>(AttributeData attribute, int constructorParameterIndex)
    {
        var value = attribute.ConstructorArguments[constructorParameterIndex].Values
           .Select(constant => (T?) constant.Value)
           .ToImmutableArray();
        var attributeSyntax = (AttributeSyntax?)attribute.ApplicationSyntaxReference?.GetSyntax();
        var argumentSyntax = attributeSyntax?.ArgumentList?.Arguments[constructorParameterIndex];
        return new DetailWithSyntax<ImmutableArray<T?>>(value, argumentSyntax);
    }

    public Location? GetLocation() => AttrubuteSyntaxes.FirstOrDefault()?.GetLocation();

    public IEnumerable<Location> GetAdditionalLocations() => AttrubuteSyntaxes.Skip(1).Select(syntax => syntax.GetLocation());
}