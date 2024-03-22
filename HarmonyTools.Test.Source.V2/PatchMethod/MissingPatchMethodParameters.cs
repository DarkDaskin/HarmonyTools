using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch(typeof(SimpleClass))]
    internal class MissingPatchMethodParameters
    {
        [HarmonyReversePatch, HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static int ReversePatch1(string question1) => default;

        [HarmonyReversePatch, HarmonyPatch(nameof(SimpleClass.SimpleStaticMethod))]
        public static int ReversePatch2() => default;
    }

    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal delegate int MissingDelegateParameters();
}