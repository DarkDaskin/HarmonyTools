using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyPatch(typeof(SimpleClass))]
    internal class OverspecifiedMethod1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
    internal class OverspecifiedMethod2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.ReadOnlyProp), MethodType.Getter)]
    [HarmonyPatch(nameof(SimpleClass.ReadWriteProp))]
    internal class OverspecifiedMethod3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    [HarmonyPatch(new[] { typeof(double), typeof(double) })]
    internal class OverspecifiedMethod4
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), new[] { typeof(int), typeof(int) },
        new[] { ArgumentType.Normal, ArgumentType.Normal })]
    [HarmonyPatch(new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal, ArgumentType.Normal })]
    internal class OverspecifiedMethod5
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.ReadOnlyProp), MethodType.Getter)]
    [HarmonyPatch(MethodType.Setter)]
    [HarmonyPatch(MethodType.Setter)]
    internal class OverspecifiedMethod6
    {
        public static void Postfix() { }
    }
}