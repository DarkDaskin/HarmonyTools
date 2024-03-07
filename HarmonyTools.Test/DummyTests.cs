﻿using System.Collections.Generic;
using HarmonyTools.Test.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using HarmonyTools.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using VerifyCS = HarmonyTools.Test.CSharpCodeFixVerifier<
    HarmonyTools.Analyzers.HarmonyToolsAnalyzer,
    HarmonyTools.HarmonyToolsCodeFixProvider>;

namespace HarmonyTools.Test;

[TestClass]
public class DummyTests
{
    [TestMethod]
    public async Task WhenEmptyFile_DoNothing()
    {
        await VerifyCS.VerifyAnalyzerAsync("");
    }

    [TestMethod, CodeDataSource("NoPatches.cs")]
    public async Task WhenNoAttributes_DoNothing(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies);
    }

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

    [TestMethod, CodeDataSource("NonExistingMethods.cs")]
    public async Task WhenNonExistingMethods_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(6, 6, 6, 60)
                .WithArguments("NonExistingMethod", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(12, 6, 12, 93)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(18, 6, 19, 61)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(25, 6, 25, 79)
                .WithArguments(".ctor", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(31, 6, 31, 77)
                .WithArguments(".cctor", "HarmonyTools.Test.PatchBase.NoStaticConstructor"),
            new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(37, 6, 37, 77)
                .WithArguments("get_NonExistingProp", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(43, 6, 43, 92)
                .WithArguments("set_ReadOnlyProp", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(49, 6, 49, 74)
                .WithArguments("get_Item", "HarmonyTools.Test.PatchBase.SimpleClass"),
            new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(58, 10, 58, 43)
                .WithArguments("NonExistingMethod", "HarmonyTools.Test.PatchBase.SimpleClass"),
        };
        if (referenceAssemblies.GetHarmonyVersion() == 2)
            expected.Add(new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(62, 6, 62, 99)
                .WithSpan(63, 6, 63, 44)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass"));

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("AmbiguousMatches.cs")]
    public async Task WhenAmbiguousMatches_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(6, 6, 6, 77)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass", "3"),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(12, 6, 12, 58)
                .WithArguments("get_Item", "HarmonyTools.Test.PatchBase.SimpleClass", "2"),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(18, 6, 18, 72)
                .WithArguments(".ctor", "HarmonyTools.Test.PatchBase.MultipleConstructors", "2"),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(27, 10, 27, 81)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass", "3"),
        };
        if (referenceAssemblies.GetHarmonyVersion() == 2)
            expected.Add(new DiagnosticResult(DiagnosticIds.MethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(31, 6, 31, 99)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass", "3"));

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("NonExistingTypes.cs")]
    public async Task WhenNonExistingTypes_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.TypeMustExist, DiagnosticSeverity.Warning)
                .WithSpan(5, 6, 5, 76)
                .WithArguments("HarmonyTools.Test.PatchBase.NonExistingClass"),
            new DiagnosticResult(DiagnosticIds.TypeMustExist, DiagnosticSeverity.Warning)
                .WithSpan(14, 10, 14, 80)
                .WithArguments("HarmonyTools.Test.PatchBase.NonExistingClass"));
    }

    [TestMethod, CodeDataSource("UnspecifiedMethods.cs")]
    public async Task WhenUnspecifiedMethods_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.MethodMustBeSpecified, DiagnosticSeverity.Warning)
                .WithSpan(6, 6, 6, 18),
            new DiagnosticResult(DiagnosticIds.MethodMustBeSpecified, DiagnosticSeverity.Warning)
                .WithSpan(12, 6, 12, 39),
            new DiagnosticResult(DiagnosticIds.MethodMustBeSpecified, DiagnosticSeverity.Warning)
                .WithSpan(18, 6, 18, 52),
            new DiagnosticResult(DiagnosticIds.MethodMustBeSpecified, DiagnosticSeverity.Warning)
                .WithSpan(24, 6, 24, 71),
            new DiagnosticResult(DiagnosticIds.MethodMustBeSpecified, DiagnosticSeverity.Warning)
                .WithSpan(30, 6, 30, 42),
            new DiagnosticResult(DiagnosticIds.MethodMustBeSpecified, DiagnosticSeverity.Warning)
                .WithSpan(36, 6, 36, 44),
            new DiagnosticResult(DiagnosticIds.MethodMustBeSpecified, DiagnosticSeverity.Warning)
                .WithSpan(45, 10, 45, 43),
            new DiagnosticResult(DiagnosticIds.MethodMustBeSpecified, DiagnosticSeverity.Warning)
                .WithSpan(52, 10, 52, 41));
    }

    [TestMethod, CodeDataSource("OverspecifiedMethods.cs")]
    public async Task WhenOverspecifiedMethods_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(6, 19, 6, 38)
                .WithSpan(7, 19, 7, 38),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(13, 40, 13, 72)
                .WithSpan(14, 19, 14, 55),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(20, 40, 20, 72)
                .WithSpan(21, 19, 21, 52),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(27, 78, 27, 89)
                .WithSpan(28, 19, 28, 59),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(34, 78, 34, 112)
                .WithSpan(36, 19, 36, 53),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(35, 9, 35, 59)
                .WithSpan(36, 55, 36, 105),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(42, 74, 42, 91)
                .WithSpan(43, 19, 43, 36)
                .WithSpan(44, 19, 44, 36),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(53, 44, 53, 76)
                .WithSpan(53, 92, 53, 128),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                .WithSpan(57, 40, 57, 72)
                .WithSpan(60, 23, 60, 59),
        };
        if (referenceAssemblies.GetHarmonyVersion() == 2)
            expected.AddRange([
                new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                    .WithSpan(64, 19, 64, 60)
                    .WithSpan(65, 19, 65, 60),
                new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                    .WithSpan(64, 62, 64, 94)
                    .WithSpan(65, 62, 65, 98),
                new DiagnosticResult(DiagnosticIds.MethodMustNotBeOverspecified, DiagnosticSeverity.Warning)
                    .WithSpan(71, 19, 71, 60)
                    .WithSpan(72, 19, 72, 38),
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("InvalidArguments.cs")]
    public async Task WhenInvalidArguments_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(7, 19, 7, 29),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(7, 31, 7, 43),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(7, 45, 7, 57),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(7, 59, 7, 79),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(14, 32, 14, 36),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(14, 61, 14, 78),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(20, 19, 20, 29),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(20, 45, 20, 57),
        };
        if (referenceAssemblies.GetHarmonyVersion() == 1)
            expected.AddRange([
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(20, 59, 20, 72),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(27, 53, 27, 72),
            ]);
        if (referenceAssemblies.GetHarmonyVersion() == 2)
            expected.AddRange([
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(20, 59, 20, 74),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(26, 19, 26, 31),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(26, 33, 26, 45),
            ]);
        expected.AddRange([
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(37, 23, 37, 33),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(37, 35, 37, 47),
        ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("ArgumentTypesAndVariationsMismatch.cs")]
    public async Task WhenArgumentTypesAndVariationsMismatch_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.ArgumentTypesAndVariationsMustMatch, DiagnosticSeverity.Warning)
                .WithSpan(7, 19, 7, 40)
                .WithSpan(7, 42, 7, 92),
            new DiagnosticResult(DiagnosticIds.ArgumentTypesAndVariationsMustMatch, DiagnosticSeverity.Warning)
                .WithSpan(14, 19, 14, 53)
                .WithSpan(14, 55, 14, 84),
            new DiagnosticResult(DiagnosticIds.ArgumentTypesAndVariationsMustMatch, DiagnosticSeverity.Warning)
                .WithSpan(23, 23, 23, 57)
                .WithSpan(23, 59, 23, 88));
    }
}