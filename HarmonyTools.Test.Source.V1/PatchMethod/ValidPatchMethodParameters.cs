using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using HarmonyTools.Test.PatchBase;
// ReSharper disable InconsistentNaming

namespace HarmonyTools.Test.Source.V1.PatchMethod
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidPrepareParameters1
    {
        public static void Prepare() { }

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidPrepareParameters2
    {
        public static void Prepare(MethodBase original, HarmonyInstance harmony) { }

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidPrepareParameters3
    {
        public static void Prepare(HarmonyInstance harmony1, MethodBase original1) { }

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidPrepareParameters4
    {
#nullable enable
        public static void Prepare(MethodBase? original, HarmonyInstance harmony) { }
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidPrepareParameters5
    {
#nullable enable
        public static void Prepare(MethodBase? original, HarmonyInstance? harmony) { }
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidCleanupParameters1
    {
        public static void Cleanup() { }

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidCleanupParameters2
    {
        public static void Cleanup(MethodBase original, HarmonyInstance harmony, Exception exception) { }

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidCleanupParameters3
    {
        public static void Cleanup(HarmonyInstance harmony1, Exception exception1, MethodBase original1) { }

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidCleanupParameters4
    {
#nullable enable
        public static void Cleanup(MethodBase? original, HarmonyInstance harmony, Exception exception) { }
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidCleanupParameters5
    {
#nullable enable
        public static void Cleanup(MethodBase? original, HarmonyInstance? harmony, Exception? exception) { }
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodParameters1
    {
        public static MethodBase TargetMethod() => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodParameters2
    {
        public static MethodBase TargetMethod(HarmonyInstance harmony) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodParameters3
    {
        public static MethodBase TargetMethod(HarmonyInstance harmony1) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodParameters4
    {
#nullable enable
        public static MethodBase TargetMethod(HarmonyInstance harmony) => default!;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodParameters5
    {
#nullable enable
        public static MethodBase TargetMethod(HarmonyInstance? harmony) => default!;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodsParameters1
    {
        public static IEnumerable<MethodBase> TargetMethods() => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodsParameters2
    {
        public static IEnumerable<MethodBase> TargetMethods(HarmonyInstance harmony) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodsParameters3
    {
        public static IEnumerable<MethodBase> TargetMethods(HarmonyInstance harmony1) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodsParameters4
    {
#nullable enable
        public static IEnumerable<MethodBase> TargetMethods(HarmonyInstance harmony) => default!;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodsParameters5
    {
#nullable enable
        public static IEnumerable<MethodBase> TargetMethods(HarmonyInstance? harmony) => default!;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyArgument(0, "question4")]
    internal class ValidPrefixPostfixParameters1
    {
        [HarmonyPrefix]
        public static void Prefix1() { }

        [HarmonyPrefix]
        [HarmonyArgument("question", "question3")]
        public static void Prefix2(string question, string __0, [HarmonyArgument("question")] string question1,
            [HarmonyArgument(0)] string question2, string question3, string question4, SimpleClass __instance, int __result, 
            out Stopwatch __state, int ____answer, string ___BaseField, int ___0, object[] __args, MethodBase __originalMethod, 
            bool __runOriginal)
        {
            __state = Stopwatch.StartNew();
        }

        [HarmonyPrefix]
        public static void Prefix3(ref int ___0, ref string ___BaseField, ref int ____answer, ref Stopwatch __state, ref int __result,
            [HarmonyArgument(0)] ref string question2, [HarmonyArgument("question")] ref string question1,
            ref string __0, ref string question) { }

        [HarmonyPrefix]
        public static void Prefix4(object question, object __0, object __instance, object __result, object ___0, object ____answer, 
            object ___BaseField, IEnumerable<object> __args, object __originalMethod) { }

#nullable enable
        [HarmonyPrefix]
        public static void Prefix5(string question, string __0, SimpleClass __instance, Stopwatch __state, string ___BaseField, 
            object?[] __args, MethodBase __originalMethod) { }

        [HarmonyPrefix]
        public static void Prefix6(string? question, string? __0, SimpleClass? __instance, Stopwatch? __state, string? ___BaseField, 
            object?[]? __args, MethodBase? __originalMethod) { }
#nullable restore

        [HarmonyPostfix]
        public static void Postfix1() { }

        [HarmonyPostfix]
        [HarmonyArgument("question", "question3")]
        public static void Postfix2(string question, string __0, [HarmonyArgument("question")] string question1,
            [HarmonyArgument(0)] string question2, string question3, string question4, SimpleClass __instance, int __result,
            Stopwatch __state, int ____answer, string ___BaseField, int ___0, object[] __args, MethodBase __originalMethod, 
            bool __runOriginal) { }

        [HarmonyPostfix]
        public static void Postfix3(ref int ___0, string ___BaseField, ref int ____answer, ref Stopwatch __state, ref int __result,
            [HarmonyArgument(0)] ref string question2, [HarmonyArgument("question")] ref string question1,
            ref string __0, ref string question) { }

        [HarmonyPostfix]
        public static void Postfix4(object question, object __0, object __instance, object __result, object ___0, object ____answer, 
            object ___BaseField, IEnumerable<object> __args, object __originalMethod) { }

#nullable enable
        [HarmonyPrefix]
        public static void Postfix5(string question, string __0, SimpleClass __instance, Stopwatch __state, string ___BaseField,
            object?[] __args, MethodBase __originalMethod) { }

        [HarmonyPrefix]
        public static void Postfix6(string? question, string? __0, SimpleClass? __instance, Stopwatch? __state, string? ___BaseField,
            object?[]? __args, MethodBase? __originalMethod) { }
#nullable restore

        [HarmonyPostfix]
        public static int Postfix7(int result, string question, SimpleClass __instance) => result;
    }

    [HarmonyPatch(typeof(SimpleClass2)), HarmonyPatchAll]
    internal class ValidPrefixPostfixParameters2
    {
        [HarmonyPrefix]
        public static void Prefix1(SimpleClass2 __instance, int __result, ref Stopwatch __state, int ____answer, int ___0, object[] __args, 
            MethodBase __originalMethod, bool __runOriginal) { }

        [HarmonyPostfix]
        public static void Postfix1(SimpleClass2 __instance, int __result, Stopwatch __state, int ____answer, int ___0, object[] __args,
            MethodBase __originalMethod, bool __runOriginal) { }

#nullable enable
        [HarmonyPrefix]
        public static void Prefix2(SimpleClass2? __instance) { }

        [HarmonyPostfix]
        public static void Postfix2(SimpleClass2? __instance) { }
#nullable restore
    }

    [HarmonyPatch(typeof(ClassWithNullables))]
    internal class ValidPostfixNullableParameters
    {
#nullable enable
        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.NonNullable))]
        public static void Postfix1(string s, string __result) { }

        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.NonNullable))]
        public static void Postfix2(string? s, string? __result) { }

        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.NonNullable))]
        public static void Postfix3(ref string __result) { }

        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.Nullable))]
        public static void Postfix4(string? s, string? __result) { }

        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.Nullable))]
        public static void Postfix5(ref string? __result) { }
#nullable restore
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidTranspilerParameters
    {

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler1(IEnumerable<CodeInstruction> instructions) => instructions;

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator, MethodBase original) => instructions;

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler3(ILGenerator generator1, MethodBase original1, 
            IEnumerable<CodeInstruction> instructions1) => instructions1;

#nullable enable
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler4(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator, MethodBase original) => instructions;

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler5(IEnumerable<CodeInstruction?>? instructions,
            ILGenerator? generator, MethodBase? original) => instructions!;
#nullable restore
    }
}