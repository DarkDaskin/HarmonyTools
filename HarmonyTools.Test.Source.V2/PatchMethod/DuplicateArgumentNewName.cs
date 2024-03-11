using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    [HarmonyArgument(0, "foo")]
    internal class DuplicateArgumentNewName
    {
        [HarmonyArgument(1, "foo")]
        public static void Postfix(int foo) { }
    }
}