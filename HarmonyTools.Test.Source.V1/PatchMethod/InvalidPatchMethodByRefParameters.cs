using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
// ReSharper disable InconsistentNaming


namespace HarmonyTools.Test.Source.V1.PatchMethod
{
    [HarmonyPatch]
    internal class InvalidPatchMethodByRefParameters1
    {
        public static void Prepare(ref MethodBase original, ref HarmonyInstance harmony) { }

        public static void Cleanup(ref MethodBase original, ref Exception exception, ref HarmonyInstance harmony) { }

        public static MethodBase TargetMethod(ref HarmonyInstance harmony) => default;

        public static IEnumerable<CodeInstruction> Transpiler(ref IEnumerable<CodeInstruction> instructions,
            ref ILGenerator generator, ref MethodBase original) => instructions;

        public static void Prefix(ref object __instance, ref object[] __args, ref MethodBase __originalMethod, ref bool __runOriginal) { }

        public static void Postfix(ref object __instance, ref object[] __args, ref MethodBase __originalMethod, ref bool __runOriginal) { }
    }

    [HarmonyPatch]
    internal class InvalidPatchMethodByRefParameters2
    {
        public static IEnumerable<MethodBase> TargetMethods(ref HarmonyInstance harmony) => default;

        public static void Postfix() { }
    }
}