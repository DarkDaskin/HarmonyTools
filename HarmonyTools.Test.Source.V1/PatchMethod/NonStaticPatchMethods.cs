using System.Collections.Generic;
using System.Reflection;
using Harmony;

namespace HarmonyTools.Test.Source.V1.PatchMethod
{
    [HarmonyPatch]
    internal class NonStaticPatchMethods
    {
        public void Prefix() { }

        public void Postfix() { }

        public IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => instructions;

        public void Prepare() { }

        public void Cleanup() { }

        public MethodBase TargetMethod() => default;

        public void NonPatchMethod() { }
    }
}