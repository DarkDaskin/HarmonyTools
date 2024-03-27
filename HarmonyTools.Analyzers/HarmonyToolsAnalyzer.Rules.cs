using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
#pragma warning disable RS2008

namespace HarmonyTools.Analyzers;

public partial class HarmonyToolsAnalyzer
{
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
    private static readonly DiagnosticDescriptor DontUseBulkPatchingMethodsWithReversePatchesRule =
        CreateRule(DiagnosticIds.DontUseBulkPatchingMethodsWithReversePatches,
            nameof(Resources.DontUseBulkPatchingMethodsWithReversePatchesTitle), 
            nameof(Resources.DontUseBulkPatchingMethodsWithReversePatchesMessageFormat),
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
    private static readonly DiagnosticDescriptor MultipleArgumentsMustNotTargetSameParameterRule =
        CreateRule(DiagnosticIds.MultipleArgumentsMustNotTargetSameParameter,
            nameof(Resources.MultipleArgumentsMustNotTargetSameParameterTitle), 
            nameof(Resources.MultipleArgumentsMustNotTargetSameParameterMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor ArgumentNewNamesMustCorrespondToParameterNamesRule =
        CreateRule(DiagnosticIds.ArgumentNewNamesMustCorrespondToParameterNames,
            nameof(Resources.ArgumentNewNamesMustCorrespondToParameterNamesTitle),
            nameof(Resources.ArgumentNewNamesMustCorrespondToParameterNamesMessageFormat),
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
    private static readonly DiagnosticDescriptor DontUseInstanceParameterWithStaticMethodsRule =
        CreateRule(DiagnosticIds.DontUseInstanceParameterWithStaticMethods,
            nameof(Resources.DontUseInstanceParameterWithStaticMethodsTitle),
            nameof(Resources.DontUseInstanceParameterWithStaticMethodsMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DontUseResultRefWithMethodsNotReturningByRefRule =
        CreateRule(DiagnosticIds.DontUseResultRefWithMethodsNotReturningByRef,
            nameof(Resources.DontUseResultRefWithMethodsNotReturningByRefTitle),
            nameof(Resources.DontUseResultRefWithMethodsNotReturningByRefMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DontUseResultWithMethodsReturningByRefRule =
        CreateRule(DiagnosticIds.DontUseResultWithMethodsReturningByRef,
            nameof(Resources.DontUseResultWithMethodsReturningByRefTitle),
            nameof(Resources.DontUseResultWithMethodsReturningByRefMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor DontUseResultWithMethodsReturningVoidRule =
        CreateRule(DiagnosticIds.DontUseResultWithMethodsReturningVoid,
            nameof(Resources.DontUseResultWithMethodsReturningVoidTitle),
            nameof(Resources.DontUseResultWithMethodsReturningVoidMessageFormat),
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
    private static readonly DiagnosticDescriptor DontUseArgumentsWithSpecialParametersRule =
        CreateRule(DiagnosticIds.DontUseArgumentsWithSpecialParameters,
            nameof(Resources.DontUseArgumentsWithSpecialParametersTitle), nameof(Resources.DontUseArgumentsWithSpecialParametersMessageFormat),
            PatchMethodCategory, DiagnosticSeverity.Warning);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    [
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
        DontUseBulkPatchingMethodsWithReversePatchesRule,

        PatchMethodsMustBeStaticRule,
        PatchMethodMustHaveSingleKindRule,
        DontDefineMultipleAuxiliaryPatchMethodsRule,
        PatchMethodsMustNotBeGenericRule,
        ArgumentsOnTypesAndMethodsMustHaveNewNameRule,
        MultipleArgumentsMustNotTargetSameParameterRule,
        ArgumentNewNamesMustCorrespondToParameterNamesRule,
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
        DontUseInstanceParameterWithStaticMethodsRule,
        DontUseResultRefWithMethodsNotReturningByRefRule,
        DontUseResultWithMethodsReturningByRefRule,
        DontUseResultWithMethodsReturningVoidRule,
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
        DontUseArgumentsWithSpecialParametersRule,
    ];

    private static DiagnosticDescriptor CreateRule(string id, string titleResource, string messageFormatResource,
        string category, DiagnosticSeverity severity, bool isEnabledByDefault = true, string? descriptionResource = null) =>
        new(id,
            new LocalizableResourceString(titleResource, Resources.ResourceManager, typeof(Resources)),
            new LocalizableResourceString(messageFormatResource, Resources.ResourceManager, typeof(Resources)),
            category, severity, isEnabledByDefault,
            descriptionResource == null
                ? null
                : new LocalizableResourceString(descriptionResource, Resources.ResourceManager, typeof(Resources)));
}