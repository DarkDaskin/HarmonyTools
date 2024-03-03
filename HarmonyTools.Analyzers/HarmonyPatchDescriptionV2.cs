using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchDescriptionV2 : HarmonyPatchDescription
{
    public override int HarmonyVersion => 2;

    public static HarmonyPatchDescriptionV2? Parse(INamedTypeSymbol type, Compilation compilation) => 
        Parse<HarmonyPatchDescriptionV2>(type, compilation, "HarmonyLib");
}