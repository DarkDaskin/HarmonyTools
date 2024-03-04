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

    [TestMethod, CodeDataSource("ValidSimplePatches.cs")]
    public async Task WhenValidSimplePatches_DoNothing(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies);
    }

    [TestMethod, CodeDataSource("NonExistingMethods.cs")]
    public async Task WhenNonExistingMethods_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, 
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
                .WithArguments("get_Item", "HarmonyTools.Test.PatchBase.SimpleClass"));
    }

    [TestMethod, CodeDataSource("AmbiguousMatches.cs")]
    public async Task WhenAmbiguousMatches_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, 
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(6, 6, 6, 77)
                .WithArguments("OverloadedMethod", "HarmonyTools.Test.PatchBase.SimpleClass", "3"),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(12, 6, 12, 58)
                .WithArguments("get_Item", "HarmonyTools.Test.PatchBase.SimpleClass", "2"),
            new DiagnosticResult(DiagnosticIds.MethodMustNotBeAmbiguous, DiagnosticSeverity.Warning)
                .WithSpan(18, 6, 18, 72)
                .WithArguments(".ctor", "HarmonyTools.Test.PatchBase.MultipleConstructors", "2"));
    }
}