using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace HarmonyTools.Test.Source.V2.PatchMethod
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

        public void Finalizer() { }

        public void ReversePatch() { }

        public void NonPatchMethod() { }
    }
}