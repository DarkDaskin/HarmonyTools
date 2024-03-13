namespace HarmonyTools.Test.PatchBase
{
    public static class ClassWithNullables
    {
        public static string NoAnnotation() => "";
#nullable enable
        public static string? Nullable() => "";
        public static string NonNullable() => "";
#nullable restore
    }
}