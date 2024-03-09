﻿using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class MultiplePatchMethodKinds
    {
        [HarmonyPostfix]
        public static void Prefix() { }

        [HarmonyPrepare, HarmonyCleanup, HarmonyPrefix]
        public static void InvalidMethod() { }

        [HarmonyPrefix]
        public static void ValidMethod() { }
    }
}