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

[TestClass, CodeDirectory("General")]
public class GeneralTests
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

    [TestMethod, CodeDataSource("InvalidArguments.cs", ProvideVersion = true)]
    public async Task WhenInvalidArguments_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
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
                .WithSpan(23, 23, 23, 33),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(23, 35, 23, 47),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(27, 40, 27, 42),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(33, 19, 33, 29),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(33, 45, 33, 57),
        };
        if (version == 1)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(33, 59, 33, 72),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(40, 53, 40, 72),
            ]);
        else if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(33, 59, 33, 74),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(39, 19, 39, 31),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(39, 33, 39, 45),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(45, 19, 45, 21),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(45, 23, 45, 25),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(51, 96, 51, 100),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(57, 96, 57, 98),
                new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                    .WithSpan(63, 77, 63, 98),
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("InvalidArguments2.cs")]
    public async Task WhenInvalidArguments2_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(7, 6, 7, 19),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(7, 21, 7, 33),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(14, 20, 14, 24),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(14, 40, 14, 44),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(21, 27, 21, 31),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(21, 33, 21, 35),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(21, 58, 21, 62),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(21, 64, 21, 66),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(28, 22, 28, 26),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(28, 45, 28, 49),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(28, 51, 28, 55),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(29, 22, 29, 24),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(29, 43, 29, 45),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(29, 47, 29, 49),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(30, 22, 30, 24),
            new DiagnosticResult(DiagnosticIds.AttributeArgumentsMustBeValid, DiagnosticSeverity.Warning)
                .WithSpan(30, 43, 30, 45));
    }
    
    [TestMethod, CodeDataSource("MissingHarmonyPatchOnType.cs", FixedPath = "MissingHarmonyPatchOnType_Fixed.cs")]
    public async Task WhenMissingHarmonyPatchOnType_ReportAndFix(string code, ReferenceAssemblies referenceAssemblies, string fixedCode)
    {
        await CSharpCodeFixVerifier<HarmonyToolsAnalyzer, HarmonyPatchAttributeMustBeOnTypeCodeFixProvider>
            .VerifyCodeFixAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.HarmonyPatchAttributeMustBeOnType, DiagnosticSeverity.Warning)
                .WithSpan(6, 20, 6, 45),
            fixedCode);
    }

    [TestMethod, CodeDataSource("GenericPatchType.cs")]
    public async Task WhenGenericPatchType_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.PatchTypeMustNotBeGeneric, DiagnosticSeverity.Warning)
                .WithSpan(7, 20, 7, 36));
    }
}