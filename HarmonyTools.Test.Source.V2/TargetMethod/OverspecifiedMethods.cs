using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.TargetMethod
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

    [HarmonyPatch]
    internal class OverspecifiedMethod7
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
        public static void Postfix() { }
    }

    [HarmonyPatch("HarmonyTools.Test.PatchBase.SimpleClass", nameof(SimpleClass.SimpleMethod))]
    [HarmonyPatch("HarmonyTools.Test.PatchBase.SimpleClass", nameof(SimpleClass.OverloadedMethod))]
    internal class OverspecifiedMethod8
    {
        public static void Postfix() { }
    }

    [HarmonyPatch("HarmonyTools.Test.PatchBase.SimpleClass", nameof(SimpleClass.SimpleMethod))]
    [HarmonyPatch(typeof(SimpleClass))]
    internal class OverspecifiedMethod9
    {
        public static void Postfix() { }
    }

    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod), MethodDispatchType.Call)]
    [HarmonyDelegate(MethodDispatchType.VirtualCall)]
    internal delegate int OverspecifiedMethodDelegate(string question);
}