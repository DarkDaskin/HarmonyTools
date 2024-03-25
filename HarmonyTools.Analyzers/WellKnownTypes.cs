using System.Linq;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class WellKnownTypes
{
    public const string Harmony1Namespace = "Harmony";
    public const string Harmony2Namespace = "HarmonyLib";

    private static string[] _allHarmonyNamespaces = [Harmony1Namespace, Harmony2Namespace];

    public readonly ITypeSymbol Object;
    public readonly ITypeSymbol ArrayOfObject;
    public readonly ITypeSymbol Void;
    public readonly ITypeSymbol Boolean;
    public readonly ITypeSymbol Int32;
    public readonly ITypeSymbol String;
    public readonly ITypeSymbol Type;
    public readonly ITypeSymbol ArrayOfType;
    public readonly ITypeSymbol Exception;
    public readonly ITypeSymbol Delegate;
    public readonly ITypeSymbol MethodBase;
    public readonly ITypeSymbol EnumerableOfMethodBase;
    // ReSharper disable once InconsistentNaming
    public readonly ITypeSymbol ILGenerator;
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
    public readonly ITypeSymbol? CodeInstruction;
    public readonly ITypeSymbol? EnumerableOfCodeInstruction;
    public readonly ITypeSymbol? HarmonyInstance;
    public readonly INamedTypeSymbol? RefResultOfT;

    public WellKnownTypes(Compilation compilation, string harmonyNamespace)
    {
        Object = compilation.ObjectType.WithNullableAnnotation(NullableAnnotation.Annotated);
        ArrayOfObject = compilation.CreateArrayTypeSymbol(Object, elementNullableAnnotation: NullableAnnotation.Annotated)
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        Void = compilation.GetSpecialType(SpecialType.System_Void);
        Boolean = compilation.GetSpecialType(SpecialType.System_Boolean);
        Int32 = compilation.GetSpecialType(SpecialType.System_Int32);
        String = compilation.GetSpecialType(SpecialType.System_String)
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        Type = compilation.GetTypeByMetadataName("System.Type")!
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        ArrayOfType = compilation.CreateArrayTypeSymbol(Type)
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        Exception = compilation.GetTypeByMetadataName("System.Exception")!
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        Delegate = compilation.GetTypeByMetadataName("System.Delegate")!
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        MethodBase = compilation.GetTypeByMetadataName("System.Reflection.MethodBase")!
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        EnumerableOfMethodBase = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
            .Construct(MethodBase).WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        ILGenerator = compilation.GetTypeByMetadataName("System.Reflection.Emit.ILGenerator")!
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
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
        ArrayOfArgumentType = ArgumentType == null ? null : compilation.CreateArrayTypeSymbol(ArgumentType)
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        PropertyMethod = compilation.GetTypeByMetadataName($"{harmonyNamespace}.PropertyMethod");
        MethodDispatchType = compilation.GetTypeByMetadataName($"{harmonyNamespace}.MethodDispatchType");
        CodeInstruction = compilation.GetTypeByMetadataName($"{harmonyNamespace}.CodeInstruction")?
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated); 
        EnumerableOfCodeInstruction = CodeInstruction == null
            ? null
            : compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T).Construct(CodeInstruction)
                .WithNullableAnnotation(NullableAnnotation.NotAnnotated); ;
        HarmonyInstance = (harmonyNamespace == Harmony2Namespace
            ? compilation.GetTypeByMetadataName($"{harmonyNamespace}.Harmony")
            : compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyInstance"))?
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        RefResultOfT = (INamedTypeSymbol?)compilation.GetTypeByMetadataName($"{harmonyNamespace}.RefResult`1")?
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
    }

    public static bool IsHarmonyLoaded(Compilation compilation) =>
        _allHarmonyNamespaces.Any(harmonyNamespace => IsHarmonyLoaded(compilation, harmonyNamespace));

    public static bool IsHarmonyLoaded(Compilation compilation, string harmonyNamespace) =>
        compilation.GetTypeByMetadataName($"{harmonyNamespace}.HarmonyPatch") is not null;
}