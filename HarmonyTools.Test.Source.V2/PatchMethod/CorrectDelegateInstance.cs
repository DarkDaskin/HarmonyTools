using System;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class CorrectDelegateInstance1
    {
        [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.DoNothing))]
        internal delegate void DoNothing();

        public static void Postfix(DoNothing @delegate) { }
    }

    [HarmonyPatch(typeof(SimpleClass2), nameof(SimpleClass2.SimpleMethod), new Type[0])]
    internal class CorrectDelegateInstance2
    {
        [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleStaticMethod))]
        internal delegate int SimpleStaticMethod(string question);

        public static void Postfix(SimpleStaticMethod @delegate) { }
    }
}