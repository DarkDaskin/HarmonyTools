using System;
using System.Collections.Immutable;
using HarmonyTools.Analyzers.HarmonyEnums;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchDescriptionV2(ISymbol symbol) : HarmonyPatchDescription(symbol)
{
    public override int HarmonyVersion => 2;

    public ImmutableArray<DetailWithSyntax<string?>> TargetTypeNames { get; private set; } = [];
    public ImmutableArray<DetailWithSyntax<MethodDispatchType>> MethodDispatchTypes { get; private set; } = [];
    public DetailWithSyntax<string?>? PatchCategory { get; private set; }

    public static HarmonyPatchDescriptionSet<HarmonyPatchDescriptionV2> Parse(INamedTypeSymbol type, WellKnownTypes wellKnownTypes) =>
        Parse(type, wellKnownTypes, symbol => new HarmonyPatchDescriptionV2(symbol));

    protected override void ProcessAttribute(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        base.ProcessAttribute(attribute, wellKnownTypes);

        if (attribute.Is(wellKnownTypes.HarmonyPatchCategory)) 
            PatchCategory = attribute.GetDetailWithSyntax<string?>(0);
    }

    protected override void ProcessHarmonyPatchAttribute(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        base.ProcessHarmonyPatchAttribute(attribute, wellKnownTypes);

        if (attribute.IsMatch(wellKnownTypes.String, wellKnownTypes.String, wellKnownTypes.MethodType!))
        {
            TargetTypeNames = TargetTypeNames.Add(attribute.GetDetailWithSyntax<string?>(0));
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(1));
        }
        // All below is for HarmonyDelegate.
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.MethodDispatchType!))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodDispatchTypes = MethodDispatchTypes.Add(attribute.GetDetailWithSyntax<MethodDispatchType>(1));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.MethodDispatchType!, wellKnownTypes.ArrayOfType))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodDispatchTypes = MethodDispatchTypes.Add(attribute.GetDetailWithSyntax<MethodDispatchType>(1));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(2));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.MethodDispatchType!, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodDispatchTypes = MethodDispatchTypes.Add(attribute.GetDetailWithSyntax<MethodDispatchType>(1));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(2));
            ArgumentVariations = ArgumentVariations.Add(attribute.GetDetailWithSyntaxForArray<ArgumentType>(3));
        }
        else if (attribute.IsMatch(wellKnownTypes.Type, wellKnownTypes.String, wellKnownTypes.MethodDispatchType!))
        {
            TargetTypes = TargetTypes.Add(attribute.GetDetailWithSyntax<ITypeSymbol?>(0));
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(1));
            MethodDispatchTypes = MethodDispatchTypes.Add(attribute.GetDetailWithSyntax<MethodDispatchType>(2));
        }
        else if (attribute.IsMatch(wellKnownTypes.String, wellKnownTypes.MethodDispatchType!))
        {
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(0));
            MethodDispatchTypes = MethodDispatchTypes.Add(attribute.GetDetailWithSyntax<MethodDispatchType>(1));
        }
        else if (attribute.IsMatch(wellKnownTypes.MethodDispatchType!))
        {
            MethodDispatchTypes = MethodDispatchTypes.Add(attribute.GetDetailWithSyntax<MethodDispatchType>(0));
        }
        else if (attribute.IsMatch(wellKnownTypes.MethodDispatchType!, wellKnownTypes.ArrayOfType))
        {
            MethodDispatchTypes = MethodDispatchTypes.Add(attribute.GetDetailWithSyntax<MethodDispatchType>(0));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(1));
        }
        else if (attribute.IsMatch(wellKnownTypes.MethodDispatchType!, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            MethodDispatchTypes = MethodDispatchTypes.Add(attribute.GetDetailWithSyntax<MethodDispatchType>(0));
            ArgumentTypes = ArgumentTypes.Add(attribute.GetDetailWithSyntaxForArray<ITypeSymbol?>(1));
            ArgumentVariations = ArgumentVariations.Add(attribute.GetDetailWithSyntaxForArray<ArgumentType>(2));
        }
    }

    public override void Merge(HarmonyPatchDescription other)
    {
        base.Merge(other);

        var otherV2 = (HarmonyPatchDescriptionV2)other;
        TargetTypeNames = UseFallback(TargetTypeNames, otherV2.TargetTypeNames);
        MethodDispatchTypes = UseFallback(MethodDispatchTypes, otherV2.MethodDispatchTypes);
        PatchCategory ??= otherV2.PatchCategory;
    }
}