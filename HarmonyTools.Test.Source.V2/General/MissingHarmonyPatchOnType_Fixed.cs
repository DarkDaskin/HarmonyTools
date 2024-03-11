using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.General
{
    [HarmonyPatch]
    internal class MissingHarmonyPatchOnType
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
        public static void Prefix() { }

        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
        public static void Postfix() { }
    }
}