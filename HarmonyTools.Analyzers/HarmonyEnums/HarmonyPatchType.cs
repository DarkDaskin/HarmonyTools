namespace HarmonyTools.Analyzers.HarmonyEnums;

/// <summary>Specifies the type of patch</summary>
internal enum HarmonyPatchType
{
    /// <summary>Any patch</summary>
    All,
    /// <summary>A prefix patch</summary>
    Prefix,
    /// <summary>A postfix patch</summary>
    Postfix,
    /// <summary>A transpiler</summary>
    Transpiler,
    /// <summary>A finalizer</summary>
    /// <remarks>Only valid in Harmony 2.x.</remarks>
    Finalizer,
    /// <summary>A reverse patch</summary>
    /// <remarks>Only valid in Harmony 2.x.</remarks>
    ReversePatch,
}