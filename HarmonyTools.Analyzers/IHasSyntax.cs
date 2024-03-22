using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace HarmonyTools.Analyzers;

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
        details.Select(detail => detail.Syntax?.GetIdentifierLocation() ?? detail.Syntax?.GetLocation()).FilterAndSort();

    public static Location? GetTypeLocation(this IEnumerable<IHasSyntax> details) =>
        details.GetTypeLocations().FirstOrDefault();

    public static IEnumerable<Location> GetAdditionalTypeLocations(this IEnumerable<IHasSyntax> details) =>
        details.GetTypeLocations().Skip(1);

    private static IEnumerable<Location> GetTypeLocations(this IEnumerable<IHasSyntax> details) =>
        details.Select(detail => detail.Syntax?.GetTypeLocation()).FilterAndSort();

    private static IEnumerable<Location> FilterAndSort(this IEnumerable<Location?> locations) =>
        locations.Where(location => location is not null).Cast<Location>().OrderBy(location => location.SourceSpan.Start);
}

internal class DetailWithSyntax<T>(T value, SyntaxNode? syntax) : IHasSyntax
{
    public T Value { get; } = value;
    public SyntaxNode? Syntax { get; } = syntax;

    public override string? ToString() => Value?.ToString();
}

internal static class DetailWithSyntaxExtensions
{
    public static bool Contains<T>(this IEnumerable<DetailWithSyntax<T>> details, T value) =>
        details.Any(detail => Equals(detail.Value, value));
}

internal class SyntaxWrapper(SyntaxNode? syntax) : IHasSyntax
{
    public SyntaxNode? Syntax { get; } = syntax;

    public SyntaxWrapper(AttributeData attribute) : this(attribute.GetSyntax()) { }
}
