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
    public const string ArgumentNewNamesMustBeUnique = "HT2006";
    public const string PatchMethodReturnTypesMustBeCorrect = "HT2007";
}