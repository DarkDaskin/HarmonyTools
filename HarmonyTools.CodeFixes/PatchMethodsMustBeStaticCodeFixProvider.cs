using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using HarmonyTools.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace HarmonyTools.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PatchMethodsMustBeStaticCodeFixProvider)), Shared]
public class PatchMethodsMustBeStaticCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PatchMethodsMustBeStatic];

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        if (root is null)
            return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var declaration = root.FindToken(diagnosticSpan.Start).Parent!.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
            context.RegisterCodeFix(CodeAction.Create(CodeFixResources.PatchMethodsMustBeStaticCodeFixTitle, 
                _ => Task.FromResult(MakeStatic(context.Document, root, declaration)),
                CodeFixResources.PatchMethodsMustBeStaticCodeFixTitle), diagnostic);
        }
    }

    private static Document MakeStatic(Document document, SyntaxNode root, MemberDeclarationSyntax declaration)
    {
        var syntaxEditor = new SyntaxEditor(root, document.Project.Services.SolutionServices);
        var modifiers = syntaxEditor.Generator.GetModifiers(declaration);
        syntaxEditor.SetModifiers(declaration, modifiers.WithIsStatic(true));
        return document.WithSyntaxRoot(syntaxEditor.GetChangedRoot());
    }
}