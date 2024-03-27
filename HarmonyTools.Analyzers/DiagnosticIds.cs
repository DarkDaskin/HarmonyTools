namespace HarmonyTools.Analyzers;

public static class DiagnosticIds
{
    // General
    public const string AttributeArgumentsMustBeValid = "HT0001";
    public const string HarmonyPatchAttributeMustBeOnType = "HT0002";
    public const string PatchTypeMustNotBeGeneric = "HT0003";

    // TargetMethod
    public const string TargetMethodMustExist = "HT1001";
    public const string TargetMethodMustNotBeAmbiguous = "HT1002";
    public const string TargetTypeMustExist = "HT1003";
    public const string TargetMethodMustBeFullySpecified = "HT1004";
    public const string TargetMethodMustNotBeOverspecified = "HT1005";
    public const string ArgumentTypesAndVariationsMustMatch = "HT1006";
    public const string DontUseIndividualAnnotationsWithBulkPatching = "HT1007";
    public const string DontUseMultipleBulkPatchingMethods = "HT1008";
    public const string DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethods = "HT1009";
    public const string TargetTypeMustBeNamedType = "HT1010";
    public const string TargetTypeMustNotBeOpenGenericType = "HT1011";
    public const string TargetMethodMustNotBeGeneric = "HT1012";

    // PatchMethod
    public const string PatchMethodsMustBeStatic = "HT2001";
    public const string PatchMethodMustHaveSingleKind = "HT2002";
    public const string DontDefineMultipleAuxiliaryPatchMethods = "HT2003";
    public const string PatchMethodsMustNotBeGeneric = "HT2004";
    public const string ArgumentsOnTypesAndMethodsMustHaveNewName = "HT2005";
    public const string MultipleArgumentsMustNotTargetSameParameter = "HT2006";
    public const string ArgumentNewNamesMustCorrespondToParameterNames = "HT2007";
    public const string PatchMethodReturnTypesMustBeCorrect = "HT2008";
    public const string PatchMethodParameterTypesMustBeCorrect = "HT2009";
    public const string PatchMethodParametersMustBeValidInjections = "HT2010";
    public const string TargetMethodParameterWithSpecifiedNameMustExist = "HT2011";
    public const string TargetMethodParameterWithSpecifiedIndexMustExist = "HT2012";
    public const string TargetTypeFieldWithSpecifiedNameMustExist = "HT2013";
    public const string TargetTypeFieldWithSpecifiedIndexMustExist = "HT2014";
    public const string PatchMethodDelegateParametersMustBeAnnotatedWithHarmonyDelegate = "HT2015";
    public const string DoNotUseInstanceParameterWithStaticMethods = "HT2016";
    public const string DoNotUseResultRefWithMethodsNotReturningByRef = "HT2017";
    public const string DoNotUseResultWithMethodsReturningByRef = "HT2018";
    public const string DoNotUseResultWithMethodsReturningVoid = "HT2019";
    public const string ParameterMustBeByRef = "HT2020";
    public const string ParameterMustNotBeByRef = "HT2021";
    public const string StateTypeMustNotDiffer = "HT2022";
    public const string StateShouldBeInitialized = "HT2023";
    public const string ReversePatchMethodParameterMustCorrespondToTargetMethodParameter = "HT2024";
    public const string ReversePatchMethodParameterMustHaveCorrectRefKind = "HT2025";
    public const string AllTargetMethodParametersMustBeIncluded = "HT2026";
    public const string InstanceParameterMustBePresent = "HT2027";
    public const string InstanceParameterMustNotBePresent = "HT2028";
    public const string PatchMethodsMustNotReturnByRef = "HT2029";
    public const string DoNotUseArgumentsWithSpecialParameters = "HT2030";
}