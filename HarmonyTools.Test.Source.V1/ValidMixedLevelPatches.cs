﻿using System;
using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1
{
    [HarmonyPatch(typeof(SimpleClass))]
    internal class MixedLevelSimpleMethodPatch1
    {
        [HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
    internal class MixedLevelSimpleMethodPatch2
    {
        [HarmonyPatch(MethodType.Normal)]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
    internal class MixedLevelOverloadedMethod1
    {
        [HarmonyPatch(new[] { typeof(int), typeof(int) })]
        public static void Postfix() { }
    }

    [HarmonyPatch(new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal, ArgumentType.Normal })]
    internal class MixedLevelOverloadedMethod2
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), new[] { typeof(int), typeof(int) })]
    internal class MixedLevelOverloadedMethod3
    {
        [HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
        public static void Postfix() { }
    }

    [HarmonyPatch(nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    internal class MixedLevelOverloadedMethod4
    {
        [HarmonyPatch(typeof(SimpleClass))]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    internal class MixedLevelOverloadedMethod5
    {
        [HarmonyPatch(nameof(SimpleClass.OverloadedMethod), new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal, ArgumentType.Normal })]
        public static void Postfix() { }
    }

    [HarmonyPatch(new[] { typeof(int) }, new[] { ArgumentType.Out })]
    internal class MixedLevelOutMethod
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
        public static void Postfix() { }
    }
    
    [HarmonyPatch(typeof(SimpleClass))]
    internal class MixedLevelConstructor1
    {
        [HarmonyPatch(MethodType.Constructor)]
        public static void Postfix() { }
    }

    [HarmonyPatch(MethodType.Constructor)]
    internal class MixedLevelConstructor2
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(new Type[0])]
        public static void Postfix() { }
    }

    [HarmonyPatch(new Type[0], new ArgumentType[0])]
    internal class MixedLevelConstructor3
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor)]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    internal class MixedLevelConstructor4
    {
        [HarmonyPatch(MethodType.Constructor, new Type[0])]
        public static void Postfix() { }
    }

    [HarmonyPatch(MethodType.Constructor, new Type[0], new ArgumentType[0])]
    internal class MixedLevelConstructor5
    {
        [HarmonyPatch(typeof(SimpleClass))]
        public static void Postfix() { }
    }
    
    [HarmonyPatch(MethodType.StaticConstructor)]
    internal class MixedLevelStaticConstructor
    {
        [HarmonyPatch(typeof(SimpleClass))]
        public static void Postfix() { }
    }
    
    [HarmonyPatch(MethodType.Getter)]
    internal class MixedLevelPropertyGetter1
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadOnlyProp))]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    internal class MixedLevelPropertyGetter2
    {
        [HarmonyPatch(nameof(SimpleClass.ReadOnlyProp), MethodType.Getter)]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    [Obsolete("Testing obsolete constructor")]
    internal class MixedLevelPropertyGetterObsolete
    {
        [HarmonyPatch(nameof(SimpleClass.ReadOnlyProp), PropertyMethod.Getter)]
        public static void Postfix() { }
    }
    
    [HarmonyPatch(nameof(SimpleClass.ReadWriteProp))]
    internal class MixedLevelPropertySetter1
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Setter)]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    internal class MixedLevelPropertySetter2
    {
        [HarmonyPatch(nameof(SimpleClass.ReadWriteProp), MethodType.Setter)]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    [Obsolete("Testing obsolete constructor")]
    internal class MixedLevelPropertySetterObsolete
    {
        [HarmonyPatch(nameof(SimpleClass.ReadWriteProp), PropertyMethod.Setter)]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(new[] { typeof(int) })]
    internal class MixedLevelIndexer1
    {
        [HarmonyPatch(MethodType.Getter)]
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Getter)]
    internal class MixedLevelIndexer2
    {
        [HarmonyPatch(new[] { typeof(int) }, new[] { ArgumentType.Normal })]
        public static void Postfix() { }
    }
}