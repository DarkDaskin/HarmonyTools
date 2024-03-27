using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.TargetMethod
{
    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatchAll]
    public class BulkPatchingMethodsWithReversePatch1
    {
        public static void ReversePatch() { }
    }

    [HarmonyPatch]
    public class BulkPatchingMethodsWithReversePatch2
    {
        public static IEnumerable<MethodBase> TargetMethods() => default;

        public static void ReversePatch() { }
    }
}