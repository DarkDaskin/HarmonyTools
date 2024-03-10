using System;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch((Type)null, (string)null, (Type[])null, (ArgumentType[])null)]
    internal class InvalidArguments1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyPatch(new Type[] { null }, new ArgumentType[] { (ArgumentType)100 })]
    internal class InvalidArguments2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class InvalidArguments3
    {
        [HarmonyPatch((Type)null, (string)null)]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), "")]
    internal class InvalidArguments4
    {
        public static void Postfix() { }
    }

    [HarmonyPatch((Type)null), HarmonyPatch((string)null, (MethodType)100)]
    internal class InvalidArguments5
    {
        public static void Postfix() { }
    }

    [HarmonyPatch((string)null, (string)null)]
    internal class InvalidArguments6
    {
        public static void Postfix() { }
    }

    [HarmonyPatch("", "")]
    internal class InvalidArguments7
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod)), HarmonyPatchCategory(null)]
    internal class InvalidArguments8
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod)), HarmonyPatchCategory("")]
    internal class InvalidArguments9
    {
        public static void Postfix() { }
    }

    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod), (MethodDispatchType)3)]
    internal delegate int InvalidArguments10(string question);
}