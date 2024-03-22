namespace HarmonyTools.Test.PatchBase
{
    public class SimpleClass2
    {
        public static readonly int NonAnswer = 34;

        private readonly int _answer = 42;
        
        public int SimpleMethod() => _answer;
        public int SimpleMethod(string question) => _answer;
        public static int SimpleStaticMethod(string question) => NonAnswer;
    }
}
