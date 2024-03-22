using HarmonyTools.Test.PatchBase;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using System.IO;
using System;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch]
    internal class InvalidPatchMethodReturnTypes1
    {
        public static object Prepare() => default;

        public static object Cleanup() => default;

        public static object TargetMethod() => default;

        public static object Prefix() => default;

        public static object Postfix(string result) => default;

        public static IEnumerable<int> Transpiler(IEnumerable<CodeInstruction> instructions) => default;
    }

    [HarmonyPatch]
    internal class InvalidPatchMethodReturnTypes2
    {
        public static IEnumerable<int> TargetMethods() => default;
        
        public static void Postfix() { }

        public static void Transpiler(IEnumerable<CodeInstruction> instructions) { }
    }

    [HarmonyPatch(typeof(ClassWithNullables))]
    internal class InvalidPatchMethodReturnTypesNullables1
    {
#nullable enable
        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.NonNullable))]
        public static string? Postfix1(string? result) => result;

        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.Nullable))]
        public static string Postfix2(string? result) => result!;
#nullable restore
    }

    [HarmonyPatch]
    internal class InvalidPatchMethodReturnTypesNullables2
    {
#nullable enable
        public static MethodBase? TargetMethod() => default;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class InvalidPatchMethodReturnTypesNullables3
    {
#nullable enable
        public static IEnumerable<MethodBase>? TargetMethods() => default;

        public static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction>? instructions) => instructions;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class InvalidPatchMethodReturnTypesNullables4
    {
#nullable enable
        public static IEnumerable<MethodBase?> TargetMethods() => default!;

        public static IEnumerable<CodeInstruction?> Transpiler(IEnumerable<CodeInstruction?> instructions) => instructions;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class InvalidPatchMethodReturnTypes3
    {
        public static object Finalizer() => default;
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.GetFile))]
    internal class InvalidPatchMethodReturnTypes4
    {
        public static FileInfo ReversePatch(SimpleClass instance) => throw new InvalidOperationException();
    }

    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal delegate string InvalidDelegateReturnType(string question);

#nullable enable
    [HarmonyDelegate(typeof(ClassWithNullables), nameof(ClassWithNullables.Nullable))]
    internal delegate string InvalidDelegateNullableReturnType(string? s);
#nullable restore
}