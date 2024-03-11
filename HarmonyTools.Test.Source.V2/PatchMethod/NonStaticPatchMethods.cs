using System.Reflection;
using HarmonyLib;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch]
    internal class NonStaticPatchMethods
    {
        public void Prefix() { }

        public void Postfix() { }

        public void Transpiler() { }

        public void Prepare() { }

        public void Cleanup() { }

        public MethodBase TargetMethod() => default;

        public void Finalizer() { }

        public void ReversePatch() { }

        public void NonPatchMethod() { }
    }
}