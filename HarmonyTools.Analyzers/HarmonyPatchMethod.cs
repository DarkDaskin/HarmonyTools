using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchMethod(IMethodSymbol method, ImmutableArray<DetailWithSyntax<PatchMethodKind>> methodKinds, 
    HarmonyPatchDescription? patchDescription) : IHasSyntax
{
    public IMethodSymbol Method { get; } = method;
    public ImmutableArray<DetailWithSyntax<PatchMethodKind>> MethodKinds { get; } = methodKinds;
    public HarmonyPatchDescription? PatchDescription { get; protected set; } = patchDescription;

    public override string ToString() => Method.ToString();

    public SyntaxNode? Syntax => Method.GetSyntax();

    public IMethodSymbol? TargetMethod => PatchDescription?.TargetMethod;

    public bool Is(PatchMethodKind kind) => MethodKinds.Contains(kind);

    public bool IsPrimary => MethodKinds.Any(kind => kind.Value.IsPrimary());

    // TODO: Check if parameter can be bound to original method parameter. Without that we can't be sure.
    //public bool? IsPassthrough() => Is(PatchMethodKind.Postfix) &&
    //                                (Method.Parameters.FirstOrDefault()?.Type.Is(TargetMethod?.ReturnType, true) ?? false);
    public bool? IsPassthrough() => Is(PatchMethodKind.Postfix) ? null : false;

    public bool ContainsTranspiler(WellKnownTypes wellKnownTypes, Compilation compilation, CancellationToken cancellationToken = default) =>
        Method.GetLocalFunctions(compilation, cancellationToken)
            .Any(innerMethod => innerMethod.ReturnType.Is(wellKnownTypes.EnumerableOfCodeInstruction));
}

internal class HarmonyPatchMethod<TPatchDescription>(IMethodSymbol method, ImmutableArray<DetailWithSyntax<PatchMethodKind>> methodKinds, 
    TPatchDescription? patchDescription) : HarmonyPatchMethod(method, methodKinds, patchDescription)
    where TPatchDescription : HarmonyPatchDescription
{
    public new TPatchDescription? PatchDescription
    {
        get => (TPatchDescription?)base.PatchDescription;
        set => base.PatchDescription = value;
    }
}