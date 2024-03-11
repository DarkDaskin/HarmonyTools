using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class MultiplePatchMethodKinds
    {
        [HarmonyPostfix]
        public static void Prefix() { }

        [HarmonyPrepare, HarmonyCleanup, HarmonyPrefix]
        public static void InvalidMethod() { }

        [HarmonyPrefix]
        public static void ValidMethod() { }
    }
}