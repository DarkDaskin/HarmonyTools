using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2.PatchMethod
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.OverloadedMethod), typeof(int), typeof(int))]
    [HarmonyArgument(0)]
    internal class MissingArgumentNewName
    {
        [HarmonyArgument("x")]
        public static void Postfix(int x) { }
    }
}