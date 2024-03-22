using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HarmonyTools.Analyzers;

internal class PatchMethod(IMethodSymbol method, ImmutableArray<DetailWithSyntax<PatchMethodKind>> methodKinds, 
    PatchDescription? patchDescription) : IHasSyntax
{
    public IMethodSymbol Method { get; } = method;
    public ImmutableArray<DetailWithSyntax<PatchMethodKind>> MethodKinds { get; } = methodKinds;
    public PatchDescription? PatchDescription { get; protected set; } = patchDescription;
    public ImmutableArray<PatchMethodParameter> Parameters { get; private set; }

    public override string ToString() => Method.ToString();

    public SyntaxNode? Syntax => Method.GetSyntax();

    public Location? GetLocation(CancellationToken cancellationToken = default) =>
        Method.GetSyntax(cancellationToken: cancellationToken)?.GetIdentifierLocation();

    public Location? GetRefReturnLocation(CancellationToken cancellationToken = default)
    {
        var typeSyntax = Method.GetSyntax(cancellationToken: cancellationToken) switch
        {
            MethodDeclarationSyntax methodDeclarationSyntax => methodDeclarationSyntax.ReturnType,
            DelegateDeclarationSyntax delegateDeclarationSyntax => delegateDeclarationSyntax.ReturnType,
            _ => null
        };
        if (typeSyntax is not RefTypeSyntax refTypeSyntax)
            return null;
        var location = refTypeSyntax.RefKeyword.GetLocation();
        return !location.IsInSource ? null : location;
    }

    public INamedTypeSymbol? TargetType => PatchDescription?.TargetType;

    public IMethodSymbol? TargetMethod => PatchDescription?.TargetMethod;

    public bool Is(PatchMethodKind kind) => MethodKinds.Contains(kind);

    public bool IsPrimary => MethodKinds.Any(kind => kind.Value.IsPrimary());

    public bool IsPatchAll => PatchDescription is { IsPatchAll.Value: true };

    public IEnumerable<IMethodSymbol> GetTargetMethods()
    {
        if (TargetMethod is not null)
            return [TargetMethod];

        if (IsPatchAll && TargetType is not null)
            return TargetType.GetMembers().OfType<IMethodSymbol>();

        return [];
    }

    public bool? IsPassthrough() => Is(PatchMethodKind.Postfix) 
        ? TargetMethod is not null && !Parameters.IsDefault
            ? Parameters.FirstOrDefault()?.IsPassthrough ?? false
            : null
        : false;
    
    public bool ContainsTranspiler(WellKnownTypes wellKnownTypes, Compilation compilation, CancellationToken cancellationToken = default) =>
        Method.GetLocalFunctions(compilation, cancellationToken)
            .Any(innerMethod => innerMethod.ReturnType.Is(wellKnownTypes.EnumerableOfCodeInstruction));

    public void UpdateParameters(WellKnownTypes wellKnownTypes, Compilation compilation) => 
        Parameters = PatchMethodParameter.GetParameters(this, wellKnownTypes, compilation);
}

internal class PatchMethod<TPatchDescription>(IMethodSymbol method, ImmutableArray<DetailWithSyntax<PatchMethodKind>> methodKinds, 
    TPatchDescription? patchDescription) : PatchMethod(method, methodKinds, patchDescription)
    where TPatchDescription : PatchDescription
{
    public new TPatchDescription? PatchDescription
    {
        get => (TPatchDescription?)base.PatchDescription;
        set => base.PatchDescription = value;
    }
}