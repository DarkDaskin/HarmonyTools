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
        var transpilerMessage = version == 2
            ? "Incorrect patch method 'Transpiler' return type. Valid types are: 'System.Collections.Generic.IEnumerable<HarmonyLib.CodeInstruction>' and their subtypes."
            : "Incorrect patch method 'Transpiler' return type. Valid types are: 'System.Collections.Generic.IEnumerable<Harmony.CodeInstruction>' and their subtypes.";
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(13, 23, 13, 29)
                .WithMessage("Incorrect patch method 'Prepare' return type. Valid types are: 'void', 'bool' and their subtypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(15, 23, 15, 29)
                .WithMessage("Incorrect patch method 'Cleanup' return type. Valid types are: 'void', 'System.Exception' and their subtypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(17, 23, 17, 29)
                .WithMessage("Incorrect patch method 'TargetMethod' return type. Valid types are: 'System.Reflection.MethodBase' and their subtypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(19, 23, 19, 29)
                .WithMessage("Incorrect patch method 'Prefix' return type. Valid types are: 'void', 'bool' and their subtypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(23, 23, 23, 39)
                .WithMessage(transpilerMessage),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(29, 23, 29, 39)
                .WithMessage("Incorrect patch method 'TargetMethods' return type. Valid types are: 'System.Collections.Generic.IEnumerable<System.Reflection.MethodBase>' and their subtypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(33, 23, 33, 27)
                .WithMessage(transpilerMessage),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(41, 23, 41, 30)
                .WithMessage("Incorrect patch method 'Postfix1' return type. Valid types are: 'string' and their subtypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(52, 23, 52, 34)
                .WithMessage("Incorrect patch method 'TargetMethod' return type. Valid types are: 'System.Reflection.MethodBase' and their subtypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(62, 23, 62, 47)
                .WithMessage("Incorrect patch method 'TargetMethods' return type. Valid types are: 'System.Collections.Generic.IEnumerable<System.Reflection.MethodBase>' and their subtypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(64, 23, 64, 52)
                .WithMessage(transpilerMessage),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(74, 23, 74, 47)
                .WithMessage("Incorrect patch method 'TargetMethods' return type. Valid types are: 'System.Collections.Generic.IEnumerable<System.Reflection.MethodBase>' and their subtypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(76, 23, 76, 52)
                .WithMessage(transpilerMessage),
        };
        if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(85, 23, 85, 29)
                    .WithMessage("Incorrect patch method 'Finalizer' return type. Valid types are: 'void', 'System.Exception' and their subtypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(91, 23, 91, 31)
                    .WithMessage("Incorrect patch method 'ReversePatch' return type. Valid types are: 'System.IO.FileSystemInfo' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(95, 23, 95, 29)
                    .WithMessage("Incorrect patch method 'Invoke' return type. Valid types are: 'int'."),
                new DiagnosticResult(DiagnosticIds.PatchMethodReturnTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(99, 23, 99, 29)
                    .WithMessage("Incorrect patch method 'Invoke' return type. Valid types are: 'string?' and their supertypes."),
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("ValidPatchMethodParameters.cs")]
    public async Task WhenValidPatchMethodParameters_DoNothing(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies);
    }

    [TestMethod, CodeDataSource("UnrecognizedPatchMethodParameters.cs", ProvideVersion = true)]
    public async Task WhenUnrecognizedPatchMethodParameters_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.PatchMethodParametersMustBeValidInjections, DiagnosticSeverity.Warning)
                .WithSpan(13, 43, 13, 50)
                .WithArguments("Prepare", "harmony"),
            new DiagnosticResult(DiagnosticIds.PatchMethodParametersMustBeValidInjections, DiagnosticSeverity.Warning)
                .WithSpan(13, 62, 13, 71)
                .WithArguments("Prepare", "exception"),
            new DiagnosticResult(DiagnosticIds.PatchMethodParametersMustBeValidInjections, DiagnosticSeverity.Warning)
                .WithSpan(15, 43, 15, 50)
                .WithArguments("Cleanup", "harmony"),
            new DiagnosticResult(DiagnosticIds.PatchMethodParametersMustBeValidInjections, DiagnosticSeverity.Warning)
                .WithSpan(17, 54, 17, 61)
                .WithArguments("TargetMethod", "harmony"),
            new DiagnosticResult(DiagnosticIds.PatchMethodParametersMustBeValidInjections, DiagnosticSeverity.Warning)
                .WithSpan(17, 74, 17, 82)
                .WithArguments("TargetMethod", "original"),
            new DiagnosticResult(DiagnosticIds.PatchMethodParametersMustBeValidInjections, DiagnosticSeverity.Warning)
                .WithSpan(17, 94, 17, 103)
                .WithArguments("TargetMethod", "exception"),
            new DiagnosticResult(DiagnosticIds.PatchMethodParametersMustBeValidInjections, DiagnosticSeverity.Warning)
                .WithSpan(25, 68, 25, 75)
                .WithArguments("TargetMethods", "harmony"),
            new DiagnosticResult(DiagnosticIds.PatchMethodParametersMustBeValidInjections, DiagnosticSeverity.Warning)
                .WithSpan(25, 88, 25, 96)
                .WithArguments("TargetMethods", "original"),
            new DiagnosticResult(DiagnosticIds.PatchMethodParametersMustBeValidInjections, DiagnosticSeverity.Warning)
                .WithSpan(25, 108, 25, 117)
                .WithArguments("TargetMethods", "exception"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(35, 42, 35, 45)
                .WithArguments("SimpleMethod", "foo", "Prefix", "foo"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(35, 54, 35, 57)
                .WithArguments("SimpleMethod", "1", "Prefix", "__1"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(35, 91, 35, 95)
                .WithArguments("SimpleMethod", "foo", "Prefix", "foo1"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(36, 41, 36, 45)
                .WithArguments("SimpleMethod", "1", "Prefix", "foo2"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(36, 54, 36, 58)
                .WithArguments("SimpleMethod", "foo", "Prefix", "foo3"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(36, 67, 36, 71)
                .WithArguments("SimpleMethod", "1", "Prefix", "foo4"),
            version == 2
                ? new DiagnosticResult(DiagnosticIds.PatchMethodDelegateParametersMustBeAnnotatedWithHarmonyDelegate, DiagnosticSeverity.Warning)
                    .WithSpan(36, 80, 36, 86)
                    .WithArguments("Prefix", "action", "System.Action")
                : new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(36, 80, 36, 86)
                    .WithArguments("SimpleMethod", "action", "Prefix", "action"),
            new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(36, 95, 36, 101)
                .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass", "foo", "Prefix", "___foo"),
            new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(36, 110, 36, 115)
                .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass", "10", "Prefix", "___10"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(36, 127, 36, 138)
                .WithArguments("SimpleMethod", "__exception", "Prefix", "__exception"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(40, 44, 40, 47)
                .WithArguments("SimpleMethod", "foo", "Postfix1", "foo"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(40, 56, 40, 59)
                .WithArguments("SimpleMethod", "1", "Postfix1", "__1"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(40, 93, 40, 97)
                .WithArguments("SimpleMethod", "foo", "Postfix1", "foo1"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(41, 41, 41, 45)
                .WithArguments("SimpleMethod", "1", "Postfix1", "foo2"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(41, 54, 41, 58)
                .WithArguments("SimpleMethod", "foo", "Postfix1", "foo3"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(41, 67, 41, 71)
                .WithArguments("SimpleMethod", "1", "Postfix1", "foo4"),
            version == 2
                ? new DiagnosticResult(DiagnosticIds.PatchMethodDelegateParametersMustBeAnnotatedWithHarmonyDelegate, DiagnosticSeverity.Warning)
                    .WithSpan(41, 80, 41, 86)
                    .WithArguments("Postfix1", "action", "System.Action")
                : new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(41, 80, 41, 86)
                    .WithArguments("SimpleMethod", "action", "Postfix1", "action"),
            new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(41, 95, 41, 101)
                .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass", "foo", "Postfix1", "___foo"),
            new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(41, 110, 41, 115)
                .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass", "10", "Postfix1", "___10"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(41, 127, 41, 138)
                .WithArguments("SimpleMethod", "__exception", "Postfix1", "__exception"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(44, 41, 44, 47)
                .WithArguments("SimpleMethod", "result", "Postfix2", "result"),
            new DiagnosticResult(DiagnosticIds.PatchMethodParametersMustBeValidInjections, DiagnosticSeverity.Warning)
                .WithSpan(46, 113, 46, 122)
                .WithArguments("Transpiler", "generator"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(52, 42, 52, 50)
                .WithArguments(".cctor", "question", "Prefix", "question"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(52, 42, 52, 50)
                .WithArguments(".ctor", "question", "Prefix", "question"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(52, 42, 52, 50)
                .WithArguments("SimpleMethod", "question", "Prefix", "question"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(52, 59, 52, 62)
                .WithArguments(".cctor", "0", "Prefix", "__0"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(52, 59, 52, 62)
                .WithArguments(".ctor", "0", "Prefix", "__0"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(52, 59, 52, 62)
                .WithArguments("SimpleMethod", "0", "Prefix", "__0"),
            new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(52, 71, 52, 77)
                .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass2", "foo", "Prefix", "___foo"),
            new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(52, 86, 52, 91)
                .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass2", "10", "Prefix", "___10"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(54, 43, 54, 51)
                .WithArguments(".cctor", "question", "Postfix", "question"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(54, 43, 54, 51)
                .WithArguments(".ctor", "question", "Postfix", "question"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(54, 43, 54, 51)
                .WithArguments("SimpleMethod", "question", "Postfix", "question"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(54, 60, 54, 63)
                .WithArguments(".cctor", "0", "Postfix", "__0"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(54, 60, 54, 63)
                .WithArguments(".ctor", "0", "Postfix", "__0"),
            new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(54, 60, 54, 63)
                .WithArguments("SimpleMethod", "0", "Postfix", "__0"),
            new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                .WithSpan(54, 72, 54, 78)
                .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass2", "foo", "Postfix", "___foo"),
            new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                .WithSpan(54, 87, 54, 92)
                .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass2", "10", "Postfix", "___10"),
            new DiagnosticResult(DiagnosticIds.DoNotUseInstanceParameterWithStaticMethods, DiagnosticSeverity.Warning)
                .WithSpan(60, 48, 60, 58)
                .WithArguments("SimpleStaticMethod"),
            new DiagnosticResult(DiagnosticIds.DoNotUseResultWithMethodsReturningVoid, DiagnosticSeverity.Warning)
                .WithSpan(66, 43, 66, 51)
                .WithArguments("DoNothing"),
        };
        if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.DoNotUseResultRefWithMethodsNotReturningByRef, DiagnosticSeverity.Warning)
                    .WithSpan(72, 54, 72, 65)
                    .WithArguments("SimpleMethod"),
                new DiagnosticResult(DiagnosticIds.DoNotUseResultRefWithMethodsNotReturningByRef, DiagnosticSeverity.Warning)
                    .WithSpan(74, 55, 74, 66)
                    .WithArguments("SimpleMethod"),
                new DiagnosticResult(DiagnosticIds.DoNotUseResultRefWithMethodsNotReturningByRef, DiagnosticSeverity.Warning)
                    .WithSpan(76, 57, 76, 68)
                    .WithArguments("SimpleMethod"),
                new DiagnosticResult(DiagnosticIds.DoNotUseResultWithMethodsReturningByRef, DiagnosticSeverity.Warning)
                    .WithSpan(82, 39, 82, 47)
                    .WithArguments("GetAnswerRef"),
                new DiagnosticResult(DiagnosticIds.DoNotUseResultWithMethodsReturningByRef, DiagnosticSeverity.Warning)
                    .WithSpan(84, 40, 84, 48)
                    .WithArguments("GetAnswerRef"),
                new DiagnosticResult(DiagnosticIds.DoNotUseResultWithMethodsReturningByRef, DiagnosticSeverity.Warning)
                    .WithSpan(86, 42, 86, 50)
                    .WithArguments("GetAnswerRef"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(94, 45, 94, 48)
                    .WithArguments("SimpleMethod", "foo", "Finalizer", "foo"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(94, 57, 94, 60)
                    .WithArguments("SimpleMethod", "1", "Finalizer", "__1"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(94, 94, 94, 98)
                    .WithArguments("SimpleMethod", "foo", "Finalizer", "foo1"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(95, 41, 95, 45)
                    .WithArguments("SimpleMethod", "1", "Finalizer", "foo2"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(95, 54, 95, 58)
                    .WithArguments("SimpleMethod", "foo", "Finalizer", "foo3"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(95, 67, 95, 71)
                    .WithArguments("SimpleMethod", "1", "Finalizer", "foo4"),
                new DiagnosticResult(DiagnosticIds.PatchMethodDelegateParametersMustBeAnnotatedWithHarmonyDelegate, DiagnosticSeverity.Warning)
                    .WithSpan(95, 80, 95, 86)
                    .WithArguments("Finalizer", "action", "System.Action"),
                new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(95, 95, 95, 101)
                    .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass", "foo", "Finalizer", "___foo"),
                new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(95, 110, 95, 115)
                    .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass", "10", "Finalizer", "___10"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(101, 45, 101, 53)
                    .WithArguments(".cctor", "question", "Finalizer", "question"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(101, 45, 101, 53)
                    .WithArguments(".ctor", "question", "Finalizer", "question"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(101, 45, 101, 53)
                    .WithArguments("SimpleMethod", "question", "Finalizer", "question"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(101, 62, 101, 65)
                    .WithArguments(".cctor", "0", "Finalizer", "__0"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(101, 62, 101, 65)
                    .WithArguments(".ctor", "0", "Finalizer", "__0"),
                new DiagnosticResult(DiagnosticIds.TargetMethodParameterWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(101, 62, 101, 65)
                    .WithArguments("SimpleMethod", "0", "Finalizer", "__0"),
                new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedNameMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(101, 74, 101, 80)
                    .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass2", "foo", "Finalizer", "___foo"),
                new DiagnosticResult(DiagnosticIds.TargetTypeFieldWithSpecifiedIndexMustExist, DiagnosticSeverity.Warning)
                    .WithSpan(101, 89, 101, 93)
                    .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass2", "2", "Finalizer", "___2"),
                new DiagnosticResult(DiagnosticIds.ReversePatchMethodParameterMustCorrespondToTargetMethodParameter, DiagnosticSeverity.Warning)
                    .WithSpan(108, 87, 108, 90)
                    .WithArguments("ReversePatch1", "foo", "SimpleMethod"),
                new DiagnosticResult(DiagnosticIds.InstanceParameterMustNotBePresent, DiagnosticSeverity.Warning)
                    .WithSpan(111, 53, 111, 61)
                    .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass", "ReversePatch2"),
                new DiagnosticResult(DiagnosticIds.ReversePatchMethodParameterMustCorrespondToTargetMethodParameter, DiagnosticSeverity.Warning)
                    .WithSpan(111, 87, 111, 90)
                    .WithArguments("ReversePatch2", "foo", "SimpleStaticMethod"),
                new DiagnosticResult(DiagnosticIds.ReversePatchMethodParameterMustCorrespondToTargetMethodParameter, DiagnosticSeverity.Warning)
                    .WithSpan(115, 82, 115, 85)
                    .WithArguments("Invoke", "foo", "SimpleMethod"),
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("InvalidPatchMethodParameterTypes.cs", ProvideVersion = true)]
    public async Task WhenInvalidPatchMethodParameterTypes_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(15, 35, 15, 38)
                .WithMessage("Incorrect patch method 'Prefix' parameter 'question' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(15, 49, 15, 52)
                .WithMessage("Incorrect patch method 'Prefix' parameter '__0' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(15, 88, 15, 91)
                .WithMessage("Incorrect patch method 'Prefix' parameter 'question1' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(16, 34, 16, 37)
                .WithMessage("Incorrect patch method 'Prefix' parameter 'question2' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(16, 49, 16, 52)
                .WithMessage("Incorrect patch method 'Prefix' parameter 'question3' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(16, 64, 16, 67)
                .WithMessage("Incorrect patch method 'Prefix' parameter 'question4' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(16, 79, 16, 93)
                .WithMessage("Incorrect patch method 'Prefix' parameter '__instance' type. Valid types are: 'HarmonyTools.Test.PatchBase.SimpleClass' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(16, 106, 16, 115)
                .WithMessage("Incorrect patch method 'Prefix' parameter '__result' type. Valid types are: 'int', 'object'."),
            new DiagnosticResult(DiagnosticIds.StateTypeMustNotDiffer, DiagnosticSeverity.Warning)
                .WithSpan(17, 17, 17, 26)
                .WithSpan(25, 13, 25, 19),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(17, 36, 17, 42)
                .WithMessage("Incorrect patch method 'Prefix' parameter '____answer' type. Valid types are: 'int', 'object'."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(17, 55, 17, 67)
                .WithMessage("Incorrect patch method 'Prefix' parameter '___0' type. Valid types are: 'int', 'object'."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(17, 74, 17, 82)
                .WithMessage("Incorrect patch method 'Prefix' parameter '__args' type. Valid types are: 'object[]' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(17, 91, 17, 101)
                .WithMessage("Incorrect patch method 'Prefix' parameter '__originalMethod' type. Valid types are: 'System.Reflection.MethodBase' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(17, 120, 17, 126)
                .WithMessage("Incorrect patch method 'Prefix' parameter '__runOriginal' type. Valid types are: 'bool'."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(23, 36, 23, 39)
                .WithMessage("Incorrect patch method 'Postfix' parameter 'question' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(23, 50, 23, 53)
                .WithMessage("Incorrect patch method 'Postfix' parameter '__0' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(23, 89, 23, 92)
                .WithMessage("Incorrect patch method 'Postfix' parameter 'question1' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(24, 34, 24, 37)
                .WithMessage("Incorrect patch method 'Postfix' parameter 'question2' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(24, 49, 24, 52)
                .WithMessage("Incorrect patch method 'Postfix' parameter 'question3' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(24, 64, 24, 67)
                .WithMessage("Incorrect patch method 'Postfix' parameter 'question4' type. Valid types are: 'string' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(24, 79, 24, 93)
                .WithMessage("Incorrect patch method 'Postfix' parameter '__instance' type. Valid types are: 'HarmonyTools.Test.PatchBase.SimpleClass' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(24, 106, 24, 115)
                .WithMessage("Incorrect patch method 'Postfix' parameter '__result' type. Valid types are: 'int', 'object'."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(25, 29, 25, 35)
                .WithMessage("Incorrect patch method 'Postfix' parameter '____answer' type. Valid types are: 'int', 'object'."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(25, 48, 25, 60)
                .WithMessage("Incorrect patch method 'Postfix' parameter '___0' type. Valid types are: 'int', 'object'."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(25, 67, 25, 75)
                .WithMessage("Incorrect patch method 'Postfix' parameter '__args' type. Valid types are: 'object[]' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(25, 84, 25, 94)
                .WithMessage("Incorrect patch method 'Postfix' parameter '__originalMethod' type. Valid types are: 'System.Reflection.MethodBase' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(25, 113, 25, 119)
                .WithMessage("Incorrect patch method 'Postfix' parameter '__runOriginal' type. Valid types are: 'bool'."),
            new DiagnosticResult(DiagnosticIds.StateShouldBeInitialized, DiagnosticSeverity.Warning)
                .WithSpan(31, 45, 31, 52)
                .WithSpan(33, 46, 33, 53),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(42, 41, 42, 48)
                .WithMessage("Incorrect patch method 'Postfix1' parameter '__result' type. Valid types are: 'string'."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(45, 37, 45, 43)
                .WithMessage("Incorrect patch method 'Postfix2' parameter 's' type. Valid types are: 'string?' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(45, 47, 45, 53)
                .WithMessage("Incorrect patch method 'Postfix2' parameter '__result' type. Valid types are: 'string?' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(45, 64, 45, 72)
                .WithMessage("Incorrect patch method 'Postfix2' parameter '__args' type. Valid types are: 'object?[]' and their supertypes."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(48, 41, 48, 47)
                .WithMessage("Incorrect patch method 'Postfix3' parameter '__result' type. Valid types are: 'string?'."),
            new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                .WithSpan(56, 36, 56, 48)
                .WithMessage("Incorrect patch method 'Postfix' parameter '__instance' type. Valid types are: 'HarmonyTools.Test.PatchBase.SimpleClass2?' and their supertypes."),
        };
        if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(65, 38, 65, 41)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter 'question' type. Valid types are: 'string' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(65, 52, 65, 55)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter '__0' type. Valid types are: 'string' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(65, 91, 65, 94)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter 'question1' type. Valid types are: 'string' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(66, 34, 66, 37)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter 'question2' type. Valid types are: 'string' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(66, 49, 66, 52)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter 'question3' type. Valid types are: 'string' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(66, 64, 66, 67)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter 'question4' type. Valid types are: 'string' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(66, 79, 66, 93)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter '__instance' type. Valid types are: 'HarmonyTools.Test.PatchBase.SimpleClass' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(66, 106, 66, 115)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter '__result' type. Valid types are: 'int', 'object'."),
                new DiagnosticResult(DiagnosticIds.StateShouldBeInitialized, DiagnosticSeverity.Warning)
                    .WithSpan(67, 20, 67, 27),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(67, 29, 67, 35)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter '____answer' type. Valid types are: 'int', 'object'."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(67, 48, 67, 60)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter '___0' type. Valid types are: 'int', 'object'."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(67, 67, 67, 75)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter '__args' type. Valid types are: 'object[]' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(67, 84, 67, 94)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter '__originalMethod' type. Valid types are: 'System.Reflection.MethodBase' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(67, 113, 67, 119)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter '__runOriginal' type. Valid types are: 'bool'."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(68, 13, 68, 30)
                    .WithMessage("Incorrect patch method 'Finalizer' parameter '__exception' type. Valid types are: 'System.Exception' and their supertypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(75, 40, 75, 52)
                    .WithMessage("Incorrect patch method 'ReversePatch' parameter 'instance' type. Valid types are: 'HarmonyTools.Test.PatchBase.SimpleClass' and their subtypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(75, 63, 75, 69)
                    .WithMessage("Incorrect patch method 'ReversePatch' parameter 'question' type. Valid types are: 'string' and their subtypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(83, 43, 83, 50)
                    .WithMessage("Incorrect patch method 'ReversePatch' parameter 's' type. Valid types are: 'string' and their subtypes."),
                new DiagnosticResult(DiagnosticIds.PatchMethodParameterTypesMustBeCorrect, DiagnosticSeverity.Warning)
                    .WithSpan(89, 68, 89, 75)
                    .WithMessage("Incorrect patch method 'Invoke' parameter 's' type. Valid types are: 'string' and their subtypes."),
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("MissingPatchMethodParameters.cs")]
    public async Task WhenMissingPatchMethodParameters_Report(string code, ReferenceAssemblies referenceAssemblies)
    {
        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies,
            new DiagnosticResult(DiagnosticIds.InstanceParameterMustBePresent, DiagnosticSeverity.Warning)
                .WithSpan(10, 27, 10, 40)
                .WithArguments("HarmonyTools.Test.PatchBase.SimpleClass", "ReversePatch1"),
            new DiagnosticResult(DiagnosticIds.AllTargetMethodParametersMustBeIncluded, DiagnosticSeverity.Warning)
                .WithSpan(13, 27, 13, 40)
                .WithArguments("SimpleStaticMethod", "question", "ReversePatch2"),
            new DiagnosticResult(DiagnosticIds.AllTargetMethodParametersMustBeIncluded, DiagnosticSeverity.Warning)
                .WithSpan(17, 27, 17, 52)
                .WithArguments("SimpleMethod", "question", "Invoke"));
    }

    [TestMethod, CodeDataSource("InvalidPatchMethodByRefParameters.cs", ProvideVersion = true)]
    public async Task WhenInvalidPatchMethodByRefParameters_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(14, 36, 14, 39)
                .WithArguments("Prepare", "original"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(14, 61, 14, 64)
                .WithArguments("Prepare", "harmony"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(16, 36, 16, 39)
                .WithArguments("Cleanup", "original"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(16, 61, 16, 64)
                .WithArguments("Cleanup", "exception"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(16, 86, 16, 89)
                .WithArguments("Cleanup", "harmony"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(18, 47, 18, 50)
                .WithArguments("TargetMethod", "harmony"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(20, 63, 20, 66)
                .WithArguments("Transpiler", "instructions"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(21, 13, 21, 16)
                .WithArguments("Transpiler", "generator"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(21, 40, 21, 43)
                .WithArguments("Transpiler", "original"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(23, 35, 23, 38)
                .WithArguments("Prefix", "__instance"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(23, 58, 23, 61)
                .WithArguments("Prefix", "__args"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(23, 79, 23, 82)
                .WithArguments("Prefix", "__originalMethod"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(23, 112, 23, 115)
                .WithArguments("Prefix", "__runOriginal"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(25, 36, 25, 39)
                .WithArguments("Postfix", "__instance"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(25, 59, 25, 62)
                .WithArguments("Postfix", "__args"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(25, 80, 25, 83)
                .WithArguments("Postfix", "__originalMethod"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(25, 113, 25, 116)
                .WithArguments("Postfix", "__runOriginal"),
            new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                .WithSpan(31, 61, 31, 64)
                .WithArguments("TargetMethods", "harmony"),

        };
        if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(39, 35, 39, 38)
                    .WithArguments("Prefix", "delegate"),
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(41, 36, 41, 39)
                    .WithArguments("Postfix", "delegate"),
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(43, 38, 43, 41)
                    .WithArguments("Finalizer", "__instance"),
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(43, 66, 43, 69)
                    .WithArguments("Finalizer", "__args"),
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(43, 87, 43, 90)
                    .WithArguments("Finalizer", "__originalMethod"),
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(43, 120, 43, 123)
                    .WithArguments("Finalizer", "__runOriginal"),
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(44, 13, 44, 16)
                    .WithArguments("Finalizer", "__exception"),
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(44, 40, 44, 43)
                    .WithArguments("Finalizer", "delegate"),
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(47, 41, 47, 44)
                    .WithArguments("ReversePatch1", "instance"),
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(47, 78, 47, 86)
                    .WithArguments("ReversePatch1", "question"),
                new DiagnosticResult(DiagnosticIds.ReversePatchMethodParameterMustHaveCorrectRefKind, DiagnosticSeverity.Warning)
                    .WithSpan(51, 68, 51, 74)
                    .WithArguments("ReversePatch2", "answer", "OverloadedMethod", "out"),
                new DiagnosticResult(DiagnosticIds.ParameterMustNotBeByRef, DiagnosticSeverity.Warning)
                    .WithSpan(54, 59, 54, 67)
                    .WithArguments("Invoke", "question"),
                new DiagnosticResult(DiagnosticIds.ReversePatchMethodParameterMustHaveCorrectRefKind, DiagnosticSeverity.Warning)
                    .WithSpan(58, 51, 58, 57)
                    .WithArguments("Invoke", "answer", "OverloadedMethod", "out"),
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }

    [TestMethod, CodeDataSource("InvalidPatchMethodReturnByRefTypes.cs", ProvideVersion = true)]
    public async Task WhenInvalidPatchMethodReturnByRefTypes_Report(string code, ReferenceAssemblies referenceAssemblies, int version)
    {
        var expected = new List<DiagnosticResult>
        {
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotReturnByRef, DiagnosticSeverity.Warning)
                .WithSpan(17, 23, 17, 26)
                .WithArguments("Prepare"),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotReturnByRef, DiagnosticSeverity.Warning)
                .WithSpan(19, 23, 19, 26)
                .WithArguments("Cleanup"),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotReturnByRef, DiagnosticSeverity.Warning)
                .WithSpan(21, 23, 21, 26)
                .WithArguments("TargetMethod"),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotReturnByRef, DiagnosticSeverity.Warning)
                .WithSpan(23, 23, 23, 26)
                .WithArguments("Transpiler"),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotReturnByRef, DiagnosticSeverity.Warning)
                .WithSpan(31, 23, 31, 26)
                .WithArguments("TargetMethods"),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotReturnByRef, DiagnosticSeverity.Warning)
                .WithSpan(42, 23, 42, 26)
                .WithArguments("Prefix"),
            new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotReturnByRef, DiagnosticSeverity.Warning)
                .WithSpan(44, 23, 44, 26)
                .WithArguments("Postfix"),
        };
        if (version == 2)
            expected.AddRange(
            [
                new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotReturnByRef, DiagnosticSeverity.Warning)
                    .WithSpan(53, 23, 53, 26)
                    .WithArguments("Finalizer"),
                new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotReturnByRef, DiagnosticSeverity.Warning)
                    .WithSpan(55, 23, 55, 26)
                    .WithArguments("ReversePatch"), 
                new DiagnosticResult(DiagnosticIds.PatchMethodsMustNotReturnByRef, DiagnosticSeverity.Warning)
                    .WithSpan(58, 25, 58, 28)
                    .WithArguments("Invoke"),             
            ]);

        await VerifyCS.VerifyAnalyzerAsync(code, referenceAssemblies, expected.ToArray());
    }
}