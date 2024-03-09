﻿using System.Reflection;
using Harmony;

namespace HarmonyTools.Test.Source.V1
{
    [HarmonyPatch]
    internal class NonStaticPatchMethods
    {
        public void Prefix() { }

        public void Postfix() { }

        public void Transpiler() { }
        
        public void Prepare() { }

        public void Cleanup() { }

        public MethodBase TargetMethod() => default;

        public void NonPatchMethod() { }
    }
}