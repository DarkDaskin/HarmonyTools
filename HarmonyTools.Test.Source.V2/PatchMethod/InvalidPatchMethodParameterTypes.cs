﻿using System;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;
// ReSharper disable InconsistentNaming

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyArgument("question", "question3")]
    public class InvalidPrefixPostfixParameterTypes
    {
        [HarmonyArgument(0, "question4")]
        public static void Prefix(int question, int __0, [HarmonyArgument("question")] int question1,
            [HarmonyArgument(0)] int question2, int question3, int question4, SimpleSubClass __instance, ValueType __result,
            out Stopwatch __state, string ____answer, IConvertible ___0, string[] __args, MethodInfo __originalMethod, object __runOriginal)
        {
            __state = Stopwatch.StartNew();
        }

        [HarmonyArgument(0, "question4")]
        public static void Postfix(int question, int __0, [HarmonyArgument("question")] int question1,
            [HarmonyArgument(0)] int question2, int question3, int question4, SimpleSubClass __instance, ValueType __result,
            string __state, string ____answer, IConvertible ___0, string[] __args, MethodInfo __originalMethod, object __runOriginal) { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    public class UnassignedState
    {
        public static void Prefix(Stopwatch __state) { }

        public static void Postfix(Stopwatch __state) { }
    }

    [HarmonyPatch(typeof(ClassWithNullables))]
    internal class InvalidPostfixNullableParameterTypes1
    {
#nullable enable

        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.NonNullable))]
        public static void Postfix1(ref string? __result) { }

        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.Nullable))]
        public static void Postfix2(string s, string __result, object[] __args) { }

        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.Nullable))]
        public static void Postfix3(ref string __result) { }
#nullable restore
    }

    [HarmonyPatch(typeof(SimpleClass2)), HarmonyPatchAll]
    internal class ValidPrefixPostfixNullableParameterTypes2
    {
#nullable enable
        public static void Postfix(SimpleClass2 __instance) { }
#nullable restore
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyArgument("question", "question3")]
    public class InvalidFinalizerParameterTypes
    {
        [HarmonyArgument(0, "question4")]
        public static void Finalizer(int question, int __0, [HarmonyArgument("question")] int question1,
            [HarmonyArgument(0)] int question2, int question3, int question4, SimpleSubClass __instance, ValueType __result,
            string __state, string ____answer, IConvertible ___0, string[] __args, MethodInfo __originalMethod, object __runOriginal,
            ArgumentException __exception) { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class InvalidReversePatchParameterTypes
    {
#nullable enable
        public static int ReversePatch(SimpleClass? instance, object question) => default;
#nullable restore
    }

    [HarmonyPatch(typeof(ClassWithNullables), nameof(ClassWithNullables.NonNullable))]
    internal class InvalidReversePatchNullableParameterTypes
    {
#nullable enable
        public static string ReversePatch(string? s) => default!;
#nullable restore
    }

#nullable enable
    [HarmonyDelegate(typeof(ClassWithNullables), nameof(ClassWithNullables.NonNullable))]
    internal delegate string InvalidNullableDelegateParameterTypes(string? s);
#nullable restore
}