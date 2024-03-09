using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod)), HarmonyPatchAll]
    internal class IndividualAnnotationsWithBulkPatching1
    {
        public static void Postfix() { }
    }

    [HarmonyPatchAll]
    internal class IndividualAnnotationsWithBulkPatching2
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor), HarmonyPatchAll]
    internal class IndividualAnnotationsWithBulkPatching3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Getter, new[] { typeof(int) }), HarmonyPatchAll]
    internal class IndividualAnnotationsWithBulkPatching4
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPriority(4)]
    internal class IndividualAnnotationsWithBulkPatching5
    {
        public static MethodBase TargetMethod() => default;

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    internal class IndividualAnnotationsWithBulkPatching6
    {
        public static IEnumerable<MethodBase> TargetMethods() => default;

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
    internal class IndividualAnnotationsWithBulkPatching7
    {
        [HarmonyTargetMethod]
        public static MethodBase GetTargetMethod() => default;

        public static void Postfix() { }
    }
}