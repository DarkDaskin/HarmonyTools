using Microsoft.CodeAnalysis.CSharp;

namespace HarmonyTools.Analyzers;

internal class DetailWithSyntax<T>(T value, CSharpSyntaxNode? syntax)
{
    public T Value { get; } = value;
    public CSharpSyntaxNode? Syntax { get; } = syntax;

    public override string? ToString() => Value?.ToString();
}