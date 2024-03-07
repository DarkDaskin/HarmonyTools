using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod))]
    internal class AmbiguousMethod1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Getter)]
    internal class AmbiguousIndexer
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(MultipleConstructors), MethodType.Constructor)]
    internal class AmbiguousConstructor
    {
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class AmbiguousMethod2
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod))]
        public static void Postfix() { }
    }

    [HarmonyPatch("HarmonyTools.Test.PatchBase.SimpleClass", nameof(SimpleClass.OverloadedMethod))]
    internal class AmbiguousMethod3
    {
        public static void Postfix() { }
    }
}