using HarmonyLib;
using HarmonyTools.Test.PatchBase;

namespace HarmonyTools.Test.Source.V2
{
    [HarmonyPatch(typeof(SimpleClass), "NonExistingMethod")]
    public class NonExistingMethod
    {
        
    }
}