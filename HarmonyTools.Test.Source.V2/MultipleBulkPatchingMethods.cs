using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatchAll]
    internal class MultipleBulkPatchingMethods1
    {
        public static MethodBase TargetMethod() => default;
    }

    [HarmonyPatch]
    internal class MultipleBulkPatchingMethods2
    {
        public static MethodBase TargetMethod() => default;

        public static IEnumerable<MethodBase> TargetMethods() => default;
    }
}