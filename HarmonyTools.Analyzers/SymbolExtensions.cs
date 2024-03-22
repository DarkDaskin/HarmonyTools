using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace HarmonyTools.Analyzers;

internal static class SymbolExtensions
{
    public static SyntaxNode? GetSyntax(this ISymbol symbol, SyntaxNode? reference = null, CancellationToken cancellationToken = default) =>
        symbol.DeclaringSyntaxReferences
            .FirstOrDefault(syntaxReference => reference is null || syntaxReference.SyntaxTree == reference.SyntaxTree)?
            .GetSyntax(cancellationToken);

    public static Location? GetIdentifierLocation(this SyntaxNode? syntax)
    {
        return syntax switch
        {
            BaseTypeDeclarationSyntax typeDeclarationSyntax => typeDeclarationSyntax.Identifier.GetLocation(),
            MethodDeclarationSyntax methodDeclarationSyntax => methodDeclarationSyntax.Identifier.GetLocation(),
            DelegateDeclarationSyntax delegateDeclarationSyntax => delegateDeclarationSyntax.Identifier.GetLocation(),
            PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax.Identifier.GetLocation(),
            ParameterSyntax parameterSyntax => parameterSyntax.Identifier.GetLocation(),
            _ => null
        };
    }

    public static Location? GetTypeLocation(this SyntaxNode? syntax)
    {
        return syntax switch
        {
            MethodDeclarationSyntax methodDeclarationSyntax => methodDeclarationSyntax.ReturnType.GetLocation(),
            DelegateDeclarationSyntax delegateDeclarationSyntax => delegateDeclarationSyntax.ReturnType.GetLocation(),
            PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax.Type.GetLocation(),
            ParameterSyntax parameterSyntax => parameterSyntax.Type?.GetLocation(),
            _ => null
        };
    }

