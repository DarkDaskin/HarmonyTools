using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyTools.Analyzers;
using HarmonyTools.CodeFixes;
using HarmonyTools.Test.Infrastructure;
using HarmonyTools.Test.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = HarmonyTools.Test.Verifiers.CSharpAnalyzerVerifier<
    HarmonyTools.Analyzers.HarmonyToolsAnalyzer>;

namespace HarmonyTools.Test;

[TestClass, CodeDirectory("TargetMethod")]
public class TargetMethodTests
{
    [TestMethod, CodeDataSource("ValidTypeLevelPatches.cs")]
    public async Task WhenValidTypeLevelPatches_DoNothing(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies);
    }

    [TestMethod, CodeDataSource("ValidMethodLevelPatches.cs")]
    public async Task WhenValidMethodLevelPatches_DoNothing(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies);
    }

    [TestMethod, CodeDataSource("ValidMixedLevelPatches.cs")]
    public async Task WhenValidMixedLevelPatches_DoNothing(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies);
    }

    [TestMethod, CodeDataSource("ValidBulkPatches.cs")]
    public async Task WhenValidBulkPatches_DoNothing(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies);
    }

    [TestMethod, CodeDataSource("ValidDelegates.cs")]
    public async Task WhenValidDelegates_DoNothing(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies);
    }

    [TestMethod, CodeDataSource("NonExistingMethods.cs", ProvideVersion = true)]
    public async Task WhenNonExistingMethods_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(6, 6, 6, 60)
                .WithArguments("NonExistingMethod", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(12, 6, 12, 93)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(18, 6, 19, 61)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(25, 6, 25, 79)
                .WithArguments(".ctor", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(31, 6, 31, 77)
                .WithArguments(".cctor", "HarmonyTools.Test.PatchBase.NoStaticConstructor"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(37, 6, 37, 77)
                .WithArguments("get_NonExistingProp", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(43, 6, 43, 92)
                .WithArguments("set_ReadOnlyProp", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(49, 6, 49, 74)
                .WithArguments("get_Item", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(58, 10, 58, 43)
                .WithArguments("NonExistingMethod", "HarmonyTools.Test.PatchBase.SimpleClass"),
        };
        if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(62, 6, 62, 99)
                    .WithSpan(63, 6, 63, 44)
                    .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass"),
                new DiagnosticResult(DiagnosticIds.TargetMethodMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(69, 6, 69, 88)
                    .WithArguments("NonExistingMethod", "HarmonyTools.Test.PatchBase.SimpleClass"),
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("AmbiguousMatches.cs", ProvideVersion = true)]
    public async Task WhenAmbiguousMatches_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(6, 6, 6, 77)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass", "3"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(12, 6, 12, 58)
                .WithArguments("get_Item", "HarmonyTools.Test.PatchBase.SimpleClass", "2"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(18, 6, 18, 72)
                .WithArguments(".ctor", "HarmonyTools.Test.PatchBase.MultipleConstructors", "2"),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(27, 10, 27, 81)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass", "3"),
        };
        if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                    .WithSpan(31, 6, 31, 99)
                    .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass", "3"),
                new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                    .WithSpan(37, 6, 37, 105)
                    .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass", "3"),
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("NonExistingTypes.cs")]
    public async Task WhenNonExistingTypes_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.TargetTypeMustExist, DiagnosticSeverity.Warning)
                .WithSpan(5, 6, 5, 76)
                .WithArguments("HarmonyTools.Test.PatchBase.NonExistingClass"),
            new DiagnosticResult(DiagnosticIds.TargetTypeMustExist, DiagnosticSeverity.Warning)
                .WithSpan(14, 10, 14, 80)
                .WithArguments("HarmonyTools.Test.PatchBase.NonExistingClass"));
    }

    [TestMethod, CodeDataSource("UnspecifiedMethods.cs", ProvideVersion = true)]
    public async Task WhenUnspecifiedMethods_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.TargetMethodMustBeFullySpecified, DiagnosticSeverity.Warning)
                .WithSpan(6, 6, 6, 18),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustBeFullySpecified, DiagnosticSeverity.Warning)
                .WithSpan(12, 6, 12, 39),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustBeFullySpecified, DiagnosticSeverity.Warning)
                .WithSpan(18, 6, 18, 52),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustBeFullySpecified, DiagnosticSeverity.Warning)
                .WithSpan(24, 6, 24, 71),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustBeFullySpecified, DiagnosticSeverity.Warning)
                .WithSpan(30, 6, 30, 42),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustBeFullySpecified, DiagnosticSeverity.Warning)
                .WithSpan(36, 6, 36, 44),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustBeFullySpecified, DiagnosticSeverity.Warning)
                .WithSpan(45, 10, 45, 43),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustBeFullySpecified, DiagnosticSeverity.Warning)
                .WithSpan(52, 10, 52, 41),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustBeFullySpecified, DiagnosticSeverity.Warning)
                .WithSpan(56, 6, 56, 21),
        };
        if (version == 2)
            expected.Add(new DiagnosticResult(DiagnosticIds.TargetMethodMustBeFullySpecified, DiagnosticSeverity.Warning)
                .WithSpan(62, 6, 62, 42));

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("OverspecifiedMethods.cs", ProvideVersion = true)]
    public async Task WhenOverspecifiedMethods_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(6, 19, 6, 38)
                .WithSpan(7, 19, 7, 38),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(13, 40, 13, 72)
                .WithSpan(14, 19, 14, 55),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(20, 40, 20, 72)
                .WithSpan(21, 19, 21, 52),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(27, 78, 27, 89)
                .WithSpan(28, 19, 28, 59),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(34, 78, 34, 112)
                .WithSpan(36, 19, 36, 53),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(35, 9, 35, 59)
                .WithSpan(36, 55, 36, 105),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(42, 74, 42, 91)
                .WithSpan(43, 19, 43, 36)
                .WithSpan(44, 19, 44, 36),
            new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(53, 44, 53, 76)
                .WithSpan(53, 92, 53, 128),
        };
        if (version == 2)
            expected.AddRange([
                new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                    .WithSpan(57, 19, 57, 60)
                    .WithSpan(58, 19, 58, 60),
                new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                    .WithSpan(57, 62, 57, 94)
                    .WithSpan(58, 62, 58, 98),
                new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                    .WithSpan(64, 19, 64, 60)
                    .WithSpan(65, 19, 65, 38),
                new DiagnosticResult(DiagnosticIds.TargetMethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                    .WithSpan(71, 77, 71, 100)
                    .WithSpan(72, 22, 72, 52),
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("ArgumentTypesAndVariationsMismatch.cs", ProvideVersion = true)]
    public async Task WhenArgumentTypesAndVariationsMismatch_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.ArgumentTypesAndVariationsMustMatch, DiagnosticSeverity.Warning)
                .WithSpan(7, 19, 7, 40)
                .WithSpan(7, 42, 7, 92),
            new DiagnosticResult(DiagnosticIds.ArgumentTypesAndVariationsMustMatch, DiagnosticSeverity.Warning)
                .WithSpan(14, 19, 14, 53)
                .WithSpan(14, 55, 14, 84),
            new DiagnosticResult(DiagnosticIds.ArgumentTypesAndVariationsMustMatch, DiagnosticSeverity.Warning)
                .WithSpan(23, 23, 23, 57)
                .WithSpan(23, 59, 23, 88),
        };
        if (version == 2)
            expected.Add(new DiagnosticResult(DiagnosticIds.ArgumentTypesAndVariationsMustMatch, DiagnosticSeverity.Warning)
                .WithSpan(28, 47, 28, 81)
                .WithSpan(28, 83, 28, 112));

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("IndividualAnnotationsWithBulkPatching.cs")]
    public async Task WhenIndividualAnnotationsWithBulkPatching_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.DontUseIndividualAnnotationsWithBulkPatching, DiagnosticSeverity.Warning)
                .WithSpan(8, 6, 8, 73)
                .WithSpan(8, 75, 8, 90),
            new DiagnosticResult(DiagnosticIds.DontUseIndividualAnnotationsWithBulkPatching, DiagnosticSeverity.Warning)
                .WithSpan(14, 6, 14, 21)
                .WithSpan(17, 10, 17, 77),
            new DiagnosticResult(DiagnosticIds.DontUseIndividualAnnotationsWithBulkPatching, DiagnosticSeverity.Warning)
                .WithSpan(21, 6, 21, 63)
                .WithSpan(21, 65, 21, 80),
            new DiagnosticResult(DiagnosticIds.DontUseIndividualAnnotationsWithBulkPatching, DiagnosticSeverity.Warning)
                .WithSpan(27, 6, 27, 81)
                .WithSpan(27, 83, 27, 98),
            new DiagnosticResult(DiagnosticIds.DontUseIndividualAnnotationsWithBulkPatching, DiagnosticSeverity.Warning)
                .WithSpan(33, 6, 33, 39)
                .WithSpan(36, 34, 36, 46),
            new DiagnosticResult(DiagnosticIds.DontUseIndividualAnnotationsWithBulkPatching, DiagnosticSeverity.Warning)
                .WithSpan(41, 6, 41, 39)
                .WithSpan(44, 47, 44, 60),
            new DiagnosticResult(DiagnosticIds.DontUseIndividualAnnotationsWithBulkPatching, DiagnosticSeverity.Warning)
                .WithSpan(49, 6, 49, 39)
                .WithSpan(49, 41, 49, 87)
                .WithSpan(52, 10, 52, 29));
    }

    [TestMethod, CodeDataSource("MultipleBulkPatchingMethods.cs")]
    public async Task WhenMultipleBulkPatchingMethods_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.DontUseMultipleBulkPatchingMethods, DiagnosticSeverity.Warning)
                .WithSpan(7, 6, 7, 21)
                .WithSpan(10, 34, 10, 46),
            new DiagnosticResult(DiagnosticIds.DontUseMultipleBulkPatchingMethods, DiagnosticSeverity.Warning)
                .WithSpan(16, 34, 16, 46)
                .WithSpan(18, 47, 18, 60));
    }

    [TestMethod, CodeDataSource("TargetMethodAnnotationsOnNonPrimaryPatchMethods.cs", 
         FixedPath = "TargetMethodAnnotationsOnNonPrimaryPatchMethods_Fixed.cs")]
    public async Task WhenTargetMethodAnnotationsOnNonPrimaryPatchMethods_ReportAndFix(
        string code, ReferenceAssemblies referenceAssemblies, string fixedCode)
    {
        await CSharpCodeFixVerifier<HarmonyToolsAnalyzer, DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethodsCodeFixProvider>
            .VerifyCodeFixAsync(code, referenceAssemblies,
            [
                new DiagnosticResult(DiagnosticIds.DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethods, DiagnosticSeverity.Warning)
                    .WithSpan(12, 26, 12, 59)
                    .WithSpan(12, 61, 12, 107),
                new DiagnosticResult(DiagnosticIds.DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethods, DiagnosticSeverity.Warning)
                    .WithSpan(15, 10, 15, 43)
                    .WithSpan(15, 45, 15, 91),
            ], fixedCode);
    }

    [TestMethod, CodeDataSource("InvalidTargetTypes.cs")]
    public async Task WhenInvalidTargetTypes_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.TargetTypeMustBeNamedType, DiagnosticSeverity.Warning)
                .WithSpan(6, 19, 6, 32)
                .WithArguments("int[]"),
            new DiagnosticResult(DiagnosticIds.TargetTypeMustBeNamedType, DiagnosticSeverity.Warning)
                .WithSpan(12, 19, 12, 31)
                .WithArguments("int*"),
            new DiagnosticResult(DiagnosticIds.TargetTypeMustNotBeOpenGenericType, DiagnosticSeverity.Warning)
                .WithSpan(18, 19, 18, 33)
                .WithArguments("System.Collections.Generic.List<>"));
    }
}