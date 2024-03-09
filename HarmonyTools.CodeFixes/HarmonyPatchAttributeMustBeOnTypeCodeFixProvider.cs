using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HarmonyTools.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;

namespace HarmonyTools.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HarmonyPatchAttributeMustBeOnTypeCodeFixProvider)), Shared]
public class HarmonyPatchAttributeMustBeOnTypeCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.HarmonyPatchAttributeMustBeOnType];

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        if (root is null)
            return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var harmonyNamespace = diagnostic.Properties[HarmonyToolsAnalyzer.HarmonyNamespaceKey]!;
            var declaration = root.FindToken(diagnosticSpan.Start).Parent!.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();
            context.RegisterCodeFix(CodeAction.Create(CodeFixResources.HarmonyPatchAttributeMustBeOnTypeCodeFixTitle, 
                cancellationToken => 
                   AddAttribute(context.Document, root, declaration, $"{harmonyNamespace}.HarmonyPatch", cancellationToken),
                CodeFixResources.HarmonyPatchAttributeMustBeOnTypeCodeFixTitle), diagnostic);
        }
    }

    private static async Task<Document> AddAttribute(Document document, SyntaxNode root, MemberDeclarationSyntax declaration,
        string attributeFullName, CancellationToken cancellationToken)
    {
        var syntaxEditor = new SyntaxEditor(root, document.Project.Services.SolutionServices);
        var attribute = SyntaxFactory.Attribute(CreateNameFyntax(attributeFullName)).WithAdditionalAnnotations(Simplifier.Annotation);
        syntaxEditor.AddAttribute(declaration, attribute);
        return await Simplifier.ReduceAsync(document.WithSyntaxRoot(syntaxEditor.GetChangedRoot()), cancellationToken: cancellationToken);
    }

    private static NameSyntax CreateNameFyntax(string fullName)
    {
        var fullNameParts = fullName.Split('.');
        NameSyntax identifier = SyntaxFactory.IdentifierName(fullNameParts[0]);
        for (var i = 1; i < fullNameParts.Length; i++)
            identifier = SyntaxFactory.QualifiedName(identifier, SyntaxFactory.IdentifierName(fullNameParts[i]));
        return identifier;
    }
}