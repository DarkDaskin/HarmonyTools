using System;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class SimpleMethodPatchCompound1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod), MethodType.Normal)]
    internal class SimpleMethodPatchCompound2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch("HarmonyTools.Test.PatchBase.SimpleClass", nameof(SimpleClass.SimpleMethod))]
    internal class SimpleMethodPatchCompound3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
    internal class SimpleMethodPatchSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod)), HarmonyPatch(MethodType.Normal)]
    internal class SimpleMethodPatchSeparate2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch, HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod)), HarmonyPatch(MethodType.Normal)]
    internal class SimpleMethodPatchSeparate3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    internal class OverloadedMethodPatchCompound1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), new[] { typeof(int), typeof(int) },
        new[] { ArgumentType.Normal, ArgumentType.Normal })]
    internal class OverloadedMethodPatchCompound2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))] 
    [HarmonyPatch(new[] { typeof(int), typeof(int) })]
    internal class OverloadedMethodSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
    [HarmonyPatch(new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal, ArgumentType.Normal })]
    internal class OverloadedMethodSeparate2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), new[] { typeof(int), typeof(int) }), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
    internal class OverloadedMethodSeparate3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    [HarmonyPatch(nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    internal class OverloadedMethodSeparate4
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    [HarmonyPatch(nameof(SimpleClass.OverloadedMethod), new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal, ArgumentType.Normal })]
    internal class OverloadedMethodSeparate5
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), new[] { typeof(int) }, new[] { ArgumentType.Out })]
    internal class OutMethodCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
    [HarmonyPatch(new[] { typeof(int) }, new[] { ArgumentType.Out })]
    internal class OutMethodSeparate
    {
        public static void Postfix() { }
    }


    [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor)]
    internal class ConstructorCompound1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor, new Type[0])]
    internal class ConstructorCompound2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor, new Type[0], new ArgumentType[0])]
    internal class ConstructorCompound3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor)]
    internal class ConstructorSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor), HarmonyPatch(new Type[0])]
    internal class ConstructorSeparate2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor), HarmonyPatch(new Type[0], new ArgumentType[0])]
    internal class ConstructorSeparate3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor, new Type[0])]
    internal class ConstructorSeparate4
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor, new Type[0], new ArgumentType[0])]
    internal class ConstructorSeparate5
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.StaticConstructor)]
    internal class StaticConstructorCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.StaticConstructor)]
    internal class StaticConstructorSeparate
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.ReadOnlyProp), MethodType.Getter)]
    internal class PropertyGetterCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadOnlyProp)), HarmonyPatch(MethodType.Getter)]
    internal class PropertyGetterSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadOnlyProp), MethodType.Getter)]
    internal class PropertyGetterSeparate2
    {
        public static void Postfix() { }
    }
    
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.ReadWriteProp), MethodType.Setter)]
    internal class PropertySetterCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadWriteProp)), HarmonyPatch(MethodType.Setter)]
    internal class PropertySetterSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadWriteProp), MethodType.Setter)]
    internal class PropertySetterSeparate2
    {
        public static void Postfix() { }
    }
    
    [HarmonyPatch(typeof(SimpleClass), MethodType.Getter, typeof(int))]
    internal class IndexerCompound1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Getter, new[] { typeof(double) }, new[] { ArgumentType.Normal })]
    internal class IndexerCompound2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Getter), HarmonyPatch(new[] { typeof(int) })]
    internal class IndexerSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Getter)]
    [HarmonyPatch(new[] { typeof(int) }, new[] { ArgumentType.Normal })]
    internal class IndexerSeparate2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.EnumeratorMethod), MethodType.Enumerator)]
    internal class EnumeratorCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.EnumeratorMethod)), HarmonyPatch(MethodType.Enumerator)]
    internal class EnumeratorSeparate
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.AsyncMethod), MethodType.Async)]
    internal class AsyncCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.AsyncMethod)), HarmonyPatch(MethodType.Async)]
    internal class AsyncSeparate
    {
        public static void Postfix() { }
    }
}