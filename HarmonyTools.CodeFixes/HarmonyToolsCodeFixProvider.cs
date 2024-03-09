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
using Microsoft.CodeAnalysis.Simplification;

namespace HarmonyTools;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HarmonyToolsCodeFixProvider)), Shared]
public class HarmonyToolsCodeFixProvider : CodeFixProvider
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
       var attributeFullNameParts = attributeFullName.Split('.');
       NameSyntax identifier = SyntaxFactory.IdentifierName(attributeFullNameParts[0]);
       for (var i = 1; i < attributeFullNameParts.Length; i++)
          identifier = SyntaxFactory.QualifiedName(identifier, SyntaxFactory.IdentifierName(attributeFullNameParts[i]));
       var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(identifier)))
          .WithAdditionalAnnotations(Simplifier.Annotation);
       var newRoot = root.ReplaceNode(declaration, declaration.AddAttributeLists(attributeList));
       return await Simplifier.ReduceAsync(document.WithSyntaxRoot(newRoot), cancellationToken: cancellationToken);
    }
}