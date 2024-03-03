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

    [TestMethod, CodeDataSource("SimplePatch.cs")]
    public async Task SimplePatch(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies);
    }

    [TestMethod, CodeDataSource("NonExistingMethod.cs")]
    public async Task NonExistingMethod(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, 
            new DiagnosticResult(DiagnosticIds.MethodMustExist, DiagnosticSeverity.Warning)
                .WithSpan(6, 40, 6, 59)
                .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass", "NonExistingMethod"));
    }
}