using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchDescriptionV1 : HarmonyPatchDescription
{
    public override int HarmonyVersion => 1;

    public static HarmonyPatchDescriptionV1? Parse(INamedTypeSymbol type, Compilation compilation) => 
        Parse<HarmonyPatchDescriptionV1>(type, compilation, "Harmony");
}