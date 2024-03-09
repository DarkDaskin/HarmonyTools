using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class MultiplePatchMethodKinds
    {
        [HarmonyPostfix]
        public void Prefix() { }

        [HarmonyPrepare, HarmonyCleanup, HarmonyPrefix]
        public void InvalidMethod() { }

        [HarmonyPrefix]
        public void ValidMethod() { }
    }
}