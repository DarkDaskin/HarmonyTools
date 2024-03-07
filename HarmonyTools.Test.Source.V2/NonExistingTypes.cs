using HarmonyLib;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch("HarmonyTools.Test.PatchBase.NonExistingClass", "Method")]
    public class NonExistingType1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch]
    public class NonExistingType2
    {
        [HarmonyPatch("HarmonyTools.Test.PatchBase.NonExistingClass", "Method")]
        public static void Postfix() { }
    }
}