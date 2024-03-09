using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class DetailWithSyntax<T>(T value, SyntaxNode? syntax) : IHasSyntax
{
    public T Value { get; } = value;
    public SyntaxNode? Syntax { get; } = syntax;

    public override string? ToString() => Value?.ToString();
}

internal interface IHasSyntax
{
    SyntaxNode? Syntax { get; }
}

internal static class HasSyntaxExtensions
{
    public static Location? GetLocation(this IEnumerable<IHasSyntax> details) =>
        details.GetLocations().FirstOrDefault();

    public static IEnumerable<Location> GetAdditionalLocations(this IEnumerable<IHasSyntax> details) =>
        details.GetLocations().Skip(1);

    private static IEnumerable<Location> GetLocations(this IEnumerable<IHasSyntax> details) =>
        details.Select(detail => detail.Syntax?.GetIdentifierLocation() ?? detail.Syntax?.GetLocation())
            .Where(location => location is not null).Cast<Location>().OrderBy(location => location.SourceSpan.Start);
}