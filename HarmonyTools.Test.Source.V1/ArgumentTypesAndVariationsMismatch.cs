using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod))]
    [HarmonyPatch(new[] { typeof(int) }, new[] { ArgumentType.Normal, ArgumentType.Normal })]
    internal class ArgumentTypesAndVariationsMismatch1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod))]
    [HarmonyPatch(new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal })]
    internal class ArgumentTypesAndVariationsMismatch2
    {
        public static void Postfix() { }
    }
}