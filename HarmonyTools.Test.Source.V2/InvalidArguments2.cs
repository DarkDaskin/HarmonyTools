using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyBefore, HarmonyAfter]
    internal class InvalidArguments21
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyBefore(null), HarmonyAfter(null)]
    internal class InvalidArguments22
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyBefore("foo", null, ""), HarmonyAfter("foo", null, "")]
    internal class InvalidArguments23
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyArgument(null), HarmonyArgument(null, null)]
    [HarmonyArgument(""), HarmonyArgument("", "")]
    [HarmonyArgument(-1), HarmonyArgument(-1, "foo")]
    internal class InvalidArguments24
    {
        public static void Postfix() { }
    }
}