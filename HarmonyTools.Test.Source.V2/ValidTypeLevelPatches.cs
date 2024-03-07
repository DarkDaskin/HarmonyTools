using System;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal class TypeLevelSimpleMethodPatchCompound1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod), MethodType.Normal)]
    internal class TypeLevelSimpleMethodPatchCompound2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch("HarmonyTools.Test.PatchBase.SimpleClass", nameof(SimpleClass.SimpleMethod))]
    internal class TypeLevelSimpleMethodPatchCompound3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
    internal class TypeLevelSimpleMethodPatchSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod)), HarmonyPatch(MethodType.Normal)]
    internal class TypeLevelSimpleMethodPatchSeparate2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch, HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod)), HarmonyPatch(MethodType.Normal)]
    internal class TypeLevelSimpleMethodPatchSeparate3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    internal class TypeLevelOverloadedMethodPatchCompound1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), new[] { typeof(int), typeof(int) },
        new[] { ArgumentType.Normal, ArgumentType.Normal })]
    internal class TypeLevelOverloadedMethodPatchCompound2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))] 
    [HarmonyPatch(new[] { typeof(int), typeof(int) })]
    internal class TypeLevelOverloadedMethodSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
    [HarmonyPatch(new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal, ArgumentType.Normal })]
    internal class TypeLevelOverloadedMethodSeparate2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), new[] { typeof(int), typeof(int) }), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
    internal class TypeLevelOverloadedMethodSeparate3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    [HarmonyPatch(nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    internal class TypeLevelOverloadedMethodSeparate4
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass))]
    [HarmonyPatch(nameof(SimpleClass.OverloadedMethod), new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal, ArgumentType.Normal })]
    internal class TypeLevelOverloadedMethodSeparate5
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), new[] { typeof(int) }, new[] { ArgumentType.Out })]
    internal class TypeLevelOutMethodCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
    [HarmonyPatch(new[] { typeof(int) }, new[] { ArgumentType.Out })]
    internal class TypeLevelOutMethodSeparate
    {
        public static void Postfix() { }
    }


    [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor)]
    internal class TypeLevelConstructorCompound1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor, new Type[0])]
    internal class TypeLevelConstructorCompound2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor, new Type[0], new ArgumentType[0])]
    internal class TypeLevelConstructorCompound3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor)]
    internal class TypeLevelConstructorSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor), HarmonyPatch(new Type[0])]
    internal class TypeLevelConstructorSeparate2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor), HarmonyPatch(new Type[0], new ArgumentType[0])]
    internal class TypeLevelConstructorSeparate3
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor, new Type[0])]
    internal class TypeLevelConstructorSeparate4
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor, new Type[0], new ArgumentType[0])]
    internal class TypeLevelConstructorSeparate5
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.StaticConstructor)]
    internal class TypeLevelStaticConstructorCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.StaticConstructor)]
    internal class TypeLevelStaticConstructorSeparate
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.ReadOnlyProp), MethodType.Getter)]
    internal class TypeLevelPropertyGetterCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadOnlyProp)), HarmonyPatch(MethodType.Getter)]
    internal class TypeLevelPropertyGetterSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadOnlyProp), MethodType.Getter)]
    internal class TypeLevelPropertyGetterSeparate2
    {
        public static void Postfix() { }
    }
    
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.ReadWriteProp), MethodType.Setter)]
    internal class TypeLevelPropertySetterCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadWriteProp)), HarmonyPatch(MethodType.Setter)]
    internal class TypeLevelPropertySetterSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadWriteProp), MethodType.Setter)]
    internal class TypeLevelPropertySetterSeparate2
    {
        public static void Postfix() { }
    }
    
    [HarmonyPatch(typeof(SimpleClass), MethodType.Getter, typeof(int))]
    internal class TypeLevelIndexerCompound1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), MethodType.Getter, new[] { typeof(double) }, new[] { ArgumentType.Normal })]
    internal class TypeLevelIndexerCompound2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Getter), HarmonyPatch(new[] { typeof(int) })]
    internal class TypeLevelIndexerSeparate1
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Getter)]
    [HarmonyPatch(new[] { typeof(int) }, new[] { ArgumentType.Normal })]
    internal class TypeLevelIndexerSeparate2
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.EnumeratorMethod), MethodType.Enumerator)]
    internal class TypeLevelEnumeratorCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.EnumeratorMethod)), HarmonyPatch(MethodType.Enumerator)]
    internal class TypeLevelEnumeratorSeparate
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.AsyncMethod), MethodType.Async)]
    internal class TypeLevelAsyncCompound
    {
        public static void Postfix() { }
    }

    [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.AsyncMethod)), HarmonyPatch(MethodType.Async)]
    internal class TypeLevelAsyncSeparate
    {
        public static void Postfix() { }
    }
}