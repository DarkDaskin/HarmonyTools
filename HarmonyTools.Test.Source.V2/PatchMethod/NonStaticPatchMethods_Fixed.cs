﻿using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch]
    internal class NonStaticPatchMethods
    {
        public static void Prefix() { }

        public static void Postfix() { }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => instructions;

        public static void Prepare() { }

        public static void Cleanup() { }

        public static MethodBase TargetMethod() => default;

        public static void Finalizer() { }

        public static void ReversePatch() { }

        public void NonPatchMethod() { }
    }
}