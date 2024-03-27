using System;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.DoNothing))]
    internal delegate void DoNothing();

    [HarmonyPatch(typeof(SimpleClass2), nameof(SimpleClass2.SimpleMethod), new Type[0])]
    internal class IncorrectDelegateInstance1
    {
        public static void Postfix(DoNothing @delegate) { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleStaticMethod))]
    internal class IncorrectDelegateInstance2
    {
        public static void Postfix(DoNothing @delegate) { }
    }
}