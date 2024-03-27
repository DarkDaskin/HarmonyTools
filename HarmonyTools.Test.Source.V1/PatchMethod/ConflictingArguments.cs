using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1.PatchMethod
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    [HarmonyArgument(0, "foo")]
    internal class ConflictingArguments
    {
        [HarmonyPostfix, HarmonyArgument(1, "foo")]
        public static void Postfix1(int foo) { }

        [HarmonyPostfix]
        public static void Postfix2([HarmonyArgument(1)] int foo) { }

        [HarmonyPostfix, HarmonyArgument(0, "bar")]
        public static void Postfix3(int foo, [HarmonyArgument(1)] int bar) { }
    }
}