using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class SimplePatch
    {
        public static void Prefix(ref string question)
        {

        }

        // ReSharper disable once InconsistentNaming
        public static void Postfix(ref int __result)
        {

        }
    }
}