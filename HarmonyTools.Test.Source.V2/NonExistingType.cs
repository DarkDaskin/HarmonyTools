using HarmonyLib;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch("HarmonyTools.Test.PatchBase.NonExistingClass", "Method")]
    public class NonExistingType
    {
        public static void Postfix() { }
    }
}