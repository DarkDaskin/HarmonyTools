﻿namespace HarmonyTools.Analyzers;

public enum MethodType
{
    Normal,
    Getter,
    Setter,
    Constructor,
    StaticConstructor,
    Enumerator,
    Async,
}