using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;
// ReSharper disable InconsistentNaming

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch]
    internal class InvalidPatchMethodByRefParameters1
    {
        public static void Prepare(ref MethodBase original, ref Harmony harmony) { }

        public static void Cleanup(ref MethodBase original, ref Exception exception, ref Harmony harmony) { }

        public static MethodBase TargetMethod(ref Harmony harmony) => default;

        public static IEnumerable<CodeInstruction> Transpiler(ref IEnumerable<CodeInstruction> instructions,
            ref ILGenerator generator, ref MethodBase original) => instructions;

        public static void Prefix(ref object __instance, ref object[] __args, ref MethodBase __originalMethod, ref bool __runOriginal) { }

        public static void Postfix(ref object __instance, ref object[] __args, ref MethodBase __originalMethod, ref bool __runOriginal) { }
    }

    [HarmonyPatch]
    internal class InvalidPatchMethodByRefParameters2
    {
        public static IEnumerable<MethodBase> TargetMethods(ref Harmony harmony) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class InvalidPatchMethodByRefParameters3
    {
        public static void Prefix(ref SimpleStaticMethod @delegate) { }

        public static void Postfix(ref SimpleStaticMethod @delegate) { }

        public static void Finalizer(ref SimpleClass __instance, ref object[] __args, ref MethodBase __originalMethod, ref bool __runOriginal,
            ref Exception __exception, ref SimpleStaticMethod @delegate) { }

        [HarmonyReversePatch]
        public static int ReversePatch1(ref SimpleClass instance, ref string question) => default;

        [HarmonyReversePatch, HarmonyPatch(nameof(SimpleClass.OverloadedMethod), 
             new[] { typeof(int) }, new[] { ArgumentType.Out })]
        public static void ReversePatch2(SimpleClass instance, int answer) { }

        [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleStaticMethod))]
        public delegate int SimpleStaticMethod(ref string question);

        [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), 
            new[] { typeof(int) }, new[] { ArgumentType.Out })]
        public delegate void OverloadedMethod(int answer);
    }
}