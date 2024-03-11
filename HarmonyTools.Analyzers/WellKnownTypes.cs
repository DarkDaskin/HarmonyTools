using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class WellKnownTypes
{
    public const string Harmony1Namespace = "Harmony";
    public const string Harmony2Namespace = "HarmonyLib";

    public readonly ITypeSymbol Int32;
    public readonly ITypeSymbol String;
    public readonly ITypeSymbol Type;
    public readonly ITypeSymbol ArrayOfType;
    public readonly ITypeSymbol? HarmonyAttribute;
    public readonly ITypeSymbol? HarmonyPatch;
    public readonly ITypeSymbol? HarmonyPatchAll;
    public readonly ITypeSymbol? HarmonyPatchCategory;
    public readonly ITypeSymbol? HarmonyDelegate;
    public readonly ITypeSymbol? HarmonyPriority;
    public readonly ITypeSymbol? HarmonyBefore;
    public readonly ITypeSymbol? HarmonyAfter;
    public readonly ITypeSymbol? HarmonyDebug;
    public readonly ITypeSymbol? HarmonyPrefix;
    public readonly ITypeSymbol? HarmonyPostfix;
    public readonly ITypeSymbol? HarmonyTranspiler;
    public readonly ITypeSymbol? HarmonyPrepare;
    public readonly ITypeSymbol? HarmonyCleanup;
    public readonly ITypeSymbol? HarmonyTargetMethod;
    public readonly ITypeSymbol? HarmonyTargetMethods;
    public readonly ITypeSymbol? HarmonyFinalizer;
    public readonly ITypeSymbol? HarmonyReversePatch;
    public readonly ITypeSymbol? HarmonyArgument;
    public readonly ITypeSymbol? MethodType;
    public readonly ITypeSymbol? ArgumentType;
    public readonly ITypeSymbol? ArrayOfArgumentType;
    public readonly ITypeSymbol? PropertyMethod;
    public readonly ITypeSymbol? MethodDispatchType;

    public WellKnownTypes(Compilation compilation, string harmonyNamespace)
    {
        Int32 = compilation.GetSpecialType(SpecialType.System_Int32);
        String = compilation.GetSpecialType(SpecialType.System_String);
        Type = compilation.GetTypeByMetadataName("System.Type")!;
        ArrayOfType = compilation.CreateArrayTypeSymbol(Type);
        HarmonyAttribute = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyAttribute");
        HarmonyPatch = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPatch");
        HarmonyPatchAll = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPatchAll");
        HarmonyPatchCategory = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPatchCategory");
        HarmonyDelegate = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyDelegate");
        HarmonyPriority = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPriority");
        HarmonyBefore = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyBefore");
        HarmonyAfter = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyAfter");
        HarmonyDebug = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyDebug");
        HarmonyPrefix = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPrefix");
        HarmonyPostfix = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPostfix");
        HarmonyTranspiler = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyTranspiler");
        HarmonyPrepare = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPrepare");
        HarmonyCleanup = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyCleanup");
        HarmonyTargetMethod = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyTargetMethod");
        HarmonyTargetMethods = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyTargetMethods");
        HarmonyFinalizer = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyFinalizer");
        HarmonyReversePatch = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyReversePatch");
        HarmonyArgument = compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyArgument");
        MethodType = compilation.GetTypeByMetadataName($"{harmonyNamespace}.MethodType");
        ArgumentType = compilation.GetTypeByMetadataName($"{harmonyNamespace}.ArgumentType");
        ArrayOfArgumentType = ArgumentType == null ? null : compilation.CreateArrayTypeSymbol(ArgumentType);
        PropertyMethod = compilation.GetTypeByMetadataName($"{harmonyNamespace}.PropertyMethod");
        MethodDispatchType = compilation.GetTypeByMetadataName($"{harmonyNamespace}.MethodDispatchType");
    }

    public static bool IsHarmonyLoaded(Compilation compilation, string harmonyNamespace) =>
        compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPatch") is not null;
}