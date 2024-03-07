using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HarmonyTools.Analyzers.HarmonyEnums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HarmonyTools.Analyzers;

internal abstract class HarmonyPatchDescription(ISymbol symbol)
{
    public abstract int HarmonyVersion { get; }

    public ISymbol Symbol { get; } = symbol;
    public ImmutableArray<SyntaxNode> AttrubuteSyntaxes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<INamedTypeSymbol?>> TargetTypes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<string?>> MethodNames { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<MethodType>> MethodTypes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<ImmutableArray<ITypeSymbol?>>> ArgumentTypes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<ImmutableArray<ArgumentType>>> ArgumentVariations { get; protected set; } = [];

    protected static HarmonyPatchDescriptionSet<TPatchDescription> Parse<TPatchDescription>(INamedTypeSymbol type, 
        Compilation compilation, string harmonyNamespace, Func<ISymbol, TPatchDescription> patchDescriptionConstructor)
        where TPatchDescription : HarmonyPatchDescription
    {
        var wellKnownTypes = new WellKnownTypes(compilation, harmonyNamespace);

        if (wellKnownTypes.HarmonyPatch is null)
            return HarmonyPatchDescriptionSet<TPatchDescription>.Empty;

        var typePatchDescription = GetTypePatchDescription(type, wellKnownTypes, patchDescriptionConstructor);

        var patchMethods = ImmutableArray.CreateBuilder<HarmonyPatchMethod<TPatchDescription>>();
        var methods = type.GetMembers().OfType<IMethodSymbol>().Where(method => method.MethodKind == MethodKind.Ordinary);
        foreach (var method in methods) 
            patchMethods.Add(GetPatchMethod(method, wellKnownTypes, patchDescriptionConstructor));

        return new HarmonyPatchDescriptionSet<TPatchDescription>(typePatchDescription, patchMethods.DrainToImmutable());
    }
    private static TPatchDescription? GetTypePatchDescription<TPatchDescription>(INamedTypeSymbol type, WellKnownTypes wellKnownTypes,
        Func<ISymbol, TPatchDescription> patchDescriptionConstructor) 
        where TPatchDescription: HarmonyPatchDescription
    {
        TPatchDescription? patchDescription = null;

        var typeAttributes = type.GetAttributes();
        foreach (var attribute in typeAttributes)
        {
            if (!attribute.Is(wellKnownTypes.HarmonyPatch))
                continue;

            patchDescription ??= patchDescriptionConstructor(type);
            patchDescription.ProcessHarmonyPatchAttribute(attribute, wellKnownTypes);
        }

        return patchDescription;
    }

    private static HarmonyPatchMethod<TPatchDescription> GetPatchMethod<TPatchDescription>(IMethodSymbol method, 
        WellKnownTypes wellKnownTypes, Func<ISymbol, TPatchDescription> patchDescriptionConstructor) 
        where TPatchDescription: HarmonyPatchDescription
    {
        TPatchDescription? patchDescription = null;
        ImmutableArray<DetailWithSyntax<PatchMethodKind>> methodKinds = [];

        var methodAttributes = method.GetAttributes();
        foreach (var attribute in methodAttributes)
        {
            if (attribute.Is(wellKnownTypes.HarmonyPatch))
            {
                patchDescription ??= patchDescriptionConstructor(method);
                patchDescription.ProcessHarmonyPatchAttribute(attribute, wellKnownTypes);
            }
            
            if (attribute.Is(wellKnownTypes.HarmonyPrefix))
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Prefix, attribute.GetSyntax()));
            if (attribute.Is(wellKnownTypes.HarmonyPostfix))
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Postfix, attribute.GetSyntax()));
            if (attribute.Is(wellKnownTypes.HarmonyTranspiler))
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Transpiler, attribute.GetSyntax()));
            if (attribute.Is(wellKnownTypes.HarmonyFinalizer))
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Finalizer, attribute.GetSyntax()));
            if (attribute.Is(wellKnownTypes.HarmonyReversePatch))
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.ReversePatch, attribute.GetSyntax()));
            if (attribute.Is(wellKnownTypes.HarmonyPrepare))
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Prepare, attribute.GetSyntax()));
            if (attribute.Is(wellKnownTypes.HarmonyCleanup))
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Cleanup, attribute.GetSyntax()));
            if (attribute.Is(wellKnownTypes.HarmonyTargetMethod))
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.TargetMethod, attribute.GetSyntax()));
            if (attribute.Is(wellKnownTypes.HarmonyTargetMethods))
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.TargetMethods, attribute.GetSyntax()));
        }

        var isHarmony2 = typeof(TPatchDescription) == typeof(HarmonyPatchDescriptionV2);
        var methodSyntax = method.GetSyntax(methodKinds.Select(detail => detail.Syntax).FirstOrDefault(syntax => syntax is not null));
        switch (method.Name)
        {
            case nameof(PatchMethodKind.Prefix):
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Prefix, methodSyntax));
                break;
            case nameof(PatchMethodKind.Postfix):
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Postfix, methodSyntax));
                break;
            case nameof(PatchMethodKind.Transpiler):
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Transpiler, methodSyntax));
                break;
            case nameof(PatchMethodKind.Finalizer) when isHarmony2:
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Finalizer, methodSyntax));
                break;
            case nameof(PatchMethodKind.ReversePatch) when isHarmony2:
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.ReversePatch, methodSyntax));
                break;
            case nameof(PatchMethodKind.Prepare):
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Prepare, methodSyntax));
                break;
            case nameof(PatchMethodKind.Cleanup):
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.Cleanup, methodSyntax));
                break;
            case nameof(PatchMethodKind.TargetMethod):
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.TargetMethod, methodSyntax));
                break;
            case nameof(PatchMethodKind.TargetMethods):
                methodKinds = methodKinds.Add(new DetailWithSyntax<PatchMethodKind>(PatchMethodKind.TargetMethods, methodSyntax));
                break;
        }

        return new HarmonyPatchMethod<TPatchDescription>(method, methodKinds, patchDescription);
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
        var values = attribute.ConstructorArguments[constructorParameterIndex].Values;
        var value = values.IsDefault ? default : values.Select(constant => (T?)constant.Value).ToImmutableArray();
        var attributeSyntax = (AttributeSyntax?)attribute.ApplicationSyntaxReference?.GetSyntax();
        var argumentSyntax = attributeSyntax?.ArgumentList?.Arguments[constructorParameterIndex];
        return new DetailWithSyntax<ImmutableArray<T?>>(value, argumentSyntax);
    }

    public Location? GetLocation() => AttrubuteSyntaxes.FirstOrDefault()?.GetLocation();

    public IEnumerable<Location> GetAdditionalLocations() => AttrubuteSyntaxes.Skip(1).Select(syntax => syntax.GetLocation());

    public virtual void Merge(HarmonyPatchDescription other)
    {
        if (other.GetType() != GetType())
            // ReSharper disable once LocalizableElement
            throw new ArgumentException("Other type does not correspond to this type.", nameof(other));

        TargetTypes = TargetTypes.AddRange(other.TargetTypes);
        MethodNames = MethodNames.AddRange(other.MethodNames);
        MethodTypes = MethodTypes.AddRange(other.MethodTypes);
        ArgumentTypes = ArgumentTypes.AddRange(other.ArgumentTypes);
        ArgumentVariations = ArgumentVariations.AddRange(other.ArgumentVariations);
    }
}