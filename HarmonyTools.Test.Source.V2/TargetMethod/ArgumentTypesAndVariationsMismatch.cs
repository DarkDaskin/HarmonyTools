using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.TargetMethod
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

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod))]
    internal class ArgumentTypesAndVariationsMismatch3
    {
        [HarmonyPatch(new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal })]
        public static void Postfix() { }
    }

    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod))]
    [HarmonyDelegate(MethodDispatchType.Call, new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal })]
    internal delegate int ArgumentTypesAndVariationsMismatch4(int x, int y);
}