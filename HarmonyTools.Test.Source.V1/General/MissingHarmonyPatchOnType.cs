﻿using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1.General
{
    internal class MissingHarmonyPatchOnType
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
        public static void Prefix() { }

        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
        public static void Postfix() { }
    }
}