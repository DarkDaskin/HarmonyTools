using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using HarmonyTools.Analyzers.HarmonyEnums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace HarmonyTools.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HarmonyToolsAnalyzer : DiagnosticAnalyzer
{
    public const string HarmonyNamespaceKey = "HarmonyNamespace";

#pragma warning disable RS2008
    private const string GeneralCategory = "General";
    private static readonly DiagnosticDescriptor AttributeArgumentsMustBeValidRule =
        CreateRule(DiagnosticIds.AttributeArgumentsMustBeValid,
            nameof(Resources.AttributeArgumentsMustBeValidTitle), nameof(Resources.AttributeArgumentsMustBeValidMessageFormat),
            GeneralCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor HarmonyPatchAttributeMustBeOnTypeRule =
        CreateRule(DiagnosticIds.HarmonyPatchAttributeMustBeOnType,
            nameof(Resources.HarmonyPatchAttributeMustBeOnTypeTitle), nameof(Resources.HarmonyPatchAttributeMustBeOnTypeMessageFormat),
            GeneralCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchTypeMustNotBeGenericRule =
        CreateRule(DiagnosticIds.PatchTypeMustNotBeGeneric,
            nameof(Resources.PatchTypeMustNotBeGenericTitle), nameof(Resources.PatchTypeMustNotBeGenericMessageFormat),
            GeneralCategory, DiagnosticSeverity.Warning);

    private const string TargetMethodCategory = "TargetMethod";
    private static readonly DiagnosticDescriptor TargetMethodMustExistRule = 
        CreateRule(DiagnosticIds.TargetMethodMustExist, 
            nameof(Resources.TargetMethodMustExistTitle), nameof(Resources.TargetMethodMustExistMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetMethodMustNotBeAmbiguousRule = 
        CreateRule(DiagnosticIds.TargetMethodMustNotBeAmbiguous,
            nameof(Resources.TargetMethodMustNotBeAmbiguousTitle), nameof(Resources.TargetMethodMustNotBeAmbiguousMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetTypeMustExistRule = 
        CreateRule(DiagnosticIds.TargetTypeMustExist, 
            nameof(Resources.TargetTypeMustExistTitle), nameof(Resources.TargetTypeMustExistMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetMethodMustBeSpecifiedRule = 
        CreateRule(DiagnosticIds.TargetMethodMustBeFullySpecified, 
            nameof(Resources.TargetMethodMustBeFullySpecifiedTitle), nameof(Resources.TargetMethodMustBeFullySpecifiedMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetMethodMustNotBeOverspecifiedRule = 
        CreateRule(DiagnosticIds.TargetMethodMustNotBeOverspecified, 
            nameof(Resources.TargetMethodMustNotBeOverspecifiedTitle), nameof(Resources.TargetMethodMustNotBeOverspecifiedMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor ArgumentTypesAndVariationsMustMatchRule = 
        CreateRule(DiagnosticIds.ArgumentTypesAndVariationsMustMatch, 
            nameof(Resources.ArgumentTypesAndVariationsMustMatchTitle), nameof(Resources.ArgumentTypesAndVariationsMustMatchMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DontUseIndividualAnnotationsWithBulkPatchingRule = 
        CreateRule(DiagnosticIds.DontUseIndividualAnnotationsWithBulkPatching, 
            nameof(Resources.DontUseIndividualAnnotationsWithBulkPatchingTitle), 
            nameof(Resources.DontUseIndividualAnnotationsWithBulkPatchingMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DontUseMultipleBulkPatchingMethodsRule = 
        CreateRule(DiagnosticIds.DontUseMultipleBulkPatchingMethods, 
            nameof(Resources.DontUseMultipleBulkPatchingMethodsTitle), nameof(Resources.DontUseMultipleBulkPatchingMethodsMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethodsRule = 
        CreateRule(DiagnosticIds.DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethods, 
            nameof(Resources.DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethodsTitle), 
            nameof(Resources.DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethodsMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetTypeMustBeNamedTypeRule = 
        CreateRule(DiagnosticIds.TargetTypeMustBeNamedType, 
            nameof(Resources.TargetTypeMustBeNamedTypeTitle), nameof(Resources.TargetTypeMustBeNamedTypeMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetTypeMustNotBeOpenGenericTypeRule = 
        CreateRule(DiagnosticIds.TargetTypeMustNotBeOpenGenericType, 
            nameof(Resources.TargetTypeMustNotBeOpenGenericTypeTitle), nameof(Resources.TargetTypeMustNotBeOpenGenericTypeMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetMethodMustNotBeGenericRule = 
        CreateRule(DiagnosticIds.TargetMethodMustNotBeGeneric, 
            nameof(Resources.TargetMethodMustNotBeGenericTitle), nameof(Resources.TargetMethodMustNotBeGenericMessageFormat), 
            TargetMethodCategory, DiagnosticSeverity.Warning);

    private const string PatchMethodCategory = "PatchMethod";
    private static readonly DiagnosticDescriptor PatchMethodsMustBeStaticRule = 
        CreateRule(DiagnosticIds.PatchMethodsMustBeStatic, 
            nameof(Resources.PatchMethodsMustBeStaticTitle), nameof(Resources.PatchMethodsMustBeStaticMessageFormat), 
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodMustHaveSingleKindRule = 
        CreateRule(DiagnosticIds.PatchMethodMustHaveSingleKind, 
            nameof(Resources.PatchMethodMustHaveSingleKindTitle), nameof(Resources.PatchMethodMustHaveSingleKindMessageFormat), 
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DontDefineMultipleAuxiliaryPatchMethodsRule =
        CreateRule(DiagnosticIds.DontDefineMultipleAuxiliaryPatchMethods,
            nameof(Resources.DontDefineMultipleAuxiliaryPatchMethodsTitle), 
            nameof(Resources.DontDefineMultipleAuxiliaryPatchMethodsMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodsMustNotBeGenericRule =
        CreateRule(DiagnosticIds.PatchMethodsMustNotBeGeneric,
            nameof(Resources.PatchMethodsMustNotBeGenericTitle), nameof(Resources.PatchMethodsMustNotBeGenericMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor ArgumentsOnTypesAndMethodsMustHaveNewNameRule =
        CreateRule(DiagnosticIds.ArgumentsOnTypesAndMethodsMustHaveNewName,
            nameof(Resources.ArgumentsOnTypesAndMethodsMustHaveNewNameTitle), 
            nameof(Resources.ArgumentsOnTypesAndMethodsMustHaveNewNameMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor ArgumentNewNamesMustBeUniqueRule =
        CreateRule(DiagnosticIds.ArgumentNewNamesMustBeUnique,
            nameof(Resources.ArgumentNewNamesMustBeUniqueTitle), nameof(Resources.ArgumentNewNamesMustBeUniqueMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodReturnTypesMustBeCorrectWithSubtypesRule =
        CreateRule(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect,
            nameof(Resources.PatchMethodReturnTypesMustBeCorrectTitle), 
            nameof(Resources.PatchMethodReturnTypesMustBeCorrectWithSubtypesMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodReturnTypesMustBeCorrectWithSupertypesRule =
        CreateRule(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect,
            nameof(Resources.PatchMethodReturnTypesMustBeCorrectTitle), 
            nameof(Resources.PatchMethodReturnTypesMustBeCorrectWithSupertypesMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodReturnTypesMustBeCorrectExactRule =
        CreateRule(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect,
            nameof(Resources.PatchMethodReturnTypesMustBeCorrectTitle), 
            nameof(Resources.PatchMethodReturnTypesMustBeCorrectExactMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodParameterTypesMustBeCorrectWithSupertypesRule =
        CreateRule(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect,
            nameof(Resources.PatchMethodParameterTypesMustBeCorrectTitle), 
            nameof(Resources.PatchMethodParameterTypesMustBeCorrectWithSupertypesMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodParameterTypesMustBeCorrectExactRule =
        CreateRule(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect,
            nameof(Resources.PatchMethodParameterTypesMustBeCorrectTitle), 
            nameof(Resources.PatchMethodParameterTypesMustBeCorrectExactMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodParameterTypesMustBeCorrectWithSubtypesRule =
        CreateRule(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect,
            nameof(Resources.PatchMethodParameterTypesMustBeCorrectTitle), 
            nameof(Resources.PatchMethodParameterTypesMustBeCorrectWithSubtypesMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodParametersMustBeValidInjectionsRule =
        CreateRule(DiagnosticIds.PatchMethodParametersMustBeValidInjections,
            nameof(Resources.PatchMethodParametersMustBeValidInjectionsTitle), 
            nameof(Resources.PatchMethodParametersMustBeValidInjectionsMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetMethodParameterWithSpecifiedNameMustExistRule =
        CreateRule(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist,
            nameof(Resources.TargetMethodParameterWithSpecifiedNameMustExistTitle), 
            nameof(Resources.TargetMethodParameterWithSpecifiedNameMustExistMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetMethodParameterWithSpecifiedIndexMustExistRule =
        CreateRule(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist,
            nameof(Resources.TargetMethodParameterWithSpecifiedIndexMustExistTitle), 
            nameof(Resources.TargetMethodParameterWithSpecifiedIndexMustExistMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetTypeFieldWithSpecifiedNameMustExistRule =
        CreateRule(DiagnosticIds.TargetTypeFieldWithSpecifiedNameMustExist,
            nameof(Resources.TargetTypeFieldWithSpecifiedNameMustExistTitle), 
            nameof(Resources.TargetTypeFieldWithSpecifiedNameMustExistMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TargetTypeFieldWithSpecifiedIndexMustExistRule =
        CreateRule(DiagnosticIds.TargetTypeFieldWithSpecifiedIndexMustExist,
            nameof(Resources.TargetTypeFieldWithSpecifiedIndexMustExistTitle),
            nameof(Resources.TargetTypeFieldWithSpecifiedIndexMustExistMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodDelegateParametersMustBeAnnotatedWithHarmonyDelegateRule =
        CreateRule(DiagnosticIds.PatchMethodDelegateParametersMustBeAnnotatedWithHarmonyDelegate,
            nameof(Resources.PatchMethodDelegateParametersMustBeAnnotatedWithHarmonyDelegateTitle), 
            nameof(Resources.PatchMethodDelegateParametersMustBeAnnotatedWithHarmonyDelegateMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DoNotUseInstanceParameterWithStaticMethodsRule =
        CreateRule(DiagnosticIds.DoNotUseInstanceParameterWithStaticMethods,
            nameof(Resources.DoNotUseInstanceParameterWithStaticMethodsTitle),
            nameof(Resources.DoNotUseInstanceParameterWithStaticMethodsMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DoNotUseResultRefWithMethodsNotReturningByRefRule =
        CreateRule(DiagnosticIds.DoNotUseResultRefWithMethodsNotReturningByRef,
            nameof(Resources.DoNotUseResultRefWithMethodsNotReturningByRefTitle),
            nameof(Resources.DoNotUseResultRefWithMethodsNotReturningByRefMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DoNotUseResultWithMethodsReturningByRefRule =
        CreateRule(DiagnosticIds.DoNotUseResultWithMethodsReturningByRef,
            nameof(Resources.DoNotUseResultWithMethodsReturningByRefTitle),
            nameof(Resources.DoNotUseResultWithMethodsReturningByRefMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DoNotUseResultWithMethodsReturningVoidRule =
        CreateRule(DiagnosticIds.DoNotUseResultWithMethodsReturningVoid,
            nameof(Resources.DoNotUseResultWithMethodsReturningVoidTitle), 
            nameof(Resources.DoNotUseResultWithMethodsReturningVoidMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor ParameterMustBeByRefRule =
        CreateRule(DiagnosticIds.ParameterMustBeByRef,
            nameof(Resources.ParameterMustBeByRefTitle), nameof(Resources.ParameterMustBeByRefMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor ParameterMustNotBeByRefRule =
        CreateRule(DiagnosticIds.ParameterMustNotBeByRef,
            nameof(Resources.ParameterMustNotBeByRefTitle), nameof(Resources.ParameterMustNotBeByRefMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor StateTypeMustNotDifferRule =
        CreateRule(DiagnosticIds.StateTypeMustNotDiffer,
            nameof(Resources.StateTypeMustNotDifferTitle), nameof(Resources.StateTypeMustNotDifferMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor StateShouldBeInitializedRule =
        CreateRule(DiagnosticIds.StateShouldBeInitialized,
            nameof(Resources.StateShouldBeInitializedTitle), nameof(Resources.StateShouldBeInitializedMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor ReversePatchMethodParameterMustCorrespondToTargetMethodParameterRule =
        CreateRule(DiagnosticIds.ReversePatchMethodParameterMustCorrespondToTargetMethodParameter,
            nameof(Resources.ReversePatchMethodParameterMustCorrespondToTargetMethodParameterTitle), 
            nameof(Resources.ReversePatchMethodParameterMustCorrespondToTargetMethodParameterMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor ReversePatchMethodParameterMustHaveCorrectRefKindRule =
        CreateRule(DiagnosticIds.ReversePatchMethodParameterMustHaveCorrectRefKind,
            nameof(Resources.ReversePatchMethodParameterMustHaveCorrectRefKindTitle), 
            nameof(Resources.ReversePatchMethodParameterMustHaveCorrectRefKindMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor AllTargetMethodParametersMustBeIncludedRule =
        CreateRule(DiagnosticIds.AllTargetMethodParametersMustBeIncluded,
            nameof(Resources.AllTargetMethodParametersMustBeIncludedTitle),
            nameof(Resources.AllTargetMethodParametersMustBeIncludedMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor InstanceParameterMustBePresentRule =
        CreateRule(DiagnosticIds.InstanceParameterMustBePresent,
            nameof(Resources.InstanceParameterMustBePresentTitle), nameof(Resources.InstanceParameterMustBePresentMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor InstanceParameterMustNotBePresentRule =
        CreateRule(DiagnosticIds.InstanceParameterMustNotBePresent,
            nameof(Resources.InstanceParameterMustNotBePresentTitle), nameof(Resources.InstanceParameterMustNotBePresentMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor PatchMethodsMustNotReturnByRefRule =
        CreateRule(DiagnosticIds.PatchMethodsMustNotReturnByRef,
            nameof(Resources.PatchMethodsMustNotReturnByRefTitle), nameof(Resources.PatchMethodsMustNotReturnByRefMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [
        AttributeArgumentsMustBeValidRule,
        HarmonyPatchAttributeMustBeOnTypeRule,
        PatchTypeMustNotBeGenericRule,

        TargetMethodMustExistRule,
        TargetMethodMustNotBeAmbiguousRule,
        TargetTypeMustExistRule,
        TargetMethodMustBeSpecifiedRule,
        TargetMethodMustNotBeOverspecifiedRule,
        ArgumentTypesAndVariationsMustMatchRule,
        DontUseIndividualAnnotationsWithBulkPatchingRule,
        DontUseMultipleBulkPatchingMethodsRule,
        DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethodsRule,
        TargetTypeMustBeNamedTypeRule,
        TargetTypeMustNotBeOpenGenericTypeRule,
        TargetMethodMustNotBeGenericRule,

        PatchMethodsMustBeStaticRule,
        PatchMethodMustHaveSingleKindRule,
        DontDefineMultipleAuxiliaryPatchMethodsRule,
        PatchMethodsMustNotBeGenericRule,
        ArgumentsOnTypesAndMethodsMustHaveNewNameRule,
        ArgumentNewNamesMustBeUniqueRule,
        PatchMethodReturnTypesMustBeCorrectWithSubtypesRule,
        PatchMethodReturnTypesMustBeCorrectWithSupertypesRule,
        PatchMethodReturnTypesMustBeCorrectExactRule,
        PatchMethodParameterTypesMustBeCorrectWithSupertypesRule,
        PatchMethodParameterTypesMustBeCorrectExactRule,
        PatchMethodParameterTypesMustBeCorrectWithSubtypesRule,
        PatchMethodParametersMustBeValidInjectionsRule,
        TargetMethodParameterWithSpecifiedNameMustExistRule,
        TargetMethodParameterWithSpecifiedIndexMustExistRule,
        TargetTypeFieldWithSpecifiedNameMustExistRule,
        TargetTypeFieldWithSpecifiedIndexMustExistRule,
        PatchMethodDelegateParametersMustBeAnnotatedWithHarmonyDelegateRule,
        DoNotUseInstanceParameterWithStaticMethodsRule,
        DoNotUseResultRefWithMethodsNotReturningByRefRule,
        DoNotUseResultWithMethodsReturningByRefRule,
        DoNotUseResultWithMethodsReturningVoidRule,
        ParameterMustBeByRefRule,
        ParameterMustNotBeByRefRule,
        StateTypeMustNotDifferRule,
        StateShouldBeInitializedRule,
        ReversePatchMethodParameterMustCorrespondToTargetMethodParameterRule,
        ReversePatchMethodParameterMustHaveCorrectRefKindRule,
        AllTargetMethodParametersMustBeIncludedRule,
        InstanceParameterMustBePresentRule,
        InstanceParameterMustNotBePresentRule,
        PatchMethodsMustNotReturnByRefRule,
    ];

    private static DiagnosticDescriptor CreateRule(string id, string titleResource, string messageFormatResource,
        string category, DiagnosticSeverity severity, bool isEnabledByDefault = true, string? descriptionResource = null) =>
        new (id,
            new LocalizableResourceString(titleResource, Resources.ResourceManager, typeof(Resources)),
            new LocalizableResourceString(messageFormatResource, Resources.ResourceManager, typeof(Resources)),
            category, severity, isEnabledByDefault,
            descriptionResource == null ? null : new LocalizableResourceString(descriptionResource, Resources.ResourceManager, typeof(Resources)));

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        if (!Debugger.IsAttached)
            context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(RegisterActionsWithFullMetadata);
        context.RegisterCompilationAction(RunAnalyzerWithFullMetadata);
    }

    private static void RegisterActionsWithFullMetadata(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.Options.MetadataImportOptions != MetadataImportOptions.All) 
            return;

        if (WellKnownTypes.IsHarmonyLoaded(context.Compilation, WellKnownTypes.Harmony1Namespace))
            context.RegisterSymbolAction(symbolContext => new VerifierV1(symbolContext).Verify(), SymbolKind.NamedType);
        if (WellKnownTypes.IsHarmonyLoaded(context.Compilation, WellKnownTypes.Harmony2Namespace))
            context.RegisterSymbolAction(symbolContext => new VerifierV2(symbolContext).Verify(), SymbolKind.NamedType);
    }

    private void RunAnalyzerWithFullMetadata(CompilationAnalysisContext context)
    {
        if (context.Compilation.Options.MetadataImportOptions == MetadataImportOptions.All)
            return;

        if (!WellKnownTypes.IsHarmonyLoaded(context.Compilation, WellKnownTypes.Harmony1Namespace) &&
            !WellKnownTypes.IsHarmonyLoaded(context.Compilation, WellKnownTypes.Harmony2Namespace))
            return;

        var compilation = context.Compilation.WithOptions(context.Compilation.Options.WithMetadataImportOptions(MetadataImportOptions.All));
        var compilationWithAnalyzer = new CompilationWithAnalyzers(compilation, [this], context.Options);
        var diagnostics = compilationWithAnalyzer.GetAnalyzerDiagnosticsAsync(context.CancellationToken).Result;
        foreach (var disgnostic in diagnostics)
            context.ReportDiagnostic(disgnostic);
    }
    

    private abstract class Verifier<TPatchDescription>(SymbolAnalysisContext context, string harmonyNamespace)
        where TPatchDescription : PatchDescription
    {
        protected readonly SymbolAnalysisContext Context = context;
        protected readonly INamedTypeSymbol PatchType = (INamedTypeSymbol)context.Symbol;
        protected readonly WellKnownTypes WellKnownTypes = new(context.Compilation, harmonyNamespace);
        
        public abstract void Verify();

        protected void VerifyCore(PatchDescriptionSet<TPatchDescription> set)
        {
            PatchTypeChecks();

            if (set.TypePatchDescription is not null)
                PreMergeChecks(set.TypePatchDescription);

            foreach (var patchMethod in set.PatchMethods)
            {
                if (patchMethod.PatchDescription is null) 
                    continue;

                PreMergeChecks(patchMethod.PatchDescription);

                if (set.TypePatchDescription is not null) 
                    patchMethod.PatchDescription.Merge(set.TypePatchDescription);

                PostMergeChecks(patchMethod.PatchDescription, set);

                if (set.TypePatchDescription is not null) 
                    PatchMethodChecks(patchMethod);
            }

            var hasPrimaryPatchMethodWithoutAnnotations = set.PatchMethods.Any(
                patchMethod => patchMethod.PatchDescription is null && patchMethod.IsPrimary);
            if (set.TypePatchDescription is not null && (set.PatchMethods is [] || hasPrimaryPatchMethodWithoutAnnotations))
            {
                PostMergeChecks(set.TypePatchDescription, set);

                foreach (var patchMethod in set.PatchMethods.Where(patchMethod => patchMethod.PatchDescription is null))
                {
                    if (patchMethod.IsPrimary)
                        patchMethod.PatchDescription = set.TypePatchDescription;

                    PatchMethodChecks(patchMethod);
                }
            }

            PatchDescriptionSetChecks(set);
        }

        protected virtual void PatchTypeChecks()
        {
            CheckPatchTypeMustNotBeGeneric();
        }

        protected virtual void PreMergeChecks(TPatchDescription patchDescription)
        {
            CheckAttributeArgumentsMustBeValid(patchDescription);
            CheckArgumentTypesAndVariationsMustMatch(patchDescription);
            CheckTargetTypeMustBeNamedType(patchDescription);
            CheckTargetTypeMustNotBeOpenGenericType(patchDescription);
        }

        protected virtual void PostMergeChecks(TPatchDescription patchDescription, PatchDescriptionSet<TPatchDescription> set)
        {
            CheckMethodMustExistAndNotAmbiguous(patchDescription);
            CheckMethodMustBeSpecified(patchDescription, set);
            CheckMethodMustNotBeOverspecified(patchDescription);
            CheckArgumentsOnTypesAndMethodsMustHaveNewName(patchDescription);
            CheckArgumentNewNamesMustBeUniqueRule(patchDescription);
        }

        protected virtual void PatchMethodChecks(PatchMethod<TPatchDescription> patchMethod)
        {
            patchMethod.UpdateParameters(WellKnownTypes, Context.Compilation);

            CheckDontUseTargetMethodAnnotationsOnNonPrimaryPatchMethods(patchMethod);
            CheckPatchMethodsMustBeStatic(patchMethod);
            CheckPatchMethodMustHaveSingleKind(patchMethod);
            CheckPatchMethodsMustNotBeGeneric(patchMethod);
            CheckPatchMethodReturnTypeMustBeCorrect(patchMethod);
            CheckPatchMethodsMustNotReturnByRef(patchMethod);
            CheckPatchMethodParameters(patchMethod);
        }

        protected virtual void PatchDescriptionSetChecks(PatchDescriptionSet<TPatchDescription> set)
        {
            CheckHarmonyPatchAttributeMustBeOnType(set);
            CheckDontDefineMultipleAuxiliaryPatchMethods(set);
            CheckBulkPatching(set);
            CheckPatchMethodStateParameters(set);
        }

        protected virtual void TargetMethodChecks(IMethodSymbol targetMethod, PatchDescription patchDescription)
        {
            CheckTargetMethodMustNotBeGeneric(targetMethod, patchDescription);
        }

        private void CheckPatchTypeMustNotBeGeneric()
        {
            if (!PatchType.IsGenericType)
                return;

            Context.ReportDiagnostic(Diagnostic.Create(PatchTypeMustNotBeGenericRule,
                PatchType.GetSyntax(cancellationToken: Context.CancellationToken)?.GetIdentifierLocation()));
        }

        private void CheckAttributeArgumentsMustBeValid(PatchDescription patchDescription)
        {
            foreach (var detail in patchDescription.TargetTypes.Where(type => type.Value is null))
                ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.MethodNames.Where(type => string.IsNullOrWhiteSpace(type.Value)))
                ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.MethodTypes.Where(type => !IsValidEnumValue(type.Value)))
                ReportInvalidAttributeArgument(detail);

            if (patchDescription.HarmonyVersion == 1)
                foreach (var detail in patchDescription.MethodTypes.Where(type => type.Value is >= MethodType.Enumerator and <= MethodType.Async))
                    ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.ArgumentTypes.Where(types => types.Value.IsDefault))
                ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.ArgumentTypes.Where(types => !types.Value.IsDefault))
                for (var i = 0; i < detail.Value.Length; i++)
                    if (detail.Value[i] is null)
                        ReportInvalidAttributeArgument(detail, i);

            foreach (var detail in patchDescription.ArgumentVariations.Where(variations => variations.Value.IsDefault))
                ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.ArgumentVariations.Where(variations => !variations.Value.IsDefault))
                for (var i = 0; i < detail.Value.Length; i++)
                    if (!IsValidEnumValue(detail.Value[i]))
                        ReportInvalidAttributeArgument(detail, i);

            if (patchDescription.Before is { Value.IsDefaultOrEmpty: true })
                ReportInvalidAttributeArgument(patchDescription.Before);

            if (patchDescription.Before is { Value.IsDefaultOrEmpty: false })
                for (var i = 0; i < patchDescription.Before.Value.Length; i++)
                    if (string.IsNullOrWhiteSpace(patchDescription.Before.Value[i]))
                        ReportInvalidAttributeArgument(patchDescription.Before, i);

            if (patchDescription.After is { Value.IsDefaultOrEmpty: true })
                ReportInvalidAttributeArgument(patchDescription.After);

            if (patchDescription.After is { Value.IsDefaultOrEmpty: false })
                for (var i = 0; i < patchDescription.After.Value.Length; i++)
                    if (string.IsNullOrWhiteSpace(patchDescription.After.Value[i]))
                        ReportInvalidAttributeArgument(patchDescription.After, i);

            foreach (var argumentOverride in patchDescription.ArgumentOverrides)
            {
                if (argumentOverride.Name is not null && string.IsNullOrWhiteSpace(argumentOverride.Name.Value))
                    ReportInvalidAttributeArgument(argumentOverride.Name);
                if (argumentOverride.Index is not null && argumentOverride.Index.Value < 0)
                    ReportInvalidAttributeArgument(argumentOverride.Index);
                if (argumentOverride.NewName is not null && string.IsNullOrWhiteSpace(argumentOverride.NewName.Value))
                    ReportInvalidAttributeArgument(argumentOverride.NewName);
            }
        }

        private void CheckArgumentTypesAndVariationsMustMatch(PatchDescription patchDescription)
        {
            // Match argument variations with argument types from the same attribute.
            var argumentTypesAndVariationsByAttribute =
                from variation in patchDescription.ArgumentVariations
                let attributeSyntax = GetAttributeSyntax(variation)
                where attributeSyntax != null
                let type = patchDescription.ArgumentTypes.Single(type => GetAttributeSyntax(type) == attributeSyntax)
                where !type.Value.IsDefault && !variation.Value.IsDefault
                select (type, variation);
            foreach (var (type, variation) in argumentTypesAndVariationsByAttribute)
                if (type.Value.Length != variation.Value.Length)
                    Context.ReportDiagnostic(Diagnostic.Create(ArgumentTypesAndVariationsMustMatchRule,
                        type.Syntax!.GetLocation(), additionalLocations: [variation.Syntax!.GetLocation()]));


            static AttributeSyntax? GetAttributeSyntax(IHasSyntax hasSyntax)
            {
                var attributeArgumentSyntax = hasSyntax.Syntax as AttributeArgumentSyntax;
                var attributeArgumentListSyntax = attributeArgumentSyntax?.Parent as AttributeArgumentListSyntax;
                return attributeArgumentListSyntax?.Parent as AttributeSyntax;
            }
        }

        private void CheckTargetTypeMustBeNamedType(PatchDescription patchDescription)
        {
            if (patchDescription.TargetTypes is not [{ Value: not null }])
                return;

            var targetType = patchDescription.TargetTypes[0].Value!;
            if (targetType is INamedTypeSymbol)
                return;

            Context.ReportDiagnostic(Diagnostic.Create(TargetTypeMustBeNamedTypeRule,
                patchDescription.TargetTypes.GetLocation(),
                targetType.ToDisplayString()));
        }

        private void CheckTargetTypeMustNotBeOpenGenericType(PatchDescription patchDescription)
        {
            if (patchDescription.TargetTypes is not [{ Value: INamedTypeSymbol }])
                return;

            var targetType = (INamedTypeSymbol)patchDescription.TargetTypes[0].Value!;
            if (!targetType.IsUnboundGenericType)
                return;

            Context.ReportDiagnostic(Diagnostic.Create(TargetTypeMustNotBeOpenGenericTypeRule,
                patchDescription.TargetTypes.GetLocation(),
                targetType.ToDisplayString()));
        }

        private void CheckMethodMustExistAndNotAmbiguous(PatchDescription patchDescription)
        {
            if (patchDescription.TargetTypes is not [{ Value: INamedTypeSymbol }])
                return;

            var targetType = (INamedTypeSymbol)patchDescription.TargetTypes[0].Value!;

            if (targetType.IsUnboundGenericType)
                return;

            CheckMethodMustExistAndNotAmbiguous(patchDescription, targetType);
        }

        protected void CheckMethodMustExistAndNotAmbiguous(PatchDescription patchDescription, INamedTypeSymbol targetType)
        {
            if (HasConflictingSpecifications(patchDescription))
                return;

            patchDescription.TargetType = targetType;

            var methodType = patchDescription.MethodTypes.Length == 0 ? MethodType.Normal : patchDescription.MethodTypes[0].Value;
            var isMemberNameSpecified = patchDescription.MethodNames is [{ Value: not null } detail] && !string.IsNullOrWhiteSpace(detail.Value);

            var targetMethods = Enumerable.Empty<IMethodSymbol>();
            string memberName;
            switch (methodType)
            {
                case MethodType.Constructor when !isMemberNameSpecified:
                    targetMethods = targetType.InstanceConstructors;
                    memberName = ".ctor";
                    break;
                case MethodType.StaticConstructor when !isMemberNameSpecified:
                    targetMethods = targetType.StaticConstructors;
                    memberName = ".cctor";
                    break;
                // TODO: check that Enumerator and Async methods are actually as such
                case MethodType.Normal or MethodType.Getter or MethodType.Setter or MethodType.Enumerator or MethodType.Async
                    when isMemberNameSpecified:
                    memberName = patchDescription.MethodNames[0].Value!;
                    var targetMembers = targetType.GetMembers(memberName);
                    switch (methodType)
                    {
                        case MethodType.Getter:
                            targetMethods = targetMembers.OfType<IPropertySymbol>().Select(property => property.GetMethod)
                                .Where(method => method is not null).Cast<IMethodSymbol>();
                            memberName = $"get_{memberName}";
                            break;
                        case MethodType.Setter:
                            targetMethods = targetMembers.OfType<IPropertySymbol>().Select(property => property.SetMethod)
                                .Where(method => method is not null).Cast<IMethodSymbol>();
                            memberName = $"set_{memberName}";
                            break;
                        default:
                            targetMethods = targetMembers.OfType<IMethodSymbol>().Where(method => method.MethodKind == MethodKind.Ordinary);
                            break;
                    }
                    break;
                case MethodType.Getter or MethodType.Setter when !isMemberNameSpecified:
                    memberName = "this[]";
                    targetMembers = targetType.GetMembers(memberName);
                    switch (methodType)
                    {
                        case MethodType.Getter:
                            targetMethods = targetMembers.OfType<IPropertySymbol>().Select(property => property.GetMethod)
                                .Where(method => method is not null).Cast<IMethodSymbol>();
                            memberName = "get_Item";
                            break;
                        case MethodType.Setter:
                            targetMethods = targetMembers.OfType<IPropertySymbol>().Select(property => property.SetMethod)
                                .Where(method => method is not null).Cast<IMethodSymbol>();
                            memberName = "set_Item";
                            break;
                    }
                    break;
                default:
                    return;
            }

            if (patchDescription.ArgumentTypes.Length == 1)
            {
                var argumentTypes = patchDescription.ArgumentTypes[0].Value;
                var argumentVariations = patchDescription.ArgumentVariations.Length == 0 ?
                    [..argumentTypes.Select(_ => ArgumentType.Normal)] :
                    patchDescription.ArgumentVariations[0].Value;

                if (argumentTypes.Length != argumentVariations.Length)
                    return;

                if (argumentTypes.Any(type => type is null) || argumentVariations.Any(variation => !IsValidEnumValue(variation)))
                    return;

                if (methodType == MethodType.StaticConstructor && argumentTypes.Length > 0)
                    return;

                if (methodType is MethodType.Getter or MethodType.Setter && isMemberNameSpecified && argumentTypes.Length > 0)
                    return;

                targetMethods = targetMethods.Where(method => IsMatch(method, argumentTypes, argumentVariations, Context.Compilation, methodType == MethodType.Setter));
            }

            var targetMethodsArray = targetMethods.ToArray();
            if (targetMethodsArray.Length == 0)
                Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustExistRule,
                    patchDescription.GetLocation(IsHarmonyPatchAttribute), patchDescription.GetAdditionalLocations(IsHarmonyPatchAttribute),
                    memberName, targetType.ToDisplayString()));
            else if (targetMethodsArray.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustNotBeAmbiguousRule,
                    patchDescription.GetLocation(IsHarmonyPatchAttribute), patchDescription.GetAdditionalLocations(IsHarmonyPatchAttribute),
                    memberName, targetType.ToDisplayString(), targetMethodsArray.Length));
            else
            {
                var targetMethod = targetMethodsArray[0];
                patchDescription.TargetMethod = targetMethod;
                TargetMethodChecks(targetMethod, patchDescription);
            }
        }

        private void CheckMethodMustBeSpecified(PatchDescription patchDescription, PatchDescriptionSet<TPatchDescription> set)
        {
            if (HasConflictingSpecifications(patchDescription))
                return;

            if (GetBulkPatchMethods(set).Any())
                return;

            var methodType = patchDescription.MethodTypes.Length == 0 ? MethodType.Normal : patchDescription.MethodTypes[0].Value;
            var isMemberTypeSpecified = patchDescription.TargetTypes is [_] ||
                                        patchDescription is PatchDescriptionV2 { TargetTypeNames: [_] };
            var isMemberNameSpecified = patchDescription.MethodNames is [_];
            var isPatchAll = set.TypePatchDescription?.IsPatchAll?.Value ?? false;
            if (!isMemberTypeSpecified || methodType is MethodType.Normal && !isMemberNameSpecified && !isPatchAll)
            {
                var predicate = (AttributeData attribute) =>
                    attribute.Is(WellKnownTypes.HarmonyPatch) || isPatchAll && attribute.Is(WellKnownTypes.HarmonyPatchAll);
                Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustBeSpecifiedRule,
                    patchDescription.GetLocation(predicate), patchDescription.GetAdditionalLocations(predicate)));
            }
        }

        private void CheckMethodMustNotBeOverspecified(PatchDescription patchDescription)
        {
            if (patchDescription is PatchDescriptionV2 patchDescriptionV2)
            {
                var typeDetails = patchDescriptionV2.TargetTypes.Concat<IHasSyntax>(patchDescriptionV2.TargetTypeNames).ToArray();
                if (typeDetails.Length > 1)
                    Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustNotBeOverspecifiedRule,
                        typeDetails.GetLocation(), typeDetails.GetAdditionalLocations()));
            }
            else if (patchDescription.TargetTypes.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustNotBeOverspecifiedRule,
                    patchDescription.TargetTypes.GetLocation(), patchDescription.TargetTypes.GetAdditionalLocations()));

            if (patchDescription.MethodNames.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustNotBeOverspecifiedRule,
                    patchDescription.MethodNames.GetLocation(), patchDescription.MethodNames.GetAdditionalLocations()));

            if (patchDescription.MethodTypes.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustNotBeOverspecifiedRule,
                    patchDescription.MethodTypes.GetLocation(), patchDescription.MethodTypes.GetAdditionalLocations()));

            if (patchDescription.ArgumentTypes.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustNotBeOverspecifiedRule,
                    patchDescription.ArgumentTypes.GetLocation(), patchDescription.ArgumentTypes.GetAdditionalLocations()));

            if (patchDescription.ArgumentVariations.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustNotBeOverspecifiedRule,
                    patchDescription.ArgumentVariations.GetLocation(), patchDescription.ArgumentVariations.GetAdditionalLocations()));
        }

        private void CheckArgumentsOnTypesAndMethodsMustHaveNewName(PatchDescription patchDescription)
        {
            foreach (var argument in patchDescription.ArgumentOverrides.Where(argument => argument.NewName is null))
            {
                Context.ReportDiagnostic(Diagnostic.Create(ArgumentsOnTypesAndMethodsMustHaveNewNameRule,
                    argument.Attribute.GetSyntax()?.GetLocation()));
            }
        }

        private void CheckArgumentNewNamesMustBeUniqueRule(PatchDescription patchDescription)
        {
            var argumentRenamesPerNewName =
                from argument in patchDescription.ArgumentOverrides
                where argument.NewName is { Value: not null }
                group argument.NewName! by argument.NewName!.Value!;
            foreach (var argumentGroup in argumentRenamesPerNewName.Where(argumentGroup => argumentGroup.Count() > 1))
            {
                Context.ReportDiagnostic(Diagnostic.Create(ArgumentNewNamesMustBeUniqueRule,
                    argumentGroup.GetLocation(), argumentGroup.GetAdditionalLocations(),
                    argumentGroup.Key));
            }
        }

        private void CheckHarmonyPatchAttributeMustBeOnType(PatchDescriptionSet<TPatchDescription> set)
        {
            if (set.TypePatchDescription is { IsDefining: true })
                return;

            // Find any patch method having any Harmony attribute. Ignore convention-based methods, because in type not marked with HarmonyPatch
            // method names do not have special meaning.
            var patchMethodWithAttributeSyntax = (
                from patchMethod in set.PatchMethods
                let methodKindAttributeSyntax = patchMethod.MethodKinds.FirstOrDefault(detail => detail.Syntax is AttributeSyntax)?.Syntax
                let patchDescriptionAttributeSyntax =
                    patchMethod.PatchDescription?.Attrubutes.FirstOrDefault().GetSyntax()
                let syntax = patchDescriptionAttributeSyntax ?? methodKindAttributeSyntax
                where syntax is not null
                select (patchMethod, syntax)).FirstOrDefault();
            if (patchMethodWithAttributeSyntax.patchMethod is not null)
            {
                var properties = ImmutableDictionary.Create<string, string?>().Add(HarmonyNamespaceKey, harmonyNamespace);
                Context.ReportDiagnostic(Diagnostic.Create(HarmonyPatchAttributeMustBeOnTypeRule,
                    patchMethodWithAttributeSyntax.patchMethod.Method.ContainingType
                        .GetSyntax(patchMethodWithAttributeSyntax.syntax, Context.CancellationToken)?.GetIdentifierLocation(), properties));
            }
        }

        private void CheckBulkPatching(PatchDescriptionSet<TPatchDescription> set)
        {
            if (set.TypePatchDescription is null)
                return;

            var bulkPatchMethods = GetBulkPatchMethods(set).ToArray();
            var hasBulkPatchMethods = bulkPatchMethods.Any();
            var isPatchAll = set.TypePatchDescription.IsPatchAll?.Value ?? false;
            if (!isPatchAll && !hasBulkPatchMethods)
                return;

            var conflictingSyntaxes = bulkPatchMethods.SelectMany(patchMethod => patchMethod.MethodKinds)
                .Concat<IHasSyntax>(isPatchAll ? [set.TypePatchDescription.IsPatchAll!] : []).ToArray();

            if (conflictingSyntaxes.Length > 1)
            {
                Context.ReportDiagnostic(Diagnostic.Create(DontUseMultipleBulkPatchingMethodsRule,
                    conflictingSyntaxes.GetLocation(), conflictingSyntaxes.GetAdditionalLocations()));
            }

            var conflictingPatchDescriptions = set.PatchMethods.Select(patchMethod => patchMethod.PatchDescription).Concat([set.TypePatchDescription])
                .Distinct().Where(patchDescription => IsConflictingPatchDescription(patchDescription, hasBulkPatchMethods)).ToArray();
            if (conflictingPatchDescriptions is [])
                return;

            conflictingSyntaxes =
            [
                .. conflictingSyntaxes,
                .. conflictingPatchDescriptions.SelectMany(
                    patchDescription => patchDescription?.Attrubutes.Where(attribute => attribute.Is(WellKnownTypes.HarmonyPatch)).Select(attribute => new SyntaxWrapper(attribute)) ?? []),
            ];

            Context.ReportDiagnostic(Diagnostic.Create(DontUseIndividualAnnotationsWithBulkPatchingRule,
                conflictingSyntaxes.GetLocation(), conflictingSyntaxes.GetAdditionalLocations()));


            static bool IsConflictingPatchDescription(TPatchDescription? patchDescription, bool hasBulkPatchMethods)
            {
                if (patchDescription is null) 
                    return false;

                if (patchDescription.MethodNames.Concat<IHasSyntax>(patchDescription.MethodTypes).Concat(patchDescription.ArgumentTypes).Any())
                    return true;

                return hasBulkPatchMethods && patchDescription.TargetTypes.Any();
            }
        }

        private void CheckPatchMethodStateParameters(PatchDescriptionSet<TPatchDescription> set)
        {
            var stateParameters = set.PatchMethods
                .Where(patchMethod => !patchMethod.Parameters.IsDefault)
                .SelectMany(patchMethod => patchMethod.Parameters.Where(parameter => parameter.Kind == InjectionKind.State))
                .ToArray();

            if (stateParameters.Select(parameter => parameter.Parameter.Type).Distinct(SymbolEqualityComparer.Default).Count() > 1)
                Context.ReportDiagnostic(Diagnostic.Create(StateTypeMustNotDifferRule,
                    stateParameters.GetTypeLocation(), stateParameters.GetAdditionalTypeLocations()));

            if (stateParameters.Any() && stateParameters.All(parameter => parameter.Parameter.RefKind is not (RefKind.Ref or RefKind.Out)))
                Context.ReportDiagnostic(Diagnostic.Create(StateShouldBeInitializedRule,
                    stateParameters.GetLocation(), stateParameters.GetAdditionalLocations()));
        }

        private void CheckDontUseTargetMethodAnnotationsOnNonPrimaryPatchMethods(PatchMethod patchMethod)
        {
            if (patchMethod.PatchDescription is null || patchMethod.MethodKinds.Any(kind => kind.Value.IsPrimary()))
                return;

            var predicate = (AttributeData attribute) => attribute.Is(WellKnownTypes.HarmonyAttribute);
            Context.ReportDiagnostic(Diagnostic.Create(DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethodsRule,
                patchMethod.PatchDescription.GetLocation(predicate), patchMethod.PatchDescription.GetAdditionalLocations(predicate)));
        }

        private void CheckPatchMethodsMustBeStatic(PatchMethod patchMethod)
        {
            if (patchMethod.Method.IsStatic || patchMethod.MethodKinds is [] or [{ Value: PatchMethodKind.DelegateInvoke }])
                return;

            Context.ReportDiagnostic(Diagnostic.Create(PatchMethodsMustBeStaticRule,
                patchMethod.GetLocation(Context.CancellationToken)));
        }

        private void CheckPatchMethodMustHaveSingleKind(PatchMethod patchMethod)
        {
            if (patchMethod.MethodKinds.Length <= 1)
                return;

            Context.ReportDiagnostic(Diagnostic.Create(PatchMethodMustHaveSingleKindRule,
                patchMethod.MethodKinds.GetLocation(), patchMethod.MethodKinds.GetAdditionalLocations()));
        }

        private void CheckPatchMethodsMustNotBeGeneric(PatchMethod patchMethod)
        {
            if (!patchMethod.Method.IsGenericMethod)
                return;

            Context.ReportDiagnostic(Diagnostic.Create(PatchMethodsMustNotBeGenericRule,
                patchMethod.GetLocation(Context.CancellationToken)));
        }

        private void CheckPatchMethodReturnTypeMustBeCorrect(PatchMethod patchMethod)
        {
            if (patchMethod.MethodKinds is [])
                return;

            var targetMethodReturnType = patchMethod.TargetMethod?.ReturnType;

            var isInNullableContext = patchMethod.Method.IsInNullableContext(Context.Compilation);

            ITypeSymbol[] validReturnTypes = [];
            var matchTypeInReverse = false;
            var typeExactlyEquals = patchMethod.Method.ReturnsByRef;
            var skipValidation = false;
            if (patchMethod.Is(PatchMethodKind.Prefix))
                validReturnTypes = [WellKnownTypes.Void, WellKnownTypes.Boolean];
            else if (patchMethod.Is(PatchMethodKind.Postfix))
            {
                if (patchMethod.TargetMethod is not null)
                {
                    validReturnTypes = patchMethod.IsPassthrough() switch
                    {
                        true => [targetMethodReturnType!],
                        false => [WellKnownTypes.Void],
                        null when !targetMethodReturnType!.Is(WellKnownTypes.Void) => [WellKnownTypes.Void, targetMethodReturnType!],
                        _ => [WellKnownTypes.Void]
                    };
                }
                else
                    skipValidation = true;
            }
            else if (patchMethod.Is(PatchMethodKind.Transpiler))
                validReturnTypes = [WellKnownTypes.EnumerableOfCodeInstruction!.WithNullableAnnotation(NullableAnnotation.NotAnnotated)];
            else if (patchMethod.Is(PatchMethodKind.Finalizer))
                validReturnTypes = [WellKnownTypes.Void, WellKnownTypes.Exception.WithNullableAnnotation(NullableAnnotation.Annotated)];
            else if (patchMethod.Is(PatchMethodKind.ReversePatch) || patchMethod.Is(PatchMethodKind.DelegateInvoke))
            {
                if (patchMethod.TargetMethod is not null && 
                    !patchMethod.ContainsTranspiler(WellKnownTypes, Context.Compilation, Context.CancellationToken))
                {
                    validReturnTypes = [targetMethodReturnType!];
                    matchTypeInReverse = true;
                }
                else
                    skipValidation = true;

                if (patchMethod.TargetMethod is not null)
                    typeExactlyEquals |= patchMethod.TargetMethod.ReturnType.IsValueType;
            }
            else if (patchMethod.Is(PatchMethodKind.Prepare))
                validReturnTypes = [WellKnownTypes.Void, WellKnownTypes.Boolean];
            else if (patchMethod.Is(PatchMethodKind.Cleanup))
                validReturnTypes = [WellKnownTypes.Void, WellKnownTypes.Exception.WithNullableAnnotation(NullableAnnotation.Annotated)];
            else if (patchMethod.Is(PatchMethodKind.TargetMethod))
                validReturnTypes = [WellKnownTypes.MethodBase.WithNullableAnnotation(NullableAnnotation.NotAnnotated)];
            else if (patchMethod.Is(PatchMethodKind.TargetMethods))
                validReturnTypes = [WellKnownTypes.EnumerableOfMethodBase.WithNullableAnnotation(NullableAnnotation.NotAnnotated)];

            if (skipValidation)
                return;

            foreach (var type in validReturnTypes)
            {
                if (matchTypeInReverse
                        ? type.Is(patchMethod.Method.ReturnType, Context.Compilation, typeExactlyEquals, isInNullableContext)
                        : patchMethod.Method.ReturnType.Is(type, Context.Compilation, typeExactlyEquals, isInNullableContext))
                    return;
            }

            var rule = typeExactlyEquals
                ? PatchMethodReturnTypesMustBeCorrectExactRule
                : matchTypeInReverse
                    ? PatchMethodReturnTypesMustBeCorrectWithSupertypesRule
                    : PatchMethodReturnTypesMustBeCorrectWithSubtypesRule;
            Context.ReportDiagnostic(Diagnostic.Create(rule,
                patchMethod.Method.GetSyntax(cancellationToken: Context.CancellationToken).GetTypeLocation(),
                 patchMethod.Method.Name, GetValidTypesString(validReturnTypes, isInNullableContext)));
        }

        private void CheckPatchMethodsMustNotReturnByRef(PatchMethod patchMethod)
        {
            if (patchMethod.MethodKinds is [] || !patchMethod.Method.ReturnsByRef)
                return;

            Context.ReportDiagnostic(Diagnostic.Create(PatchMethodsMustNotReturnByRefRule,
                patchMethod.GetRefReturnLocation(Context.CancellationToken),
                patchMethod.Method.Name));
        }

        private void CheckPatchMethodParameters(PatchMethod patchMethod)
        {
            var isInNullableContext = patchMethod.Method.IsInNullableContext(Context.Compilation);

            if (patchMethod.Is(PatchMethodKind.ReversePatch) || patchMethod.Is(PatchMethodKind.DelegateInvoke))
            {
                foreach (var parameter in patchMethod.Parameters)
                    CheckReversePatchMethodParameter(patchMethod, parameter, isInNullableContext);

                if (patchMethod.TargetMethod is not null)
                {
                    var presentTargetethodParameterCount = patchMethod.Parameters.FirstOrDefault()?.Kind == InjectionKind.Instance
                        ? patchMethod.Parameters.Length - 1
                        : patchMethod.Parameters.Length;
                    var missingTargetMethodParameters = patchMethod.TargetMethod.Parameters.Skip(presentTargetethodParameterCount);
                    foreach (var parameter in missingTargetMethodParameters)
                        Context.ReportDiagnostic(Diagnostic.Create(AllTargetMethodParametersMustBeIncludedRule,
                            patchMethod.GetLocation(Context.CancellationToken),
                            patchMethod.TargetMethod.Name, parameter.Name, patchMethod.Method.Name));
                }

                if (patchMethod.Is(PatchMethodKind.ReversePatch) &&
                    patchMethod is { TargetMethod.IsStatic: false, Method.IsStatic: true, TargetType: not null } &&
                    patchMethod.Parameters.FirstOrDefault()?.Kind != InjectionKind.Instance)
                {
                    Context.ReportDiagnostic(Diagnostic.Create(InstanceParameterMustBePresentRule,
                        patchMethod.GetLocation(Context.CancellationToken),
                        patchMethod.TargetType.ToDisplayString(), patchMethod.Method.Name));
                }
            }
            else
            {
                foreach (var parameter in patchMethod.Parameters)
                    CheckOrdinaryPatchMethodParameter(patchMethod, parameter, isInNullableContext);
            }
        }

        private void CheckOrdinaryPatchMethodParameter(PatchMethod patchMethod, PatchMethodParameter parameter, bool isInNullableContext)
        {
            switch ((parameter.Kind, parameter.MatchKind))
            {
                case (InjectionKind.None, _):
                    Context.ReportDiagnostic(Diagnostic.Create(PatchMethodParametersMustBeValidInjectionsRule,
                        parameter.GetLocation(Context.CancellationToken),
                        patchMethod.Method.Name, parameter.Parameter.Name));
                    break;

                case (InjectionKind.ParameterByName, ParameterMatchKind.ByName) when parameter is PatchMethodParameterByNameParameter parameter2:
                    foreach (var method in patchMethod.GetTargetMethods())
                    {
                        var targetMethodParameter = method.Parameters.FirstOrDefault(
                            targetMethodParameter => targetMethodParameter.Name == parameter2.ParameterName);
                        if (targetMethodParameter is null)
                            Context.ReportDiagnostic(Diagnostic.Create(TargetMethodParameterWithSpecifiedNameMustExistRule,
                                parameter2.GetLocation(Context.CancellationToken),
                                method.Name, parameter2.ParameterName, patchMethod.Method.Name, parameter2.Parameter.Name));
                        else
                            CheckOrdinaryPatchMethodParameterType(patchMethod, parameter2, targetMethodParameter.Type, isInNullableContext);
                    }

                    break;

                case (InjectionKind.ParameterByIndex, ParameterMatchKind.ByName) when parameter is PatchMethodParameterByIndexParameter parameter2:
                    foreach (var method in patchMethod.GetTargetMethods())
                    {
                        var targetMethodParameter = method.Parameters.ElementAtOrDefault(parameter2.ParameterIndex);
                        if (targetMethodParameter is null)
                            Context.ReportDiagnostic(Diagnostic.Create(TargetMethodParameterWithSpecifiedIndexMustExistRule,
                                parameter2.GetLocation(Context.CancellationToken),
                                method.Name, parameter2.ParameterIndex, patchMethod.Method.Name, parameter2.Parameter.Name));
                        else
                            CheckOrdinaryPatchMethodParameterType(patchMethod, parameter2, targetMethodParameter.Type, isInNullableContext);
                    }

                    break;

                case (InjectionKind.FieldByName, ParameterMatchKind.ByName) when parameter is PatchMethodFieldByNameParameter parameter2:
                    if (patchMethod.TargetType is not null)
                    {
                        var field = patchMethod.TargetType.GetMembersIncludingBaseTypes(parameter2.FieldName).OfType<IFieldSymbol>().FirstOrDefault();
                        if (field is null)
                            Context.ReportDiagnostic(Diagnostic.Create(TargetTypeFieldWithSpecifiedNameMustExistRule,
                                parameter.GetLocation(Context.CancellationToken),
                                patchMethod.TargetType.ToDisplayString(), parameter2.FieldName, patchMethod.Method.Name, 
                                parameter.Parameter.Name));
                        else
                            CheckOrdinaryPatchMethodParameterType(patchMethod, parameter, field.Type, isInNullableContext);
                    }

                    break;

                case (InjectionKind.FieldByIndex, ParameterMatchKind.ByName) when parameter is PatchMethodFieldByIndexParameter parameter2:
                    if (patchMethod.TargetType is not null)
                    {
                        var field = patchMethod.TargetType.GetMembers().OfType<IFieldSymbol>().ElementAtOrDefault(parameter2.FieldIndex);
                        if (field is null)
                            Context.ReportDiagnostic(Diagnostic.Create(TargetTypeFieldWithSpecifiedIndexMustExistRule,
                                parameter.GetLocation(Context.CancellationToken),
                                patchMethod.TargetType.ToDisplayString(), parameter2.FieldIndex, patchMethod.Method.Name, 
                                parameter.Parameter.Name));
                        else
                            CheckOrdinaryPatchMethodParameterType(patchMethod, parameter, field.Type, isInNullableContext);
                    }

                    break;

                case (InjectionKind.Delegate, _):
                    if (!parameter.Parameter.Type.GetAttributes().Any(attribute => attribute.Is(WellKnownTypes.HarmonyDelegate)))
                        Context.ReportDiagnostic(Diagnostic.Create(PatchMethodDelegateParametersMustBeAnnotatedWithHarmonyDelegateRule,
                            parameter.GetLocation(Context.CancellationToken),
                            patchMethod.Method.Name, parameter.Parameter.Name, parameter.Parameter.Type.ToDisplayString()));
                    break;

                case (InjectionKind.Instance, ParameterMatchKind.ByName):
                    if (patchMethod.TargetType is not null)
                    {
                        ITypeSymbol expectedType = patchMethod.TargetType;
                        if (patchMethod.GetTargetMethods().Any(targetMethod => targetMethod.IsStatic))
                            expectedType = expectedType.WithNullableAnnotation(NullableAnnotation.Annotated);
                        CheckOrdinaryPatchMethodParameterType(patchMethod, parameter, expectedType, isInNullableContext);
                    }

                    if (patchMethod.TargetMethod is { IsStatic: true })
                        Context.ReportDiagnostic(Diagnostic.Create(DoNotUseInstanceParameterWithStaticMethodsRule,
                            parameter.GetLocation(Context.CancellationToken),
                            patchMethod.TargetMethod.Name));

                    CheckPatchMethodParameterIsNotByRef(patchMethod, parameter);
                    break;

                case (InjectionKind.Result, _):
                    if (patchMethod.TargetMethod is not null)
                    {
                        if (patchMethod.TargetMethod.ReturnsVoid)
                            Context.ReportDiagnostic(Diagnostic.Create(DoNotUseResultWithMethodsReturningVoidRule,
                                parameter.GetLocation(Context.CancellationToken),
                                patchMethod.TargetMethod.Name));
                        else if (patchMethod.TargetMethod.ReturnsByRef)
                            Context.ReportDiagnostic(Diagnostic.Create(DoNotUseResultWithMethodsReturningByRefRule,
                                parameter.GetLocation(Context.CancellationToken),
                                patchMethod.TargetMethod.Name));
                        else
                            CheckOrdinaryPatchMethodParameterType(patchMethod, parameter, patchMethod.TargetMethod.ReturnType, isInNullableContext);
                    }

                    break;

                case (InjectionKind.ResultRef, ParameterMatchKind.ByName) when WellKnownTypes.RefResultOfT is not null:
                    if (patchMethod.TargetMethod is not null)
                    {
                        if (patchMethod.TargetMethod.ReturnsByRef)
                        {
                            CheckOrdinaryPatchMethodParameterType(patchMethod, parameter,
                                WellKnownTypes.RefResultOfT.Construct(patchMethod.TargetMethod.ReturnType), isInNullableContext);

                            if (parameter.Parameter.RefKind is not (RefKind.Ref or RefKind.Out))
                                Context.ReportDiagnostic(Diagnostic.Create(ParameterMustBeByRefRule,
                                    parameter.GetLocation(Context.CancellationToken),
                                    patchMethod.Method.Name, parameter.Parameter.Name));
                        }
                        else
                        {
                            Context.ReportDiagnostic(Diagnostic.Create(DoNotUseResultRefWithMethodsNotReturningByRefRule,
                                parameter.GetLocation(Context.CancellationToken),
                                patchMethod.TargetMethod.Name));
                        }
                    }
                    break;

                case (InjectionKind.State, ParameterMatchKind.ByName):
                    // Checked at PatchDescriptionSet level.
                    break;

                case (InjectionKind.Args, ParameterMatchKind.ByName):
                    CheckOrdinaryPatchMethodParameterType(patchMethod, parameter, WellKnownTypes.ArrayOfObject, isInNullableContext);

                    CheckPatchMethodParameterIsNotByRef(patchMethod, parameter);
                    break;

                case (InjectionKind.OriginalMethod, ParameterMatchKind.ByName):
                    CheckOrdinaryPatchMethodParameterType(patchMethod, parameter, WellKnownTypes.MethodBase, isInNullableContext);

                    CheckPatchMethodParameterIsNotByRef(patchMethod, parameter);
                    break;

                case (InjectionKind.RunOriginal, ParameterMatchKind.ByName):
                    CheckOrdinaryPatchMethodParameterType(patchMethod, parameter, WellKnownTypes.Boolean, isInNullableContext, allowObject: false);

                    CheckPatchMethodParameterIsNotByRef(patchMethod, parameter);
                    break;

                case (InjectionKind.Exception, ParameterMatchKind.ByName):
                    CheckOrdinaryPatchMethodParameterType(patchMethod, parameter, WellKnownTypes.Exception, isInNullableContext);

                    CheckPatchMethodParameterIsNotByRef(patchMethod, parameter);
                    break;
            }

            if (parameter.MatchKind != ParameterMatchKind.ByName)
                CheckPatchMethodParameterIsNotByRef(patchMethod, parameter);
        }

        private void CheckReversePatchMethodParameter(PatchMethod patchMethod, PatchMethodParameter parameter, bool isInNullableContext)
        {
            if (patchMethod.TargetType is null || patchMethod.TargetMethod is null)
                return;

            switch (parameter.Kind)
            {
                case InjectionKind.ParameterByPosition:
                    var targetMethodParameterIndex = patchMethod.Parameters.FirstOrDefault()?.Kind == InjectionKind.Instance
                        ? parameter.Parameter.Ordinal - 1
                        : parameter.Parameter.Ordinal;
                    var targetMethodParameter = patchMethod.TargetMethod.Parameters.ElementAtOrDefault(targetMethodParameterIndex);
                    if (targetMethodParameter is null)
                    {
                        Context.ReportDiagnostic(Diagnostic.Create(ReversePatchMethodParameterMustCorrespondToTargetMethodParameterRule,
                            parameter.GetLocation(Context.CancellationToken),
                            patchMethod.Method.Name, parameter.Parameter.Name, patchMethod.TargetMethod.Name));
                    }
                    else
                    {
                        if (parameter.Parameter.RefKind != targetMethodParameter.RefKind)
                        {
                            if (targetMethodParameter.RefKind == RefKind.None)
                                Context.ReportDiagnostic(Diagnostic.Create(ParameterMustNotBeByRefRule,
                                    parameter.GetLocation(Context.CancellationToken),
                                    patchMethod.Method.Name, parameter.Parameter.Name));
                            else
                                Context.ReportDiagnostic(Diagnostic.Create(ReversePatchMethodParameterMustHaveCorrectRefKindRule,
                                    parameter.GetLocation(Context.CancellationToken),
                                    patchMethod.Method.Name, parameter.Parameter.Name, patchMethod.TargetMethod.Name,
                                    RefKindToString(targetMethodParameter.RefKind)));
                        }

                        CheckReversePatchMethodParameterType(patchMethod, parameter, targetMethodParameter.Type, isInNullableContext);
                    }
                    break;

                case InjectionKind.Instance:
                    if (patchMethod.TargetMethod.IsStatic || !patchMethod.Method.IsStatic)
                    {
                        Context.ReportDiagnostic(Diagnostic.Create(InstanceParameterMustNotBePresentRule,
                            parameter.GetLocation(Context.CancellationToken),
                            patchMethod.TargetType.ToDisplayString(), patchMethod.Method.Name));
                    }
                    else
                    {
                        var expectedType = patchMethod.TargetType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
                        CheckReversePatchMethodParameterType(patchMethod, parameter, expectedType, isInNullableContext);

                        CheckPatchMethodParameterIsNotByRef(patchMethod, parameter);
                    }
                    break;
            }


            static string RefKindToString(RefKind kind) => kind switch
            {
                RefKind.Ref => "ref",
                RefKind.Out => "out",
                RefKind.RefReadOnly or RefKind.RefReadOnlyParameter => "ref readonly",
                _ => ""
            };
        }

        private void CheckOrdinaryPatchMethodParameterType(PatchMethod patchMethod, PatchMethodParameter parameter, ITypeSymbol expectedType,
            bool includeNullability, bool allowObject = true)
        {
            var exactlyEquals = expectedType.IsValueType || parameter.Parameter.RefKind is RefKind.Ref or RefKind.Out;
            if (!expectedType.IsValueType && expectedType.Is(parameter.Parameter.Type, Context.Compilation, exactlyEquals, includeNullability)) 
                return;
            if (expectedType.IsValueType && (parameter.Parameter.Type.Is(expectedType, true) ||
                                             allowObject && parameter.Parameter.Type.Is(WellKnownTypes.Object, true)))
                return;

            var rule = exactlyEquals
                ? PatchMethodParameterTypesMustBeCorrectExactRule
                : PatchMethodParameterTypesMustBeCorrectWithSupertypesRule;
            var validTypes = new List<ITypeSymbol> { expectedType };
            // For value types, only boxing to object is supported.
            if (expectedType.IsValueType && allowObject) 
                validTypes.Add(WellKnownTypes.Object);

            Context.ReportDiagnostic(Diagnostic.Create(rule,
                parameter.GetTypeLocation(Context.CancellationToken),
                patchMethod.Method.Name, parameter.Parameter.Name, GetValidTypesString(validTypes, includeNullability)));
        }

        private void CheckReversePatchMethodParameterType(PatchMethod patchMethod, PatchMethodParameter parameter, ITypeSymbol expectedType,
            bool includeNullability)
        {
            var exactlyEquals = parameter.Parameter.RefKind is RefKind.Ref or RefKind.Out;
            if (parameter.Parameter.Type.Is(expectedType, Context.Compilation, exactlyEquals, includeNullability))
                return;

            var rule = exactlyEquals
                ? PatchMethodParameterTypesMustBeCorrectExactRule
                : PatchMethodParameterTypesMustBeCorrectWithSubtypesRule;
            Context.ReportDiagnostic(Diagnostic.Create(rule,
                parameter.GetTypeLocation(Context.CancellationToken),
                patchMethod.Method.Name, parameter.Parameter.Name, GetValidTypesString([expectedType], includeNullability)));
        }

        private void CheckPatchMethodParameterIsNotByRef(PatchMethod patchMethod, PatchMethodParameter parameter)
        {
            if (parameter.Parameter.RefKind != RefKind.None)
            {
                Context.ReportDiagnostic(Diagnostic.Create(ParameterMustNotBeByRefRule,
                    parameter.GetRefLocation(Context.CancellationToken) ?? parameter.GetLocation(Context.CancellationToken),
                    patchMethod.Method.Name, parameter.Parameter.Name));
            }
        }

        private void CheckDontDefineMultipleAuxiliaryPatchMethods(PatchDescriptionSet<TPatchDescription> set)
        {
            foreach (var kind in Enum.GetValues(typeof(PatchMethodKind)).Cast<PatchMethodKind>().Where(kind => kind.IsAuxiliary()))
            {
                var patchMethods = set.PatchMethods.Where(patchMethod => patchMethod.Is(kind)).ToArray();
                if (patchMethods.Length > 1)
                    Context.ReportDiagnostic(Diagnostic.Create(DontDefineMultipleAuxiliaryPatchMethodsRule,
                        patchMethods.GetLocation(), patchMethods.GetAdditionalLocations()));
            }
        }

        private void CheckTargetMethodMustNotBeGeneric(IMethodSymbol targetMethod, PatchDescription patchDescription)
        {
            if (!targetMethod.IsGenericMethod)
                return;

            Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustNotBeGenericRule,
                patchDescription.GetLocation(IsHarmonyPatchAttribute), patchDescription.GetAdditionalLocations(IsHarmonyPatchAttribute),
                targetMethod.Name, targetMethod.ContainingType.ToDisplayString()));
        }

        protected void ReportInvalidAttributeArgument(IHasSyntax detail) =>
            Context.ReportDiagnostic(Diagnostic.Create(AttributeArgumentsMustBeValidRule, detail.Syntax?.GetLocation()));

        private void ReportInvalidAttributeArgument<T>(DetailWithSyntax<ImmutableArray<T>> detail, int arrayIndex)
        {
            var attributeArgumentSyntax = detail.Syntax as AttributeArgumentSyntax;
            SyntaxNode? itemExpressionSyntax = null;
            if (attributeArgumentSyntax?.Expression is ArrayCreationExpressionSyntax arrayCreationExpressionSyntax)
                itemExpressionSyntax = arrayCreationExpressionSyntax.Initializer?.Expressions.ElementAtOrDefault(arrayIndex);
            // Params array
            else if (attributeArgumentSyntax?.Parent is AttributeArgumentListSyntax attributeArgumentListSyntax)
            {
                var startIndex = attributeArgumentListSyntax.Arguments.IndexOf(attributeArgumentSyntax);
                itemExpressionSyntax = attributeArgumentListSyntax.Arguments[startIndex + arrayIndex];
            }
            Context.ReportDiagnostic(Diagnostic.Create(AttributeArgumentsMustBeValidRule, (itemExpressionSyntax ?? detail.Syntax)?.GetLocation()));
        }

        private static bool HasConflictingSpecifications(PatchDescription patchDescription)
        {
            if (patchDescription.TargetTypes.Length > 1 || patchDescription.MethodNames.Length > 1 || patchDescription.MethodTypes.Length > 1 ||
                patchDescription.ArgumentTypes.Length > 1 || patchDescription.ArgumentVariations.Length > 1)
                return true;

            if (patchDescription is PatchDescriptionV2 { TargetTypeNames.Length: > 1 })
                return true;

            if (patchDescription is PatchDescriptionV2 { TargetTypes: [_], TargetTypeNames: [_] })
                return true;

            return false;
        }

        private static bool IsMatch(IMethodSymbol method, ImmutableArray<ITypeSymbol?> types, ImmutableArray<ArgumentType> variations,
            Compilation compilation, bool isSetter = false)
        {
            var parameters = method.Parameters;
            if (isSetter)
                parameters = parameters[..^1];

            if (parameters.Length != types.Length)
                return false;

            Debug.Assert(types.Length == variations.Length);
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];

                if (type is null)
                    return false;

                if (variations[i] == ArgumentType.Pointer)
                    type = compilation.CreatePointerTypeSymbol(type);

                if (!parameters[i].Type.Equals(type, SymbolEqualityComparer.Default))
                    return false;

                if (variations[i] == ArgumentType.Ref && parameters[i].RefKind != RefKind.Ref)
                    return false;
                if (variations[i] == ArgumentType.Out && parameters[i].RefKind != RefKind.Out)
                    return false;
            }

            return true;
        }

        protected static bool IsValidEnumValue<TEnum>(TEnum value) where TEnum : struct => Enum.IsDefined(typeof(TEnum), value);

        private static IEnumerable<PatchMethod<TPatchDescription>> GetBulkPatchMethods(
            PatchDescriptionSet<TPatchDescription> set) =>
            set.PatchMethods.Where(patchMethod => patchMethod.MethodKinds.Any(
                detail => detail.Value is PatchMethodKind.TargetMethod or PatchMethodKind.TargetMethods));

        protected bool IsHarmonyPatchAttribute(AttributeData attribute) => attribute.Is(WellKnownTypes.HarmonyPatch);

        private string GetValidTypesString(IEnumerable<ITypeSymbol> validTypes, bool includeNullability) =>
            string.Join(", ", validTypes
                .Select(type => includeNullability ? type : StripNullableAnnotations(type))
                .Select(type => $"'{type.ToDisplayString()}'"));

        private ITypeSymbol StripNullableAnnotations(ITypeSymbol type)
        {
            type = type.WithNullableAnnotation(NullableAnnotation.None);

            if (type is INamedTypeSymbol { IsGenericType: true } namedType)
            {
                var typeArguments = namedType.TypeArguments.Select(StripNullableAnnotations).ToImmutableArray();
                return namedType.ConstructedFrom.Construct(typeArguments, [..typeArguments.Select(_ => NullableAnnotation.None)]);
            }

            if (type is IArrayTypeSymbol { LowerBounds.IsDefaultOrEmpty: true } arrayType)
                return Context.Compilation.CreateArrayTypeSymbol(StripNullableAnnotations(arrayType.ElementType), arrayType.Rank, NullableAnnotation.None);

            return type;
        }
    }

    private sealed class VerifierV1(SymbolAnalysisContext context) : Verifier<PatchDescriptionV1>(context, WellKnownTypes.Harmony1Namespace)
    {
        public override void Verify() => VerifyCore(PatchDescriptionV1.Parse(PatchType, WellKnownTypes));
    }

    private sealed class VerifierV2(SymbolAnalysisContext context) : Verifier<PatchDescriptionV2>(context, WellKnownTypes.Harmony2Namespace)
    {
        public override void Verify() => VerifyCore(PatchDescriptionV2.Parse(PatchType, WellKnownTypes));

        protected override void PreMergeChecks(PatchDescriptionV2 patchDescription)
        {
            base.PreMergeChecks(patchDescription);

            CheckAttributeArgumentsMustBeValidV2(patchDescription);
        }

        protected override void PostMergeChecks(PatchDescriptionV2 patchDescription, PatchDescriptionSet<PatchDescriptionV2> set)
        {
            base.PostMergeChecks(patchDescription, set);

            CheckMethodMustExistAndNotAmbiguousV2(patchDescription);
            CheckTypeMustExistV2(patchDescription);
            CheckMethodMustNotBeOverspecifiedV2(patchDescription);
        }

        private void CheckAttributeArgumentsMustBeValidV2(PatchDescriptionV2 patchDescription)
        {
            foreach (var detail in patchDescription.TargetTypeNames.Where(type => string.IsNullOrWhiteSpace(type.Value)))
                ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.MethodDispatchTypes.Where(type => !IsValidEnumValue(type.Value)))
                ReportInvalidAttributeArgument(detail);

            if (patchDescription.PatchCategory is { } patchCategory && string.IsNullOrEmpty(patchCategory.Value))
                ReportInvalidAttributeArgument(patchDescription.PatchCategory);
        }

        private void CheckMethodMustExistAndNotAmbiguousV2(PatchDescriptionV2 patchDescription)
        {
            if (patchDescription.TargetTypeNames is not [{ Value: not null } detail] || string.IsNullOrWhiteSpace(detail.Value))
                return;

            var targetTypeName = patchDescription.TargetTypeNames[0].Value!;
            var targetType = Context.Compilation.GetTypeByMetadataName(targetTypeName);
            if (targetType is not null)
                CheckMethodMustExistAndNotAmbiguous(patchDescription, targetType);
        }

        private void CheckTypeMustExistV2(PatchDescriptionV2 patchDescription)
        {
            if (patchDescription.TargetTypeNames is not [{ Value: not null } detail] || string.IsNullOrWhiteSpace(detail.Value))
                return;

            var targetTypeName = patchDescription.TargetTypeNames[0].Value!;
            var targetType = Context.Compilation.GetTypeByMetadataName(targetTypeName);
            if (targetType is null)
                Context.ReportDiagnostic(Diagnostic.Create(TargetTypeMustExistRule,
                    patchDescription.GetLocation(IsHarmonyPatchAttribute), patchDescription.GetAdditionalLocations(IsHarmonyPatchAttribute),
                    targetTypeName));
        }

        private void CheckMethodMustNotBeOverspecifiedV2(PatchDescriptionV2 patchDescription)
        {
            if (patchDescription.MethodDispatchTypes.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(TargetMethodMustNotBeOverspecifiedRule,
                    patchDescription.MethodDispatchTypes.GetLocation(), patchDescription.MethodDispatchTypes.GetAdditionalLocations()));
        }
    }
}