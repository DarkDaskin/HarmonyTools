using System.Collections.Immutable;

namespace HarmonyTools.Analyzers;

internal class PatchDescriptionSet<TPatchDescription>(TPatchDescription? typePatchDescription, 
    ImmutableArray<PatchMethod<TPatchDescription>> patchMethods)
    where TPatchDescription : PatchDescription
{
    public static readonly PatchDescriptionSet<TPatchDescription> Empty = new(null, []);

    public TPatchDescription? TypePatchDescription { get; } = typePatchDescription;
    public ImmutableArray<PatchMethod<TPatchDescription>> PatchMethods { get; } = patchMethods;
}