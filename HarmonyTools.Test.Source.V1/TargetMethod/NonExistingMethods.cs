using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1.TargetMethod
{
    [HarmonyPatch(typeof(SimpleClass), "NonExistingMethod")]
    internal class NonExistingMethod1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(string))]
    internal class NonExistingOverload1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), new[] { typeof(int), typeof(int) },
        new[] { ArgumentType.Normal, ArgumentType.Pointer })]
    internal class NonExistingOverload2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor, typeof(string))]
    internal class NonExistingConstructor
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(NoStaticConstructor), MethodType.StaticConstructor)]
    internal class NonExistingStaticConstructor
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), "NonExistingProp", MethodType.Getter)]
    internal class NonExistingProperty
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.ReadOnlyProp), MethodType.Setter)]
    internal class NonExistingPropertyAccessor
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Getter, typeof(string))]
    internal class NonExistingIndexer
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    internal class NonExistingMethod2
    {
        [HarmonyPatch("NonExistingMethod")]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleSubClass), nameof(SimpleClass.SimpleMethod))]
    internal class NonExistingMethod3
    {
        public static void Postfix() { }
    }
}