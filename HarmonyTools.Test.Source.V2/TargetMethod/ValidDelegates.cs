using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.TargetMethod
{
    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    internal delegate int ValidDelegate1(string question);

    [HarmonyDelegate(typeof(SimpleClass), MethodDispatchType.Call), HarmonyDelegate(nameof(SimpleClass.SimpleMethod))]
    internal delegate int ValidDelegate2(string question);

    [HarmonyDelegate(typeof(SimpleClass), MethodDispatchType.Call, typeof(string))]
    [HarmonyDelegate(nameof(SimpleClass.SimpleMethod))]
    internal delegate int ValidDelegate3(string question);

    [HarmonyDelegate(typeof(SimpleClass), MethodDispatchType.Call, new[] { typeof(string) }, new[] { ArgumentType.Normal })]
    [HarmonyDelegate(nameof(SimpleClass.SimpleMethod))]
    internal delegate int ValidDelegate4(string question);

    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod), MethodDispatchType.Call)]
    internal delegate int ValidDelegate5(string question);

    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod)), HarmonyDelegate(MethodDispatchType.Call)]
    internal delegate int ValidDelegate6(string question);

    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyDelegate(MethodDispatchType.Call, typeof(string))]
    internal delegate int ValidDelegate7(string question);

    [HarmonyDelegate(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyDelegate(MethodDispatchType.Call, new[] { typeof(string) }, new[] { ArgumentType.Normal })]
    internal delegate int ValidDelegate8(string question);
}