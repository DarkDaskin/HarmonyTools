using System;

namespace HarmonyTools.Test.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CodeDirectoryAttribute : Attribute
{
    public CodeDirectoryAttribute(string directory)
    {
        if (string.IsNullOrEmpty(directory))
            throw new ArgumentNullException(nameof(directory));

        Directory = directory;
    }

    public string Directory { get; }
}