using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HarmonyTools.Analyzers;

internal static class AttributeDataExtensions
{
    public static bool Is(this AttributeData attribute, ITypeSymbol? type) => attribute.AttributeClass.Is(type);

    public static bool IsMatch(this AttributeData attribute, params ITypeSymbol[] argumentTypes) =>
        attribute.AttributeConstructor.IsMatch(argumentTypes);

    public static SyntaxNode? GetSyntax(this AttributeData attribute, CancellationToken cancellationToken = default) => 
        attribute.ApplicationSyntaxReference?.GetSyntax(cancellationToken);

    public static DetailWithSyntax<T> GetDetailWithSyntax<T>(this AttributeData attribute, int constructorParameterIndex)
    {
        var value = (T)attribute.ConstructorArguments[constructorParameterIndex].Value!;
        var attributeSyntax = (AttributeSyntax?)attribute.ApplicationSyntaxReference?.GetSyntax();
        var argumentSyntax = attributeSyntax?.ArgumentList?.Arguments[constructorParameterIndex];
        return new DetailWithSyntax<T>(value, argumentSyntax);
    }

    public static DetailWithSyntax<ImmutableArray<T>> GetDetailWithSyntaxForArray<T>(this AttributeData attribute, int constructorParameterIndex)
    {
        var values = attribute.ConstructorArguments[constructorParameterIndex].Values;
        var value = values.IsDefault ? default : values.Select(constant => (T)constant.Value!).ToImmutableArray();
        var attributeSyntax = (AttributeSyntax?)attribute.ApplicationSyntaxReference?.GetSyntax();
        var argumentSyntax = attributeSyntax?.ArgumentList?.Arguments[constructorParameterIndex];
        return new DetailWithSyntax<ImmutableArray<T>>(value, (SyntaxNode?)argumentSyntax ?? attributeSyntax);
    }
}