using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using HarmonyTools.Test.PatchBase;
// ReSharper disable InconsistentNaming

namespace HarmonyTools.Test.Source.V1.PatchMethod
{
    [HarmonyPatch]
    internal class UnrecognizedAuxiliaryMethodParameters1
    {
        public static void Prepare(object harmony, Exception exception) { }

        public static void Cleanup(object harmony) { }

        public static MethodBase TargetMethod(object harmony, MethodBase original, Exception exception) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class UnrecognizedAuxiliaryMethodParameters2
    {
        public static IEnumerable<MethodBase> TargetMethods(object harmony, MethodBase original, Exception exception) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyArgument("foo", "foo3")]
    internal class UnrecognizedPrimaryMethodParameters1
    {
        [HarmonyArgument(1, "foo4")]
        public static void Prefix(string foo, string __1, [HarmonyArgument("foo")] string foo1,
            [HarmonyArgument(1)] string foo2, string foo3, string foo4, Action action, object ___foo, object ___10, Exception __exception) { }

        [HarmonyPostfix]
        [HarmonyArgument(1, "foo4")]
        public static void Postfix1(string foo, string __1, [HarmonyArgument("foo")] string foo1,
            [HarmonyArgument(1)] string foo2, string foo3, string foo4, Action action, object ___foo, object ___10, Exception __exception) { }

        [HarmonyPostfix]
        public static void Postfix2(int result) { }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, object generator) => instructions;
    }

    [HarmonyPatch(typeof(SimpleClass2)), HarmonyPatchAll]
    internal class UnrecognizedPrimaryMethodParameters2
    {
        public static void Prefix(string question, string __0, object ___foo, object ___10, SimpleClass2 __instance) { }

        public static void Postfix(string question, string __0, object ___foo, object ___10, SimpleClass2 __instance) { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleStaticMethod))]
    internal class InstanceParameterForStaticMethod
    {
        public static void Postfix(SimpleClass __instance) { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.DoNothing))]
    internal class ResultParameterForVoidMethod
    {
        public static void Postfix(object __result) { }
    }
}