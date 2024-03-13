using System.Collections.Generic;
using System.Reflection;
using Harmony;

namespace HarmonyTools.Test.Source.V1.PatchMethod
{
    [HarmonyPatch]
    internal class NonStaticPatchMethods
    {
        public static void Prefix() { }

        public static void Postfix() { }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => instructions;

        public static void Prepare() { }

        public static void Cleanup() { }

        public static MethodBase TargetMethod() => default;

        public void NonPatchMethod() { }
    }
}