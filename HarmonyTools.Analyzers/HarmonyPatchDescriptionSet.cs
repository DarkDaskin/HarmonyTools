using System.Collections.Immutable;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchDescriptionSet<TPatchDescription>(TPatchDescription? typePatchDescription, 
    ImmutableArray<HarmonyPatchMethod<TPatchDescription>> patchMethods)
    where TPatchDescription : HarmonyPatchDescription
{
    public static readonly HarmonyPatchDescriptionSet<TPatchDescription> Empty = new(null, []);

    public TPatchDescription? TypePatchDescription { get; } = typePatchDescription;
    public ImmutableArray<HarmonyPatchMethod<TPatchDescription>> PatchMethods { get; } = patchMethods;
}