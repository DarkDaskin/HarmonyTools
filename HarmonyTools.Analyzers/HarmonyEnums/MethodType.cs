namespace HarmonyTools.Analyzers.HarmonyEnums;

/// <summary>Specifies the type of method</summary>
internal enum MethodType
{
    /// <summary>This is a normal method</summary>
    Normal,
    /// <summary>This is a getter</summary>
    Getter,
    /// <summary>This is a setter</summary>
    Setter,
    /// <summary>This is a constructor</summary>
    Constructor,
    /// <summary>This is a static constructor</summary>
    StaticConstructor,
    /// <summary>This targets the MoveNext method of the enumerator result, that actually contains the method's implementation</summary>
    /// <remarks>Only valid in Harmony 2.x.</remarks>
    Enumerator,
    /// <summary>This targets the MoveNext method of the async state machine, that actually contains the method's implementation</summary>
    /// <remarks>Only valid in Harmony 2.x.</remarks>
    Async,
}