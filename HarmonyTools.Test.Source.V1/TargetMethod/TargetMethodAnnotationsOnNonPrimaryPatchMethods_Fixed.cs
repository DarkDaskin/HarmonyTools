﻿using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1.TargetMethod
{
    [HarmonyPatch]
    internal class TargetMethodAnnotationsOnNonPrimaryPatchMethods
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static void Postfix() { }

        [HarmonyPrepare]
        public static void MyPrepare() { }

        public static void NonPatch() { }
    }
}