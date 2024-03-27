using System.Collections.Generic;
using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1.PatchMethod
{
    [HarmonyPatch(typeof(SimpleClass), nameof(SimpleClass.SimpleMethod))]
    [HarmonyArgument("question", "question1")]
    internal class OrphanArguments
    {
        public static void Prefix() { }

        [HarmonyArgument("question", "question2")]
        public static void Postfix() { }

        [HarmonyArgument("question", "question2")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => instructions;
    }
}