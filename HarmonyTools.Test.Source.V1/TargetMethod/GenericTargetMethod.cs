using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1.TargetMethod
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.GenericMethod))]
    internal class GenericTargetMethod
    {
        public static void Postfix() { }
    }
}