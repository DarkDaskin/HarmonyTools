using System.Collections.Generic;
using System.Reflection;
using Harmony;

namespace HarmonyTools.Test.Source.V1
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