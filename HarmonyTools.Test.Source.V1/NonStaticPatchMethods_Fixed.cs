using System.Reflection;
using Harmony;

namespace HarmonyTools.Test.Source.V1
{
    [HarmonyPatch]
    internal class NonStaticPatchMethods
    {
        public static void Prefix() { }

        public static void Postfix() { }

        public static void Transpiler() { }
        
        public static void Prepare() { }

        public static void Cleanup() { }

        public static MethodBase TargetMethod() => default;

        public void NonPatchMethod() { }
    }
}