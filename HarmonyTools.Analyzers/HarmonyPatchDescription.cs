using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HarmonyTools.Analyzers.HarmonyEnums;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal abstract class HarmonyPatchDescription(ISymbol symbol)
{
    public abstract int HarmonyVersion { get; }

    public ISymbol Symbol { get; } = symbol;

    /// <summary>Determines whether this description defines a patch.</summary>
    /// <remarks>Set to <c>true</c> when there is at least one attribure deriving from <c>HarmonyAttribute</c> at type level.</remarks>
    public bool IsDefining { get; private set; }

    public IMethodSymbol? TargetMethod { get; set; }

    public ImmutableList<AttributeData> Attrubutes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<ITypeSymbol?>> TargetTypes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<string?>> MethodNames { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<MethodType>> MethodTypes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<ImmutableArray<ITypeSymbol?>>> ArgumentTypes { get; protected set; } = [];
    public ImmutableArray<DetailWithSyntax<ImmutableArray<ArgumentType>>> ArgumentVariations { get; protected set; } = [];
    public DetailWithSyntax<bool>? IsPatchAll { get; private set; }    
    public DetailWithSyntax<int>? Priority { get; private set; }
    public DetailWithSyntax<ImmutableArray<string?>>? Before { get; private set; }
    public DetailWithSyntax<ImmutableArray<string?>>? After { get; private set; }
    public DetailWithSyntax<bool>? IsDebug { get; private set; }
    public ImmutableArray<HarmonyArgument> ArgumentOverrides { get; protected set; } = [];

    protected static HarmonyPatchDescriptionSet<TPatchDescription> Parse<TPatchDescription>(INamedTypeSymbol type, 
        WellKnownTypes wellKnownTypes, Func<ISymbol, TPatchDescription> patchDescriptionConstructor)
        where TPatchDescription : HarmonyPatchDescription
    {
        var typePatchDescription = GetTypePatchDescription(type, wellKnownTypes, patchDescriptionConstructor);

        var patchMethods = ImmutableArray.CreateBuilder<HarmonyPatchMethod<TPatchDescription>>();
        var methods = type.GetMembers().OfType<IMethodSymbol>().Where(method => method.MethodKind == MethodKind.Ordinary);
        foreach (var method in methods)
        {
            var patchMethod = GetPatchMethod(method, wellKnownTypes, patchDescriptionConstructor);
            if (patchMethod is not null)
                patchMethods.Add(patchMethod);
        }

        return new HarmonyPatchDescriptionSet<TPatchDescription>(typePatchDescription, patchMethods.DrainToImmutable());
    }
    private static TPatchDescription? GetTypePatchDescription<TPatchDescription>(INamedTypeSymbol type, WellKnownTypes wellKnownTypes,
        Func<ISymbol, TPatchDescription> patchDescriptionConstructor) 
        where TPatchDescription: HarmonyPatchDescription
    {
        TPatchDescription? patchDescription = null;

        var typeAttributes = type.GetAttributes();
        foreach (var attribute in typeAttributes)
            MaybeFillPatchDescription(ref patchDescription, type, attribute, wellKnownTypes, patchDescriptionConstructor);

        return patchDescription;
    }

    private static HarmonyPatchMethod<TPatchDescription>? GetPatchMethod<TPatchDescription>(IMethodSymbol method, 
        WellKnownTypes wellKnownTypes, Func<ISymbol, TPatchDescription> patchDescriptionConstructor) 
        where TPatchDescription: HarmonyPatchDescription
    {
        TPatchDescription? patchDescription = null;
        ImmutableArray<DetailWithSyntax<PatchMethodKind>> methodKinds = [];

        var methodAttributes = method.GetAttributes();
        foreach (var attribute in methodAttributes)
        {
            MaybeFillPatchDescription(ref patchDescription, method, attribute, wellKnownTypes, patchDescriptionConstructor);

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

        if (methodKinds is [] && patchDescription is null)
            return null;

        return new HarmonyPatchMethod<TPatchDescription>(method, methodKinds, patchDescription);
    }

    private static void MaybeFillPatchDescription<TPatchDescription>(ref TPatchDescription? patchDescription, ISymbol symbol,
        AttributeData attribute, WellKnownTypes wellKnownTypes, Func<ISymbol, TPatchDescription> patchDescriptionConstructor)
        where TPatchDescription : HarmonyPatchDescription
    {
        if (attribute.Is(wellKnownTypes.HarmonyAttribute) || attribute.Is(wellKnownTypes.HarmonyArgument))
        {
            patchDescription ??= patchDescriptionConstructor(symbol);
            patchDescription.ProcessAttribute(attribute, wellKnownTypes);
        }
    }

    protected virtual void ProcessAttribute(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        Attrubutes = Attrubutes.Add(attribute);

        if (attribute.Is(wellKnownTypes.HarmonyAttribute))
            IsDefining = Symbol is INamedTypeSymbol;

        if (attribute.Is(wellKnownTypes.HarmonyPatch))
            ProcessHarmonyPatchAttribute(attribute, wellKnownTypes);
        else if (attribute.Is(wellKnownTypes.HarmonyPatchAll))
            IsPatchAll = new DetailWithSyntax<bool>(true, attribute.GetSyntax());
        else if (attribute.Is(wellKnownTypes.HarmonyPriority))
            Priority = attribute.GetDetailWithSyntax<int>(0);
        else if (attribute.Is(wellKnownTypes.HarmonyBefore))
            Before = attribute.GetDetailWithSyntaxForArray<string?>(0);
        else if (attribute.Is(wellKnownTypes.HarmonyAfter))
            After = attribute.GetDetailWithSyntaxForArray<string?>(0);
        else if (attribute.Is(wellKnownTypes.HarmonyDebug))
            IsDebug = new DetailWithSyntax<bool>(true, attribute.GetSyntax());
        else if (attribute.Is(wellKnownTypes.HarmonyArgument))
        {
            var harmonyArgument = HarmonyArgument.Parse(attribute, wellKnownTypes);
            if (harmonyArgument is not null)
                ArgumentOverrides = ArgumentOverrides.Add(harmonyArgument);
        }
    }

    protected virtual void ProcessHarmonyPatchAttribute(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        if (attribute.IsMatch(wellKnownTypes.Type))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.ArrayOfType))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(1));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.String))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(1));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.String, wellKnownTypes.ArrayOfType))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(1));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(2));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.String, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(1));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(2));
            ArgumentVariations = ArgumentVariations.Add(attribute.GetDetailWithSyntaxForArray<ArgumentType>(3));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.MethodType!))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodTypes = MethodTypes.Add(attribute.GetDetailWithSyntax<MethodType>(1));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.MethodType!, wellKnownTypes.ArrayOfType))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodTypes = MethodTypes.Add(attribute.GetDetailWithSyntax<MethodType>(1));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(2));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.MethodType!, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodTypes = MethodTypes.Add(attribute.GetDetailWithSyntax<MethodType>(1));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(2));
            ArgumentVariations = ArgumentVariations.Add(attribute.GetDetailWithSyntaxForArray<ArgumentType>(3));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.String, wellKnownTypes.MethodType!))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(1));
            MethodTypes = MethodTypes.Add(attribute.GetDetailWithSyntax<MethodType>(2));
        }
        else if (attribute.IsMatch(wellKnownTypes.String))
        {
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(0));
        }
        else if (attribute.IsMatch(wellKnownTypes.String, wellKnownTypes.ArrayOfType))
        {
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(0));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(1));
        }
        else if (attribute.IsMatch(wellKnownTypes.String, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(0));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(1));
            ArgumentVariations = ArgumentVariations.Add(attribute.GetDetailWithSyntaxForArray<ArgumentType>(2));
        }
        else if (attribute.IsMatch(wellKnownTypes.String, wellKnownTypes.MethodType!))
        {
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(0));
            MethodTypes = MethodTypes.Add(attribute.GetDetailWithSyntax<MethodType>(1));
        }
        else if (attribute.IsMatch(wellKnownTypes.MethodType!))
        {
            MethodTypes = MethodTypes.Add(attribute.GetDetailWithSyntax<MethodType>(0));
        }
        else if (attribute.IsMatch(wellKnownTypes.MethodType!, wellKnownTypes.ArrayOfType))
        {
            MethodTypes = MethodTypes.Add(attribute.GetDetailWithSyntax<MethodType>(0));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(1));
        }
        else if (attribute.IsMatch(wellKnownTypes.MethodType!, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            MethodTypes = MethodTypes.Add(attribute.GetDetailWithSyntax<MethodType>(0));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(1));
            ArgumentVariations = ArgumentVariations.Add(attribute.GetDetailWithSyntaxForArray<ArgumentType>(2));
        }
        else if (attribute.IsMatch(wellKnownTypes.ArrayOfType))
        {
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(0));
        }
        else if (attribute.IsMatch(wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(0));
            ArgumentVariations = ArgumentVariations.Add(attribute.GetDetailWithSyntaxForArray<ArgumentType>(1));
        }
    }

    public Location? GetLocation(Func<AttributeData, bool>? predicate = null) => GetAttributeSyntaxes(predicate).GetLocation();

    public IEnumerable<Location> GetAdditionalLocations(Func<AttributeData, bool>? predicate = null) =>
        GetAttributeSyntaxes(predicate).GetAdditionalLocations();

    private IEnumerable<IHasSyntax> GetAttributeSyntaxes(Func<AttributeData, bool>? predicate) => 
        Attrubutes.Where(attribute => predicate is null || predicate(attribute)).Select(attribute => new SyntaxWrapper(attribute));

    public virtual void Merge(HarmonyPatchDescription other)
    {
        if (other.GetType() != GetType())
            // ReSharper disable once LocalizableElement
            throw new ArgumentException("Other type does not correspond to this type.", nameof(other));

        IsDefining |= other.IsDefining;

        // Rather than merging two arrays, use second array as fallback if the first one is empty.
        // Thus, it allows to override type-level annotations at method level.
        TargetTypes = UseFallback(TargetTypes, other.TargetTypes);
        MethodNames = UseFallback(MethodNames, other.MethodNames);
        MethodTypes = UseFallback(MethodTypes, other.MethodTypes);
        ArgumentTypes = UseFallback(ArgumentTypes, other.ArgumentTypes);
        ArgumentVariations = UseFallback(ArgumentVariations, other.ArgumentVariations);
        IsPatchAll ??= other.IsPatchAll;
        Priority ??= other.Priority;
        Before ??= other.Before;
        After ??= other.After;
        IsDebug ??= other.IsDebug;

        ArgumentOverrides = ArgumentOverrides.AddRange(other.ArgumentOverrides);
    }
    
    protected static ImmutableArray<T> UseFallback<T>(ImmutableArray<T> first, ImmutableArray<T> second) =>
        first.IsDefaultOrEmpty ? second : first;
}