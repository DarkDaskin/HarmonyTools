using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchDescriptionV2(ISymbol symbol) : HarmonyPatchDescription(symbol)
{
    public override int HarmonyVersion => 2;

    public ImmutableArray<DetailWithSyntax<string?>> TargetTypeNames { get; private set; } = [];
    
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
    }

    public override void Merge(HarmonyPatchDescription other)
    {
        base.Merge(other);

        var otherV2 = (HarmonyPatchDescriptionV2)other;
        TargetTypeNames = TargetTypeNames.AddRange(otherV2.TargetTypeNames);
    }
}