using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch]
    internal class ValidPatchMethodReturnTypes1
    {
        public static void Prepare() { }

        public static void Cleanup() { }

        public static MethodBase TargetMethod() => default;

        public static void Prefix() { }

        public static void Postfix() { }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => instructions;

        public static void Finalizer() { }
    }


    [HarmonyPatch]
    internal class ValidPatchMethodReturnTypes2
    {
        public static MethodInfo TargetMethod() => default;

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidPatchMethodReturnTypes3
    {
        public static bool Prepare() => true;

        public static void Cleanup() { }

        public static IEnumerable<MethodBase> TargetMethods() => default;

        public static bool Prefix() => true;

        public static int Postfix(int result) => result;

        public static Exception Finalizer() => null;
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    internal class ValidPatchMethodReturnTypes4
    {
        [HarmonyPostfix]
        public static int Postfix1(int x) => x;

        [HarmonyPostfix]
        public static void Postfix2(int x) { }
    }


    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.GetFile))]
    internal class ValidPatchMethodReturnTypes5
    {
        [HarmonyReversePatch]
        public static FileSystemInfo ReversePatch1(SimpleClass instance) => throw new InvalidOperationException();

        [HarmonyReversePatch]
        public static object ReversePatch2(SimpleClass instance) => throw new InvalidOperationException();

        [HarmonyReversePatch]
        public static string ReversePatch3(SimpleClass instance)
        {
            _ = Transpiler(default);
            return default;

            IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => instructions;
        }

        [HarmonyReversePatch, HarmonyPatch(nameof(SimpleClass.EnumeratorMethod))]
        public static IEnumerable<object> ReversePatch4(SimpleClass instance) => throw new InvalidOperationException();
    }

    [HarmonyPatch]
    internal class ValidPatchMethodReturnTypes6
    {
        public static MethodBase[] TargetMethods() => default;
        
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(ClassWithNullables))]
    internal class ValidPatchMethodReturnTypesNullables1
    {
        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.NoAnnotation))]
        public static string Postfix1(string result) => result;

#nullable enable
        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.NonNullable))]
        public static string Postfix2(string result) => result;

        [HarmonyPostfix, HarmonyPatch(nameof(ClassWithNullables.Nullable))]
        public static string? Postfix3(string? result) => result;
#nullable restore
    }

    [HarmonyPatch]
    internal class ValidPatchMethodReturnTypesNullables2
    {
#nullable enable
        public static MethodBase TargetMethod() => default!;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => instructions;

        public static Exception Finalizer() => null!;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidPatchMethodReturnTypesNullables3
    {
#nullable enable
        public static IEnumerable<MethodBase> TargetMethods() => default!;

        public static Exception? Finalizer() => null;
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(ClassWithNullables), nameof(ClassWithNullables.NonNullable))]
    internal class ValidPatchMethodReturnTypesNullables4
    {
#nullable enable
        [HarmonyReversePatch]
        public static string ReversePatch1(string s) => throw new InvalidOperationException();

        [HarmonyReversePatch]
        public static string? ReversePatch2(string s) => throw new InvalidOperationException();
#nullable restore
    }
}