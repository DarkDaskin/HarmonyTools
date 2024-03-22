namespace HarmonyTools.Test.PatchBase
{
    public static class ClassWithNullables
    {
        public static string NoAnnotation(string s) => s;
#nullable enable
        public static string? Nullable(string? s) => s;
        public static string NonNullable(string s) => s;
#nullable restore
    }
}