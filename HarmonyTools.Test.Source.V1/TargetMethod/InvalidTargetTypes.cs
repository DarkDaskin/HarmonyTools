using System.Collections.Generic;
using Harmony;

namespace HarmonyTools.Test.Source.V1.TargetMethod
{
    [HarmonyPatch(typeof(int[]), "Method")]
    internal class ArrayTargetType
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(int*), "Method")]
    internal class PointerTargetType
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(List<>), "Add")]
    internal class OpenGenericTargetType
    {
        public static void Postfix() { }
    }
}