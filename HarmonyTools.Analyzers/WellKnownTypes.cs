using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class WellKnownTypes(Compilation compilation, string harmonyNamespace)
{
    public readonly ITypeSymbol String = compilation.GetSpecialType(SpecialType.System_String);
    public readonly ITypeSymbol Type = compilation.GetTypeByMetadataName("System.Type")!;
    public readonly ITypeSymbol HarmonyPatchAttribute = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPatch")!;
}