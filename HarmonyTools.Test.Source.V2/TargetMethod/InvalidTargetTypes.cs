using System.Collections.Generic;
using HarmonyLib;

namespace HarmonyTools.Test.Source.V2.TargetMethod
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