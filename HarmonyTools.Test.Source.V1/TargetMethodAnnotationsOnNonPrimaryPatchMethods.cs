using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1
{
    [HarmonyPatch]
    internal class TargetMethodAnnotationsOnNonPrimaryPatchMethods
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static void Postfix() { }

        [HarmonyPrepare, HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static void MyPrepare() { }

        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static void NonPatch() { }
    }
}