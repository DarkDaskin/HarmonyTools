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
                .WithSpan(10, 21, 10, 27),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(12, 21, 12, 28),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(14, 45, 14, 55),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(16, 21, 16, 28),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(18, 21, 18, 28),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                .WithSpan(20, 27, 20, 39),
        };
        if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                    .WithSpan(22, 21, 22, 30),
                new DiagnosticResult(DiagnosticIds.PatchMethodsMustBeStatic, DiagnosticSeverity.Warning)
                    .WithSpan(24, 21, 24, 33),
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

    [TestMethod, CodeDataSource("MissingArgumentNewName.cs")]
    public async Task WhenMissingArgumentNewName_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.ArgumentsOnTypesAndMethodsMustHaveNewName, DiagnosticSeverity.Warning)
                .WithSpan(7, 6, 7, 24),
            new DiagnosticResult(DiagnosticIds.ArgumentsOnTypesAndMethodsMustHaveNewName, DiagnosticSeverity.Warning)
                .WithSpan(10, 10, 10, 30));
    }

    [TestMethod, CodeDataSource("DuplicateArgumentNewName.cs")]
    public async Task WhenDuplicateArgumentNewName_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.ArgumentNewNamesMustBeUnique, DiagnosticSeverity.Warning)
                .WithSpan(7, 25, 7, 30)
                .WithSpan(10, 29, 10, 34)
                .WithArguments("foo"));
    }

    [TestMethod, CodeDataSource("ValidPatchMethodReturnTypes.cs")]
    public async Task WhenValidPatchMethodReturnTypes_DoNothing(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies);
    }

    [TestMethod, CodeDataSource("InvalidPatchMethodReturnTypes.cs", ProvideVersion = true)]
    public async Task WhenInvalidPatchMethodReturnTypes_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
    {
        var enumerableOfCodeInstruction = version == 2
            ? "'System.Collections.Generic.IEnumerable<HarmonyLib.CodeInstruction>'"
            : "'System.Collections.Generic.IEnumerable<Harmony.CodeInstruction>'";
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(13, 23, 13, 29)
                .WithArguments("Prepare", "'void', 'bool'"),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(15, 23, 15, 29)
                .WithArguments("Cleanup", "'void', 'System.Exception'"),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(17, 23, 17, 29)
                .WithArguments("TargetMethod", "'System.Reflection.MethodBase'"),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(19, 23, 19, 29)
                .WithArguments("Prefix", "'void', 'bool'"),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(23, 23, 23, 39)
                .WithArguments("Transpiler", enumerableOfCodeInstruction),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(29, 23, 29, 39)
                .WithArguments("TargetMethods", "'System.Collections.Generic.IEnumerable<System.Reflection.MethodBase>'"),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(33, 23, 33, 27)
                .WithArguments("Transpiler", enumerableOfCodeInstruction),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(41, 23, 41, 30)
                .WithArguments("Postfix1", "'void', 'string'"), // TODO: must mot be void
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(52, 23, 52, 34)
                .WithArguments("TargetMethod", "'System.Reflection.MethodBase'"),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(62, 23, 62, 47)
                .WithArguments("TargetMethods", "'System.Collections.Generic.IEnumerable<System.Reflection.MethodBase>'"),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(64, 23, 64, 52)
                .WithArguments("Transpiler", enumerableOfCodeInstruction),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(74, 23, 74, 47)
                .WithArguments("TargetMethods", "'System.Collections.Generic.IEnumerable<System.Reflection.MethodBase>'"),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(76, 23, 76, 52)
                .WithArguments("Transpiler", enumerableOfCodeInstruction),
        };
        if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(85, 23, 85, 29)
                    .WithArguments("Finalizer", "'void', 'System.Exception'"),
                new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(91, 23, 91, 31)
                    .WithArguments("ReversePatch", "'System.IO.FileSystemInfo'"),
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }
}