using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class MultipleAuxiliaryPatchMethods
    {
        public void Prepare() { }

        [HarmonyPrepare]
        public void Prepare2() { }

        public static void Postfix() { }
    }
}