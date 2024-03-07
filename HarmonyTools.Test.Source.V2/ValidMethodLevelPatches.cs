using System;
using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch]
    internal class MethodLevelSimpleMethodPatchCompound1
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelSimpleMethodPatchCompound2
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod), MethodType.Normal)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelSimpleMethodPatchCompound3
    {
        [HarmonyPatch("HarmonyTools.Test.PatchBase.SimpleClass", nameof(SimpleClass.SimpleMethod))]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelSimpleMethodPatchSeparate1
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod))]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelSimpleMethodPatchSeparate2
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod)), HarmonyPatch(MethodType.Normal)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelSimpleMethodPatchSeparate3
    {
        [HarmonyPatch, HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.SimpleMethod)), HarmonyPatch(MethodType.Normal)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelOverloadedMethodPatchCompound1
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelOverloadedMethodPatchCompound2
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), new[] { typeof(int), typeof(int) },
            new[] { ArgumentType.Normal, ArgumentType.Normal })]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelOverloadedMethodSeparate1
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
        [HarmonyPatch(new[] { typeof(int), typeof(int) })]

        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelOverloadedMethodSeparate2
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
        [HarmonyPatch(new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal, ArgumentType.Normal })]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelOverloadedMethodSeparate3
    {
        [HarmonyPatch(typeof(SimpleClass), new[] { typeof(int), typeof(int) }), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelOverloadedMethodSeparate4
    {
        [HarmonyPatch(typeof(SimpleClass))]
        [HarmonyPatch(nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelOverloadedMethodSeparate5
    {
        [HarmonyPatch(typeof(SimpleClass))]
        [HarmonyPatch(nameof(SimpleClass.OverloadedMethod), new[] { typeof(int), typeof(int) }, new[] { ArgumentType.Normal, ArgumentType.Normal })]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelOutMethodCompound
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), new[] { typeof(int) }, new[] { ArgumentType.Out })]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelOutMethodSeparate
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.OverloadedMethod))]
        [HarmonyPatch(new[] { typeof(int) }, new[] { ArgumentType.Out })]
        public static void Postfix() { }
    }
    
    [HarmonyPatch]
    internal class MethodLevelConstructorCompound1
    {
        [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelConstructorCompound2
    {
        [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor, new Type[0])]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelConstructorCompound3
    {
        [HarmonyPatch(typeof(SimpleClass), MethodType.Constructor, new Type[0], new ArgumentType[0])]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelConstructorSeparate1
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelConstructorSeparate2
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor), HarmonyPatch(new Type[0])]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelConstructorSeparate3
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor), HarmonyPatch(new Type[0], new ArgumentType[0])]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelConstructorSeparate4
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor, new Type[0])]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelConstructorSeparate5
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Constructor, new Type[0], new ArgumentType[0])]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelStaticConstructorCompound
    {
        [HarmonyPatch(typeof(SimpleClass), MethodType.StaticConstructor)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelStaticConstructorSeparate
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.StaticConstructor)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelPropertyGetterCompound
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.ReadOnlyProp), MethodType.Getter)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelPropertyGetterSeparate1
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadOnlyProp)), HarmonyPatch(MethodType.Getter)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelPropertyGetterSeparate2
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadOnlyProp), MethodType.Getter)]
        public static void Postfix() { }
    }
    
    [HarmonyPatch]
    internal class MethodLevelPropertySetterCompound
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.ReadWriteProp), MethodType.Setter)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelPropertySetterSeparate1
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadWriteProp)), HarmonyPatch(MethodType.Setter)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelPropertySetterSeparate2
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.ReadWriteProp), MethodType.Setter)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelIndexerCompound1
    {
        [HarmonyPatch(typeof(SimpleClass), MethodType.Getter, typeof(int))]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelIndexerCompound2
    {
        [HarmonyPatch(typeof(SimpleClass), MethodType.Getter, new[] { typeof(double) }, new[] { ArgumentType.Normal })]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelIndexerSeparate1
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Getter), HarmonyPatch(new[] { typeof(int) })]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelIndexerSeparate2
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(MethodType.Getter)]
        [HarmonyPatch(new[] { typeof(int) }, new[] { ArgumentType.Normal })]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelEnumeratorCompound
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.EnumeratorMethod), MethodType.Enumerator)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelEnumeratorSeparate
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.EnumeratorMethod)), HarmonyPatch(MethodType.Enumerator)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelAsyncCompound
    {
        [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.AsyncMethod), MethodType.Async)]
        public static void Postfix() { }
    }

    [HarmonyPatch]
    internal class MethodLevelAsyncSeparate
    {
        [HarmonyPatch(typeof(SimpleClass)), HarmonyPatch(nameof(SimpleClass.AsyncMethod)), HarmonyPatch(MethodType.Async)]
        public static void Postfix() { }
    }
}