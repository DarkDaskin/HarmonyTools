namespace HarmonyTools.Analyzers;

internal enum InjectionKind
{
    None,

    ParameterByName,
    ParameterByIndex,
    FieldByName,
    FieldByIndex,
    Delegate,
    Instance,
    Result,
    ResultRef,
    State,
    Args,
    OriginalMethod,
    RunOriginal,
    Exception,

    HarmonyInstance,
    Instructions,
    // ReSharper disable once InconsistentNaming
    ILGenerator,

    ParameterByPosition,
}