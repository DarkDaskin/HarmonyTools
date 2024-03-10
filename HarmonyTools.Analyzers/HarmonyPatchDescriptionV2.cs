using System.Collections.Immutable;
using HarmonyTools.Analyzers.HarmonyEnums;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchDescriptionV2(ISymbol symbol) : HarmonyPatchDescription(symbol)
{
    public override int HarmonyVersion => 2;

    public ImmutableArray<DetailWithSyntax<string?>> TargetTypeNames { get; private set; } = [];
    public ImmutableArray<DetailWithSyntax<MethodDispatchType>> MethodDispatchTypes { get; private set; } = [];
    
    public static HarmonyPatchDescriptionSet<HarmonyPatchDescriptionV2> Parse(INamedTypeSymbol type, WellKnownTypes wellKnownTypes) =>
        Parse(type, wellKnownTypes, symbol => new HarmonyPatchDescriptionV2(symbol));

    protected override void ProcessHarmonyPatchAttribute(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        base.ProcessHarmonyPatchAttribute(attribute, wellKnownTypes);

        if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.String, wellKnownTypes.String, wellKnownTypes.MethodType!))
        {
            TargetTypeNames = TargetTypeNames.Add(GetDetailWithSyntax<string?>(attribute, 0));
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 1));
        }
        // All below is for HarmonyDelegate.
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.MethodDispatchType!))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodDispatchTypes = MethodDispatchTypes.Add(GetDetailWithSyntax<MethodDispatchType>(attribute, 1));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.MethodDispatchType!, wellKnownTypes.ArrayOfType))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodDispatchTypes = MethodDispatchTypes.Add(GetDetailWithSyntax<MethodDispatchType>(attribute, 1));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol?>(attribute, 2));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.MethodDispatchType!, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodDispatchTypes = MethodDispatchTypes.Add(GetDetailWithSyntax<MethodDispatchType>(attribute, 1));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol?>(attribute, 2));
            ArgumentVariations = ArgumentVariations.Add(GetDetailWithSyntaxForArray<ArgumentType>(attribute, 3));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.Type, wellKnownTypes.String, wellKnownTypes.MethodDispatchType!))
        {
            TargetTypes = TargetTypes.Add(GetDetailWithSyntax<INamedTypeSymbol?>(attribute, 0));
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 1));
            MethodDispatchTypes = MethodDispatchTypes.Add(GetDetailWithSyntax<MethodDispatchType>(attribute, 2));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.String, wellKnownTypes.MethodDispatchType!))
        {
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 0));
            MethodDispatchTypes = MethodDispatchTypes.Add(GetDetailWithSyntax<MethodDispatchType>(attribute, 1));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.MethodDispatchType!))
        {
            MethodDispatchTypes = MethodDispatchTypes.Add(GetDetailWithSyntax<MethodDispatchType>(attribute, 0));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.MethodDispatchType!, wellKnownTypes.ArrayOfType))
        {
            MethodDispatchTypes = MethodDispatchTypes.Add(GetDetailWithSyntax<MethodDispatchType>(attribute, 0));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol?>(attribute, 1));
        }
        else if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.MethodDispatchType!, wellKnownTypes.ArrayOfType, wellKnownTypes.ArrayOfArgumentType!))
        {
            MethodDispatchTypes = MethodDispatchTypes.Add(GetDetailWithSyntax<MethodDispatchType>(attribute, 0));
            ArgumentTypes = ArgumentTypes.Add(GetDetailWithSyntaxForArray<ITypeSymbol?>(attribute, 1));
            ArgumentVariations = ArgumentVariations.Add(GetDetailWithSyntaxForArray<ArgumentType>(attribute, 2));
        }
    }

    public override void Merge(HarmonyPatchDescription other)
    {
        base.Merge(other);

        var otherV2 = (HarmonyPatchDescriptionV2)other;
        TargetTypeNames = TargetTypeNames.AddRange(otherV2.TargetTypeNames);
        MethodDispatchTypes = MethodDispatchTypes.AddRange(otherV2.MethodDispatchTypes);
    }
}