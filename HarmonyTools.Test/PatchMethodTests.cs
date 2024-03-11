using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyTools.Analyzers;
using HarmonyTools.Test.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = HarmonyTools.Test.Verifiers.CSharpCodeFixVerifier<
    HarmonyTools.Analyzers.HarmonyToolsAnalyzer,
    HarmonyTools.CodeFixes.PatchMethodsMustBeStaticCodeFixProvider>;

namespace HarmonyTools.Test;

[TestClass, CodeDirectory("PatchMethod")]
public class PatchMethodTests
{
    [TestMethod, CodeDataSource("MultiplePatchMethodKinds.cs")]
    public async Task WhenMultiplePatchMethodKinds_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.PatchMethodMustHaveSingleKind, DiagnosticSeverity.Warning)
                .WithSpan(9, 10, 9, 24)
                .WithSpan(10, 28, 10, 34),
            new DiagnosticResult(DiagnosticIds.PatchMethodMustHaveSingleKind, DiagnosticSeverity.Warning)
                .WithSpan(12, 10, 12, 24)
                .WithSpan(12, 26, 12, 40)
                .WithSpan(12, 42, 12, 55));
    }

    [TestMethod, CodeDataSource("MultipleAuxiliaryPatchMethods.cs")]
    public async Task WhenMultipleAuxiliaryPatchMethods_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.DontDefineMultipleAuxiliaryPatchMethods, DiagnosticSeverity.Warning)
                .WithSpan(9, 28, 9, 35)
                .WithSpan(12, 28, 12, 36));
    }

    [TestMethod, CodeDataSource("NonStaticPatchMethods.cs", FixedPath = "NonStaticPatchMethods_Fixed.cs", ProvideVersion = true)]
    public async Task WhenNonStaticPatchMethods_ReportAndFix(string code, ReferenceAssemblies referenceAssemblies, string fixedCode, int version)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(9, 21, 9, 27),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(11, 21, 11, 28),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(13, 21, 13, 31),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(15, 21, 15, 28),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(17, 21, 17, 28),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(19, 27, 19, 39),
        };
        if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                    .WithSpan(21, 21, 21, 30),
                new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                    .WithSpan(23, 21, 23, 33),
            ]);

        await VerifyCS.VerifyCodeFixAsync(code, referenceAssemblies, expected.ToArray(), fixedCode);
    }

    [TestMethod, CodeDataSource("GenericPatchMethod.cs")]
    public async Task WhenGenericPatchMethod_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotBeGeneric, DiagnosticSeverity.Warning)
                .WithSpan(9, 28, 9, 35));
    }
}