namespace HarmonyTools.Analyzers;

internal enum PatchMethodKind
{
    Prefix,
    Postfix,
    Transpiler,
    /// <remarks>Only valid in Harmony 2.x.</remarks>
    Finalizer,
    /// <remarks>Only valid in Harmony 2.x.</remarks>
    ReversePatch,
    Prepare,
    Cleanup,
    TargetMethod,
    TargetMethods,
}

internal static class PatchMethodKindExtensions
{
    public static bool IsPrimary(this PatchMethodKind kind) => kind is >= PatchMethodKind.Prefix and <= PatchMethodKind.ReversePatch;

    public static bool IsAuxiliary(this PatchMethodKind kind) => kind is >= PatchMethodKind.Prepare and <= PatchMethodKind.TargetMethods;

    public static bool IsHarmony2Only(this PatchMethodKind kind) => kind is >= PatchMethodKind.Finalizer and <= PatchMethodKind.ReversePatch;
}