using HarmonyTools.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HarmonyTools;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HarmonyToolsCodeFixProvider)), Shared]
public class HarmonyToolsCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.HarmonyPatchAttributeMustBeOnType];

    public sealed override FixAllProvider GetFixAllProvider()
    {
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        return WellKnownFixAllProviders.BatchFixer;
    }

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
            context.RegisterCodeFix(CodeAction.Create("test", 
                cancellationToken => Task.FromResult(AddAttribute(context.Document, declaration, $"{harmonyNamespace}.HarmonyPatch", cancellationToken)), 
                "test"), diagnostic);
        }
    }

    private static Document AddAttribute(Document document, MemberDeclarationSyntax declaration, string attributeFullName, CancellationToken cancellationToken)
    {
        var attributeFullNameParts = attributeFullName.Split('.');
        NameSyntax identifier = SyntaxFactory.IdentifierName(attributeFullNameParts[0]);
        for (var i = 1; i < attributeFullNameParts.Length; i++)
            identifier = SyntaxFactory.QualifiedName(identifier, SyntaxFactory.IdentifierName(attributeFullNameParts[i]));
        var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(identifier)));
        var root = declaration.SyntaxTree.GetRoot(cancellationToken).ReplaceNode(declaration, declaration.AddAttributeLists(attributeList));
        return document.WithSyntaxRoot(root);
    }
}