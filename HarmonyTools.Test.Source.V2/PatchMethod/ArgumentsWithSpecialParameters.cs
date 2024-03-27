using HarmonyLib;
using HarmonyTools.Test.PatchBase;
// ReSharper disable InconsistentNaming

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyArgument(0, "__0")]
    internal class ArgumentsWithSpecialParameters
    {
        [HarmonyArgument(0, "____answer")]
        public static void Postfix(string __0, [HarmonyArgument(0)] int __result, [HarmonyArgument(0)] int ____answer) { }
    }
}