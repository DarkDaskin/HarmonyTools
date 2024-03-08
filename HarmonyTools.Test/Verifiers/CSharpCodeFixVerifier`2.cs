﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace HarmonyTools.Test.Verifiers
{
    public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic()"/>
        public static DiagnosticResult Diagnostic() => 
            CSharpCodeFixVerifier<TAnalyzer, TCodeFix, MSTestVerifier>.Diagnostic();

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(string)"/>
        public static DiagnosticResult Diagnostic(string diagnosticId) => 
            CSharpCodeFixVerifier<TAnalyzer, TCodeFix, MSTestVerifier>.Diagnostic(diagnosticId);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
        public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) => 
            CSharpCodeFixVerifier<TAnalyzer, TCodeFix, MSTestVerifier>.Diagnostic(descriptor);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
        public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
            => VerifyAnalyzerAsync(source, null, expected);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
        public static async Task VerifyAnalyzerAsync(string source, ReferenceAssemblies? referenceAssemblies = null, params DiagnosticResult[] expected)
        {
            var test = new Test
            {
                ReferenceAssemblies = referenceAssemblies ?? CSharpVerifierHelper.DefaultReferenceAssemblies,
                TestCode = source,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, string)"/>
        public static async Task VerifyCodeFixAsync(string source, string fixedSource) => 
            await VerifyCodeFixAsync(source, default(ReferenceAssemblies), fixedSource);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, string)"/>
        public static async Task VerifyCodeFixAsync(string source, ReferenceAssemblies? referenceAssemblies, string fixedSource) => 
            await VerifyCodeFixAsync(source, referenceAssemblies, DiagnosticResult.EmptyDiagnosticResults, fixedSource);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult, string)"/>
        public static async Task VerifyCodeFixAsync(string source, DiagnosticResult expected, string fixedSource) => 
            await VerifyCodeFixAsync(source, null, expected, fixedSource);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult, string)"/>
        public static async Task VerifyCodeFixAsync(string source, ReferenceAssemblies? referenceAssemblies, DiagnosticResult expected, string fixedSource) =>
            await VerifyCodeFixAsync(source, referenceAssemblies, [expected], fixedSource);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult[], string)"/>
        public static async Task VerifyCodeFixAsync(string source, DiagnosticResult[] expected, string fixedSource) =>
            await VerifyCodeFixAsync(source, null, expected, fixedSource);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult[], string)"/>
        public static async Task VerifyCodeFixAsync(string source, ReferenceAssemblies? referenceAssemblies, DiagnosticResult[] expected, string fixedSource)
        {
            var test = new Test
            {
                ReferenceAssemblies = referenceAssemblies ?? CSharpVerifierHelper.DefaultReferenceAssemblies,
                TestCode = source,
                FixedCode = fixedSource,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }
    }
}
