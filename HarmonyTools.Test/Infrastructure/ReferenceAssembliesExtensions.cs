using Microsoft.CodeAnalysis.Testing;
using System.IO;
using System;
using System.Linq;

namespace HarmonyTools.Test.Infrastructure;

internal static class ReferenceAssembliesExtensions
{
    public static int GetHarmonyVersion(this ReferenceAssemblies assemblies)
    {
        var harmonyAssemblyPath = assemblies.Assemblies.Single(s => s.Contains("0Harmony"));
        var parts = harmonyAssemblyPath.Split(Path.DirectorySeparatorChar);
        var version = Version.Parse(parts[^4]);
        return version.Major;
    }
}