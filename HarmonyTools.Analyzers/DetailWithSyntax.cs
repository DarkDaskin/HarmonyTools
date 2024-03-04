using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class DetailWithSyntax<T>(T value, SyntaxNode? syntax)
{
    public T Value { get; } = value;
    public SyntaxNode? Syntax { get; } = syntax;

    public override string? ToString() => Value?.ToString();
}