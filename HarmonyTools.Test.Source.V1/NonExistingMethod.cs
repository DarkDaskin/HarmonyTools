using Harmony;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V1
{
    [HarmonyPatch(typeof(SimpleClass), "NonExistingMethod")]
    public class NonExistingMethod
    {
        
    }
}