using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace HarmonyTools.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HarmonyToolsAnalyzer : DiagnosticAnalyzer
{
    private const string TargetCategory = "Target";

#pragma warning disable RS2008
    private static readonly DiagnosticDescriptor MethodMustExistRule = 
        CreateRule(DiagnosticIds.MethodMustExist, nameof(Resources.MethodMustExistTitle), nameof(Resources.MethodMustExistMessageFormat), TargetCategory, DiagnosticSeverity.Warning);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [MethodMustExistRule];

    private static DiagnosticDescriptor CreateRule(string id, string titleResource, string messageFormatResource,
        string category, DiagnosticSeverity severity, bool isEnabledByDefault = true, string? descriptionResource = null) =>
        new DiagnosticDescriptor(id,
            new LocalizableResourceString(titleResource, Resources.ResourceManager, typeof(Resources)),
            new LocalizableResourceString(messageFormatResource, Resources.ResourceManager, typeof(Resources)),
            category, severity, isEnabledByDefault,
            descriptionResource == null ? null : new LocalizableResourceString(descriptionResource, Resources.ResourceManager, typeof(Resources)));

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        context.RegisterSymbolAction(AnalyzeTypeV1, SymbolKind.NamedType);
        context.RegisterSymbolAction(AnalyzeTypeV2, SymbolKind.NamedType);
    }

    private static void AnalyzeTypeV1(SymbolAnalysisContext context)
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        var patchDescription = HarmonyPatchDescriptionV1.Parse(namedTypeSymbol, context.Compilation);
        if (patchDescription == null)
            return;

        CheckMethodMustExist(context, patchDescription);
    }

    private static void AnalyzeTypeV2(SymbolAnalysisContext context)
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        var patchDescription = HarmonyPatchDescriptionV2.Parse(namedTypeSymbol, context.Compilation);
        if (patchDescription == null)
            return;

        CheckMethodMustExist(context, patchDescription);
    }

    private static void CheckMethodMustExist(SymbolAnalysisContext context, HarmonyPatchDescription patchDescription)
    {
        if (patchDescription is not { PatchedTypes: [{ Value: not null }], PatchedMethodNames: [{ Value: not null }] })
            return;

        var targetType = patchDescription.PatchedTypes[0].Value!;
        var targetMethodName = patchDescription.PatchedMethodNames[0].Value!;
        var porentialTargetMembers = targetType.GetMembers(targetMethodName);
        // TODO: narrow method
        if (porentialTargetMembers.Count(member => member is IMethodSymbol or IPropertySymbol) == 0)
            context.ReportDiagnostic(Diagnostic.Create(MethodMustExistRule, patchDescription.PatchedMethodNames[0].Syntax?.GetLocation(), 
                targetType.ToDisplayString(), targetMethodName));
    }
}