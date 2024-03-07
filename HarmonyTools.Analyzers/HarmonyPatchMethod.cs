using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchMethod(IMethodSymbol method, ImmutableArray<PatchMethodKind> methodKinds, 
    HarmonyPatchDescription? patchDescription)
{
    public IMethodSymbol Method { get; } = method;
    public ImmutableArray<PatchMethodKind> MethodKinds { get; } = methodKinds;
    public HarmonyPatchDescription? PatchDescription { get; } = patchDescription;

    public override string ToString() => Method.ToString();
}

internal class HarmonyPatchMethod<TPatchDescription>(IMethodSymbol method, ImmutableArray<PatchMethodKind> methodKinds, 
    TPatchDescription? patchDescription) : HarmonyPatchMethod(method, methodKinds, patchDescription)
    where TPatchDescription : HarmonyPatchDescription
{
    public new TPatchDescription? PatchDescription => (TPatchDescription?)base.PatchDescription;
}