    /// <summary>
    /// Determines whether <paramref name="type"/> is the same type as <paramref name="otherType"/> or its subtype.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="otherType">The type to compare with.</param>
    /// <param name="exactlyEquals">Requires <paramref name="type"/> and <paramref name="otherType"/> to be the same.</param>
    /// <param name="includeNullability">Whether to include nullability of types into comparison.</param>
    /// <remarks>
    /// This does not take covariance into account. For the fully correct check use
    /// <see cref="Is(ITypeSymbol?,ITypeSymbol?,Compilation,bool,bool)"/>.
    /// </remarks>
    public static bool Is(this ITypeSymbol? type, ITypeSymbol? otherType, bool exactlyEquals = false, bool includeNullability = false)
    {
        for (var currentType = type; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.Equals(otherType, SymbolEqualityComparer.Default))
            {
                if (exactlyEquals && includeNullability)
                    return currentType.Equals(otherType, SymbolEqualityComparer.IncludeNullability);

                if (!includeNullability)
                    return true;

                if (otherType is not (INamedTypeSymbol { IsGenericType: true } or IArrayTypeSymbol) && currentType.NullableAnnotation.Is(otherType.NullableAnnotation))
                    return true;

                if (currentType is INamedTypeSymbol { IsGenericType: true } namedType)
                {
                    if (otherType is INamedTypeSymbol { IsGenericType: true } otherNamedType &&
                        namedType.Arity == otherNamedType.Arity &&
                        namedType.TypeArguments.Zip(otherNamedType.TypeArguments,
                                (typeArgument, otherTypeArgument) =>
                                    typeArgument.NullableAnnotation.Is(otherTypeArgument.NullableAnnotation))
                            .All(v => v))
                        return true;

                    if (otherType is IArrayTypeSymbol otherArrayType &&
                        namedType.Arity == 1 &&
                        namedType.TypeArguments[0].NullableAnnotation.Is(otherArrayType.ElementNullableAnnotation))
                        return true;
                }
                else if (currentType is IArrayTypeSymbol arrayType)
                {
                    if (otherType is INamedTypeSymbol { IsGenericType: true, Arity: 1 } otherNamedType &&
                        arrayType.ElementNullableAnnotation.Is(otherNamedType.TypeArguments[0].NullableAnnotation))
                        return true;

                    if (otherType is IArrayTypeSymbol otherArrayType &&
                        arrayType.ElementNullableAnnotation.Is(otherArrayType.ElementNullableAnnotation))
                        return true;                    
                }
            }

            if (exactlyEquals)
                return false;

            foreach (var interfaceType in currentType.Interfaces)
                if (interfaceType.Is(otherType))
                    return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether <paramref name="type"/> is the same type as <paramref name="otherType"/> or its subtype.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="otherType">The type to compare with.</param>
    /// <param name="compilation">The compilation to which <paramref name="type"/> and <paramref name="otherType"/> belong.</param>
    /// <param name="exactlyEquals">Requires <paramref name="type"/> and <paramref name="otherType"/> to be the same.</param>
    /// <param name="includeNullability">Whether to include nullability of types into comparison.</param>
    public static bool Is(this ITypeSymbol? type, ITypeSymbol? otherType, Compilation compilation, 
        bool exactlyEquals = false, bool includeNullability = false)
    {
        if (exactlyEquals)
            return type.Is(otherType, true, includeNullability);

        if (!compilation.HasImplicitConversion(type, otherType))
            return false;

        if (includeNullability && type is not null && otherType is not null)
        {
            if (!type.NullableAnnotation.Is(otherType.NullableAnnotation))
                return false;

            if (otherType is INamedTypeSymbol { IsGenericType: true } || otherType is IArrayTypeSymbol)
                return type.Is(otherType, false, true);
        }

        return true;
    }

    private static bool Is(this NullableAnnotation annotation, NullableAnnotation otherAnnotation)
    {
        if (annotation == otherAnnotation)
            return true;

        if (annotation == NullableAnnotation.None || otherAnnotation == NullableAnnotation.None)
            return true;

        return otherAnnotation == NullableAnnotation.Annotated;
    }

    public static bool IsMatch(this IMethodSymbol? method, params ITypeSymbol[] argumentTypes)
    {
        if (method is null || method.Parameters.Length != argumentTypes.Length)
            return false;

        for (var i = 0; i < argumentTypes.Length; i++)
            if (!method.Parameters[i].Type.Equals(argumentTypes[i], SymbolEqualityComparer.Default))
                return false;

        return true;
    }

    public static IEnumerable<IMethodSymbol> GetLocalFunctions(this IMethodSymbol method, Compilation compilation, CancellationToken cancellationToken = default)
    {
        foreach (var syntaxReference in method.DeclaringSyntaxReferences)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxReference.SyntaxTree);
            var methodBodyOperation = semanticModel.GetOperation(syntaxReference.GetSyntax(cancellationToken)) as IMethodBodyOperation;

            if (methodBodyOperation?.BlockBody is null)
                continue;

            foreach (var operation in methodBodyOperation.BlockBody.Operations.OfType<ILocalFunctionOperation>())
                yield return operation.Symbol;
        }
    }

    public static bool IsInNullableContext(this ISymbol symbol, Compilation compilation)
    {
        foreach (var syntaxReference in symbol.DeclaringSyntaxReferences)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxReference.SyntaxTree);
            if ((semanticModel.GetNullableContext(syntaxReference.Span.Start) & NullableContext.Enabled) != 0)
                return true;
        }

        return false;
    }

    public static IEnumerable<ISymbol> GetMembersIncludingBaseTypes(this ITypeSymbol type)
    {
        for (var currentType = type; currentType is not null; currentType = currentType.BaseType)
            foreach (var member in currentType.GetMembers())
                yield return member;
    }

    public static IEnumerable<ISymbol> GetMembersIncludingBaseTypes(this ITypeSymbol type, string name)
    {
        for (var currentType = type; currentType is not null; currentType = currentType.BaseType)
            foreach (var member in currentType.GetMembers(name))
                yield return member;
    }
}