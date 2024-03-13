﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HarmonyTools.Test.PatchBase
{
    public class SimpleClass
    {
        public static readonly int NonAnswer = 34;

        private readonly int _answer = 42;
        
        public int SimpleMethod(string question) => _answer;

        public int OverloadedMethod(int x, int y) => x + y;
        public double OverloadedMethod(double x, double y) => x - y;
        public void OverloadedMethod(out int answer) => answer = _answer;
        public void GenericMethod<T>() { }
        public FileSystemInfo GetFile() => null;

        public IEnumerable<string> EnumeratorMethod()
        {
            yield break;
        }

        public async Task AsyncMethod()
        {
            await Task.Delay(1000);
        }

        public int ReadOnlyProp => 1;
        public int ReadWriteProp { get; set; } = 2;

        public int this[int index] => index + 1;
        public double this[double index] => index - 1;
    }
}
