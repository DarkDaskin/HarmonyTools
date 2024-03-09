namespace HarmonyTools.Analyzers;

public static class DiagnosticIds
{
    public const string TargetMethodMustExist = "HT1001";
    public const string TargetMethodMustNotBeAmbiguous = "HT1002";
    public const string TargetTypeMustExist = "HT1003";
    public const string TargetMethodMustBeFullySpecified = "HT1004";
    public const string TargetMethodMustNotBeOverspecified = "HT1005";
    public const string AttributeArgumentsMustBeValid = "HT1006";
    public const string ArgumentTypesAndVariationsMustMatch = "HT1007";
    public const string HarmonyPatchAttributeMustBeOnType = "HT1008";
    public const string DontUseIndividualAnnotationsWithBulkPatching = "HT1009";
    public const string DontUseMultipleBulkPatchingMethods = "HT1010";
    public const string DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethods = "HT1011";

    public const string PatchMethodsMustBeStatic = "HT2001";
    public const string PatchMethodMustHaveSingleKind = "HT2002";
    public const string DontDefineMultipleAuxiliaryPatchMethods = "HT2003";
}