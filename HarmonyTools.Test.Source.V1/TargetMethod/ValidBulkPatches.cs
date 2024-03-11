using System.Collections.Generic;
using System.Reflection;
using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1.TargetMethod
{
    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatchAll]
    internal class ValidBulkPatchPatchAll1
    {
        public static void Postfix() { }
    }

    [HarmonyPatchAll]
    internal class ValidBulkPatchPatchAll2
    {
        [HarmonyPatch(typeof(SimpleClass))]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidBulkPatchTargetMethod
    {
        public static MethodBase TargetMethod() => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidBulkPatchTargetMethods
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> FindTargetMethods() => default;

        public static void Postfix() { }
    }
}