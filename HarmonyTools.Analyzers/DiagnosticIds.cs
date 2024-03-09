namespace HarmonyTools.Analyzers;

public static class DiagnosticIds
{
    public const string MethodMustExist = "HT1001";
    public const string MethodMustNotBeAmbiguous = "HT1002";
    public const string TypeMustExist = "HT1003";
    public const string MethodMustBeSpecified = "HT1004";
    public const string MethodMustNotBeOverspecified = "HT1005";
    public const string AttributeArgumentsMustBeValid = "HT1006";
    public const string ArgumentTypesAndVariationsMustMatch = "HT1007";
    public const string HarmonyPatchAttributeMustBeOnType = "HT1008";
    public const string DontUseIndividualAnnotationsWithBulkPatching = "HT1009";
    public const string DontUseMultipleBulkPatchingMethods = "HT1010";

    public const string PatchMethodMustHaveSingleKind = "HT2001";
    public const string DontDefineMultipleAuxiliaryPatchMethods = "HT2002";
}