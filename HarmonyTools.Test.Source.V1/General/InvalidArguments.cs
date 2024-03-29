﻿using System;
using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1.General
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

    [HarmonyPatch((Type)null), HarmonyPatch((string)null, (MethodType)5)]
    internal class InvalidArguments5
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    [HarmonyPatch(nameof(SimpleClass.SimpleMethod), (PropertyMethod)100)]
    [Obsolete("Testing obsolete constructor")]
    internal class InvalidArguments6
    {
        public static void Postfix() { }
    }
}