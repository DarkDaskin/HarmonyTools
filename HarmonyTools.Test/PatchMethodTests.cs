using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyTools.Analyzers;
using HarmonyTools.Test.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = HarmonyTools.Test.Verifiers.CSharpCodeFixVerifier<
    HarmonyTools.Analyzers.HarmonyToolsAnalyzer,
    HarmonyTools.HarmonyToolsCodeFixProvider>;

namespace HarmonyTools.Test;

[TestClass]
public class PatchMethodTests
{
    [TestMethod, CodeDataSource("MultiplePatchMethodKinds.cs")]
    public async Task WhenMultiplePatchMethodKinds_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.PatchMethodMustHaveSingleKind, DiagnosticSeverity.Warning)
                .WithSpan(9, 10, 9, 24)
                .WithSpan(10, 21, 10, 27),
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
                .WithSpan(9, 21, 9, 28)
                .WithSpan(12, 21, 12, 29));
    }
}