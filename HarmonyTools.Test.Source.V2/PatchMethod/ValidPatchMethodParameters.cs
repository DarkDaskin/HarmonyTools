using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;
// ReSharper disable InconsistentNaming

namespace HarmonyTools.Test.Source.V2.PatchMethod
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
        public static void Prepare(MethodBase original, Harmony harmony) { }

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidPrepareParameters3
    {
        public static void Prepare(Harmony harmony1, MethodBase original1) { }

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidPrepareParameters4
    {
#nullable enable
        public static void Prepare(MethodBase? original, Harmony harmony) { }
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidPrepareParameters5
    {
#nullable enable
        public static void Prepare(MethodBase? original, Harmony? harmony) { }
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
        public static void Cleanup(MethodBase original, Harmony harmony, Exception exception) { }

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidCleanupParameters3
    {
        public static void Cleanup(Harmony harmony1, Exception exception1, MethodBase original1) { }

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidCleanupParameters4
    {
#nullable enable
        public static void Cleanup(MethodBase? original, Harmony harmony, Exception exception) { }
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class ValidCleanupParameters5
    {
#nullable enable
        public static void Cleanup(MethodBase? original, Harmony? harmony, Exception? exception) { }
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
        public static MethodBase TargetMethod(Harmony harmony) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodParameters3
    {
        public static MethodBase TargetMethod(Harmony harmony1) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodParameters4
    {
#nullable enable
        public static MethodBase TargetMethod(Harmony harmony) => default!;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodParameters5
    {
#nullable enable
        public static MethodBase TargetMethod(Harmony? harmony) => default!;
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
        public static IEnumerable<MethodBase> TargetMethods(Harmony harmony) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodsParameters3
    {
        public static IEnumerable<MethodBase> TargetMethods(Harmony harmony1) => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodsParameters4
    {
#nullable enable
        public static IEnumerable<MethodBase> TargetMethods(Harmony harmony) => default!;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidTargetMethodsParameters5
    {
#nullable enable
        public static IEnumerable<MethodBase> TargetMethods(Harmony? harmony) => default!;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyArgument(0, "question4")]
    internal class ValidPrefixPostfixFinalizerParameters1
    {
        [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
        public delegate int OverloadedMethod(int x, int y);

        [HarmonyPrefix]
        public static void Prefix1() { }

        [HarmonyPrefix]
        [HarmonyArgument("question", "question3")]
        public static void Prefix2(string question, string __0, [HarmonyArgument("question")] string question1, 
            [HarmonyArgument(0)] string question2, string question3, string question4, OverloadedMethod @delegate, 
            SimpleClass __instance, int __result, out Stopwatch __state, int ____answer, string ___BaseField, int ___0, 
            object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            __state = Stopwatch.StartNew();
        }

        [HarmonyPrefix]
        public static void Prefix3(ref int ___0, ref string ___BaseField, ref int ____answer, ref Stopwatch __state, 
            ref int __result, [HarmonyArgument(0)] ref string question2, 
            [HarmonyArgument("question")] ref string question1, ref string __0, ref string question) { }

        [HarmonyPrefix]
        public static void Prefix4(object question, object __0, object __instance, object __result, object ____answer,
            object ___BaseField, object ___0, IEnumerable<object> __args, object __originalMethod) { }

#nullable enable
        [HarmonyPrefix]
        public static void Prefix5(string question, string __0, SimpleClass __instance, Stopwatch __state, string ___BaseField,
            object?[] __args, MethodBase __originalMethod, OverloadedMethod @delegate) { }

        [HarmonyPrefix]
        public static void Prefix6(string? question, string? __0, SimpleClass? __instance, Stopwatch? __state, string? ___BaseField,
            object?[]? __args, MethodBase? __originalMethod, OverloadedMethod? @delegate) { }
#nullable restore

        [HarmonyPostfix]
        public static void Postfix1() { }

        [HarmonyPostfix]
        [HarmonyArgument("question", "question3")]
        public static void Postfix2(string question, string __0, [HarmonyArgument("question")] string question1,
            [HarmonyArgument(0)] string question2, string question3, string question4, OverloadedMethod @delegate, 
            SimpleClass __instance, int __result, Stopwatch __state, int ____answer, string ___BaseField, int ___0, object[] __args, 
            MethodBase __originalMethod, bool __runOriginal) { }

        [HarmonyPostfix]
        public static void Postfix3(ref int ___0, ref string ___BaseField, ref int ____answer, ref Stopwatch __state, ref int __result,
            [HarmonyArgument(0)] ref string question2, [HarmonyArgument("question")] ref string question1,
            ref string __0, ref string question) { }

        [HarmonyPostfix]
        public static void Postfix4(object question, object __0, object __instance, object __result, object ___0, object ____answer, 
            string ___BaseField, IEnumerable<object> __args, object __originalMethod) { }

#nullable enable
        [HarmonyPrefix]
        public static void Postfix5(string question, string __0, SimpleClass __instance, Stopwatch __state, string ___BaseField,
            object?[] __args, MethodBase __originalMethod, OverloadedMethod @delegate) { }

        [HarmonyPrefix]
        public static void Postfix6(string? question, string? __0, SimpleClass? __instance, Stopwatch? __state, string? ___BaseField,
            object?[]? __args, MethodBase? __originalMethod, OverloadedMethod? @delegate) { }
#nullable restore

        [HarmonyPostfix]
        public static int Postfix7(int result, string question, SimpleClass __instance) => result;

        [HarmonyFinalizer]
        public static void Finalizer1() { }

        [HarmonyFinalizer]
        [HarmonyArgument("question", "question3")]
        public static void Finalizer2(string question, string __0, [HarmonyArgument("question")] string question1,
            [HarmonyArgument(0)] string question2, string question3, string question4, OverloadedMethod @delegate, 
            SimpleClass __instance, int __result, Stopwatch __state, int ____answer, string ___BaseField, int ___0, object[] __args, 
            MethodBase __originalMethod, bool __runOriginal, Exception __exception) { }

        [HarmonyFinalizer]
        public static void Finalizer3(ref int ___0, ref string ___BaseField, ref int ____answer, ref Stopwatch __state, ref int __result,
            [HarmonyArgument(0)] ref string question2, [HarmonyArgument("question")] ref string question1,
            ref string __0, ref string question) { }

        [HarmonyFinalizer]
        public static void Finalizer4(object question, object __0, object __instance, object __result, object ___0, object ____answer, 
            object ___BaseField, IEnumerable<object> __args, object __originalMethod, Exception __exception) { }

#nullable enable
        [HarmonyFinalizer]
        public static void Finalizer5(string question, string __0, SimpleClass __instance, Stopwatch __state, string ___BaseField,
            object?[] __args, MethodBase __originalMethod, OverloadedMethod @delegate) { }

        [HarmonyFinalizer]
        public static void Finalizer6(string? question, string? __0, SimpleClass? __instance, Stopwatch? __state, string? ___BaseField,
            object?[]? __args, MethodBase? __originalMethod, OverloadedMethod? @delegate) { }
#nullable restore
    }

    [HarmonyPatch(typeof(SimpleClass2)), HarmonyPatchAll]
    internal class ValidPrefixPostfixFinalizerParameters2
    {
        [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
        public delegate int OverloadedMethod(int x, int y);

        [HarmonyPrefix]
        public static void Prefix1(SimpleClass2 __instance, int __result, ref Stopwatch __state, int ____answer, int ___0, object[] __args,
            MethodBase __originalMethod, bool __runOriginal, OverloadedMethod @delegate) { }

        [HarmonyPostfix]
        public static void Postfix1(SimpleClass2 __instance, int __result, Stopwatch __state, int ____answer, int ___0, object[] __args,
            MethodBase __originalMethod, bool __runOriginal, OverloadedMethod @delegate) { }

        [HarmonyFinalizer]
        public static void Finalizer1(SimpleClass2 __instance, int __result, Stopwatch __state, int ____answer, int ___0, object[] __args,
            MethodBase __originalMethod, bool __runOriginal, Exception __exception, OverloadedMethod @delegate) { }

#nullable enable
        [HarmonyPrefix]
        public static void Prefix2(SimpleClass2? __instance) { }

        [HarmonyPostfix]
        public static void Postfix2(SimpleClass2? __instance) { }

        [HarmonyFinalizer]
        public static void Finalizer2(SimpleClass2? __instance) { }
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

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.GetAnswerRef))]
    internal class ValidPrefixPostfixFinalizerParameters3
    {
        public static void Prefix(ref RefResult<int> __resultRef) { }

        public static void Postfix(ref RefResult<int> __resultRef) { }

        public static void Finalizer(ref RefResult<int> __resultRef) { }
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

    [HarmonyPatch(typeof(SimpleClass))]
    internal class ValidReversePatchParameters
    {
        [HarmonyReversePatch, HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static int ReversePatch1(SimpleClass instance, string question1) => default;

        [HarmonyReversePatch, HarmonyPatch(nameof(SimpleClass.SimpleStaticMethod))]
        public static int ReversePatch2(string question1) => default;

        [HarmonyReversePatch, HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static int ReversePatch3(SimpleSubClass instance, string question1) => default;

#nullable enable
        [HarmonyReversePatch, HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static int ReversePatch4(SimpleClass instance, string question1) => default;

        [HarmonyReversePatch, HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static int ReversePatch5(SimpleClass instance, string? question1) => default;
#nullable restore
    }

    [HarmonyPatch(typeof(ClassWithNullables))]
    internal class ValidReversePatchNullableParameters
    {
#nullable enable
        [HarmonyReversePatch, HarmonyPatch(nameof(ClassWithNullables.NonNullable))]
        public static string ReversePatch1(string s) => default!;

        [HarmonyReversePatch, HarmonyPatch(nameof(ClassWithNullables.Nullable))]
        public static string? ReversePatch2(string s) => default;

        [HarmonyReversePatch, HarmonyPatch(nameof(ClassWithNullables.Nullable))]
        public static string? ReversePatch3(string? s) => default;
#nullable restore
    }

    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal delegate int ValidDelegate(string question1);

#nullable enable
    [HarmonyDelegate(typeof(ClassWithNullables), nameof(ClassWithNullables.NonNullable))]
    internal delegate string ValidNullableDelegate1(string s);

    [HarmonyDelegate(typeof(ClassWithNullables), nameof(ClassWithNullables.Nullable))]
    internal delegate string? ValidNullableDelegate2(string s);

    [HarmonyDelegate(typeof(ClassWithNullables), nameof(ClassWithNullables.Nullable))]
    internal delegate string? ValidNullableDelegate3(string? s);
#nullable restore
}