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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethodsCodeFixProvider)), Shared]
public class DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethodsCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => 
        [DiagnosticIds.DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethods];

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        if (root is null)
            return;

        foreach (var diagnostic in context.Diagnostics)
        {
            Location[] locations = [diagnostic.Location, .. diagnostic.AdditionalLocations];
            var attributes = locations.Select(location =>
                root.FindToken(location.SourceSpan.Start).Parent!.AncestorsAndSelf().OfType<AttributeSyntax>().First()).ToArray();
            context.RegisterCodeFix(CodeAction.Create(
                CodeFixResources.DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethodsCodeFixTitle, 
                _ => Task.FromResult(RemoveAttributes(context.Document, root, attributes)),
                CodeFixResources.DontUseTargetMethodAnnotationsOnNonPrimaryPatchMethodsCodeFixTitle), diagnostic);
        }
    }

    private static Document RemoveAttributes(Document document, SyntaxNode root, AttributeSyntax[] attributes)
    {
        var syntaxEditor = new SyntaxEditor(root, document.Project.Services.SolutionServices);
        foreach (var attribute in attributes)
            syntaxEditor.RemoveNode(attribute);
        return document.WithSyntaxRoot(syntaxEditor.GetChangedRoot());
    }
}