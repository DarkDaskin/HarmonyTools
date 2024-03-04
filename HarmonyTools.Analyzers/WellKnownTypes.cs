using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class WellKnownTypes
{
    public readonly ITypeSymbol String;
    public readonly ITypeSymbol Type;
    public readonly ITypeSymbol ArrayOfType;
    public readonly ITypeSymbol? HarmonyPatch;
    public readonly ITypeSymbol? MethodType;
    public readonly ITypeSymbol? ArgumentType;
    public readonly ITypeSymbol? ArrayOfArgumentType;
    public readonly ITypeSymbol? PropertyMethod;

    public WellKnownTypes(Compilation compilation, string harmonyNamespace)
    {
        String = compilation.GetSpecialType(SpecialType.System_String);
        Type = compilation.GetTypeByMetadataName("System.Type")!;
        ArrayOfType = compilation.CreateArrayTypeSymbol(Type);
        HarmonyPatch = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPatch");
        MethodType = compilation.GetTypeByMetadataName($"{harmonyNamespace}.MethodType");
        ArgumentType = compilation.GetTypeByMetadataName($"{harmonyNamespace}.ArgumentType");
        ArrayOfArgumentType = ArgumentType == null ? null : compilation.CreateArrayTypeSymbol(ArgumentType);
        PropertyMethod = compilation.GetTypeByMetadataName($"{harmonyNamespace}.PropertyMethod");
    }
}