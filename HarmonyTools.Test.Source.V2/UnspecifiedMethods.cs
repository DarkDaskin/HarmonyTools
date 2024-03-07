﻿using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch]
    internal class UnspecifiedMethod1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    internal class UnspecifiedMethod2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
    internal class UnspecifiedMethod3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(nameof(SimpleClass.ReadOnlyProp), MethodType.Getter)]
    internal class UnspecifiedMethod4
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(MethodType.Constructor)]
    internal class UnspecifiedMethod5
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(new[] { typeof(string) })]
    internal class UnspecifiedMethod6
    {
        public static void Postfix() { }
    }
}