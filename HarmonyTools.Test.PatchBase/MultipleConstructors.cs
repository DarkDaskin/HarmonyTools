namespace HarmonyTools.Test.PatchBase
{
    public class MultipleConstructors
    {
        public int Value { get; }

        public MultipleConstructors() { }

        public MultipleConstructors(int value)
        {
            Value = value;
        }
    }
}