using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Testing;

namespace HarmonyTools.Test.Verifiers
{
    public static partial class VisualBasicCodeRefactoringVerifier<TCodeRefactoring>
        where TCodeRefactoring : CodeRefactoringProvider, new()
    {
        /// <inheritdoc cref="CodeRefactoringVerifier{TCodeRefactoring, TTest, TVerifier}.VerifyRefactoringAsync(string, string)"/>
        public static async Task VerifyRefactoringAsync(string code, string fixedCode)
        {
            await VerifyRefactoringAsync(code, DiagnosticResult.EmptyDiagnosticResults, fixedCode);
        }

        /// <inheritdoc cref="CodeRefactoringVerifier{TCodeRefactoring, TTest, TVerifier}.VerifyRefactoringAsync(string, DiagnosticResult, string)"/>
        public static async Task VerifyRefactoringAsync(string code, DiagnosticResult expected, string fixedCode)
        {
            await VerifyRefactoringAsync(code, new[] { expected }, fixedCode);
        }

        /// <inheritdoc cref="CodeRefactoringVerifier{TCodeRefactoring, TTest, TVerifier}.VerifyRefactoringAsync(string, DiagnosticResult[], string)"/>
        public static async Task VerifyRefactoringAsync(string code, DiagnosticResult[] expected, string fixedCode)
        {
            var test = new Test
            {
                TestCode = code,
                FixedCode = fixedCode,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }
    }
}
