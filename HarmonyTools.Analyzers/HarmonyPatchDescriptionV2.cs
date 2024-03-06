using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchDescriptionV2 : HarmonyPatchDescription
{
    public override int HarmonyVersion => 2;

    public ImmutableArray<DetailWithSyntax<string?>> TargetTypeNames { get; private set; } = [];

    public static HarmonyPatchDescriptionV2? Parse(INamedTypeSymbol type, Compilation compilation) => 
        Parse<HarmonyPatchDescriptionV2>(type, compilation, "HarmonyLib");

    protected override void ProcessHarmonyPatchAttribute(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        base.ProcessHarmonyPatchAttribute(attribute, wellKnownTypes);

        if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.String, wellKnownTypes.String, wellKnownTypes.MethodType!))
        {
            TargetTypeNames = TargetTypeNames.Add(GetDetailWithSyntax<string?>(attribute, 0));
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 1));
        }
    }
}