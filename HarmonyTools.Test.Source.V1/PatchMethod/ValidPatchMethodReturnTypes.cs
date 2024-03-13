using System.Collections.Generic;
using System.Reflection;
using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1.PatchMethod
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
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    internal class ValidPatchMethodReturnTypes4
    {
        [HarmonyPostfix]
        public static int Postfix1(int x) => x;

        [HarmonyPostfix]
        public static void Postfix2(int x) { }
    }

    [HarmonyPatch]
    internal class ValidPatchMethodReturnTypes5
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
#nullable restore

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class ValidPatchMethodReturnTypesNullables3
    {
#nullable enable
        public static IEnumerable<MethodBase> TargetMethods() => default!;
#nullable restore

        public static void Postfix() { }
    }
}