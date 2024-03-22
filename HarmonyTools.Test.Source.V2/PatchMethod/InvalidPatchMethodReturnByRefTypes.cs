using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch]
    internal class InvalidPatchMethodReturnByRefTypes1
    {
        private static bool _bool;
        private static Exception _exception;
        private static MethodBase _methodBase;
        private static IEnumerable<CodeInstruction> _codeInstructions;

        public static ref bool Prepare() => ref _bool;

        public static ref Exception Cleanup() => ref _exception;

        public static ref MethodBase TargetMethod() => ref _methodBase;

        public static ref IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => ref _codeInstructions;
    }

    [HarmonyPatch]
    internal class InvalidPatchMethodReturnByRefTypes2
    {
        private static IEnumerable<MethodBase> _methodBases;

        public static ref IEnumerable<MethodBase> TargetMethods() => ref _methodBases;

        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class InvalidPatchMethodReturnByRefTypes3
    {
        private static bool _bool;
        private static int _int;

        public static ref bool Prefix() => ref _bool;

        public static ref int Postfix(int result) => ref _int;
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class InvalidPatchMethodReturnByRefTypes4
    {
        private static Exception _exception;
        private static int _int;

        public static ref Exception Finalizer() => ref _exception;

        public static ref int ReversePatch(SimpleClass instance, string question) => ref _int;

        [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
        public delegate ref int SimpleMethod(string question);
    }
}