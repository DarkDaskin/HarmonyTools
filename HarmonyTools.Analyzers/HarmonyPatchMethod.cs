using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchMethod(IMethodSymbol method, ImmutableArray<DetailWithSyntax<PatchMethodKind>> methodKinds, 
    HarmonyPatchDescription? patchDescription) : IHasSyntax
{
    public IMethodSymbol Method { get; } = method;
    public ImmutableArray<DetailWithSyntax<PatchMethodKind>> MethodKinds { get; } = methodKinds;
    public HarmonyPatchDescription? PatchDescription { get; } = patchDescription;

    public override string ToString() => Method.ToString();

    public SyntaxNode? Syntax => Method.GetSyntax();
}

internal class HarmonyPatchMethod<TPatchDescription>(IMethodSymbol method, ImmutableArray<DetailWithSyntax<PatchMethodKind>> methodKinds, 
    TPatchDescription? patchDescription) : HarmonyPatchMethod(method, methodKinds, patchDescription)
    where TPatchDescription : HarmonyPatchDescription
{
    public new TPatchDescription? PatchDescription => (TPatchDescription?)base.PatchDescription;
}