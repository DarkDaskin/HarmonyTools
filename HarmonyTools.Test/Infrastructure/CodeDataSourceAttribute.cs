using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using PathIO = System.IO.Path;

namespace HarmonyTools.Test.Infrastructure;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class CodeDataSourceAttribute(string path) : Attribute, ITestDataSource
{
    private static readonly int[] Versions = [1, 2];
    private static readonly string BasePath = @"..\..\..\..\HarmonyTools.Test.Source".Replace('\\', PathIO.DirectorySeparatorChar);
    private static readonly Dictionary<int, ReferenceAssemblies> ReferenceAssembliesPerVersion = new();

    public string Path { get; } = path;
    public string? FixedPath { get; set; }
    public bool ProvideVersion { get; set; }

    static CodeDataSourceAttribute()
    {
        var latestInstance = MSBuildLocator.QueryVisualStudioInstances()
            .Where(v => !v.MSBuildPath.Contains("preview"))
            .OrderByDescending(v => v.Version)
            .First();
        MSBuildLocator.RegisterInstance(latestInstance);

        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        foreach (var version in Versions)
            ReferenceAssembliesPerVersion.Add(version, GetReferenceAssemblies(version));
    }

    private static ReferenceAssemblies GetReferenceAssemblies(int version)
    {
        var projectFilePath = PathIO.Combine($"{BasePath}.V{version}", $"HarmonyTools.Test.Source.V{version}.csproj");
        var properties = new Dictionary<string, string>()
        {
#if DEBUG
            ["Configuration"] = "Debug"

#else
            ["Configuration"] = "Release"
#endif
        };
        using var workspace = MSBuildWorkspace.Create(properties);
        workspace.LoadMetadataForReferencedProjects = true;
        var project = workspace.OpenProjectAsync(projectFilePath).Result;

        var assemblies = ReferenceAssemblies.Default;
        assemblies = assemblies.WithAssemblies([..project.MetadataReferences.Select(mr => mr.Display)]);
        return assemblies;
    }

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        foreach (var version in Versions)
        {
            var fullPath = GetFullPath(Path, version, methodInfo);
            var fullFixedPath = FixedPath is null ? null : GetFullPath(FixedPath, version, methodInfo);
            if (File.Exists(fullPath) && (fullFixedPath is null || File.Exists(fullFixedPath)))
            {
                List<object> data = [File.ReadAllText(fullPath), ReferenceAssembliesPerVersion[version]];
                if (fullFixedPath is not null)
                    data.Add(File.ReadAllText(fullFixedPath));
                if (ProvideVersion)
                    data.Add(version);
                yield return data.ToArray();
            }
        }
    }

    private static string GetFullPath(string path, int version, MethodInfo methodInfo)
    {
        var basePathWithVersion = $"{BasePath}.V{version}";
        var directoryAttribute = methodInfo.GetCustomAttribute<CodeDirectoryAttribute>() ??
                                 methodInfo.DeclaringType?.GetCustomAttribute<CodeDirectoryAttribute>();
        return directoryAttribute is not null
            ? PathIO.Combine(basePathWithVersion, directoryAttribute.Directory.Replace('\\', PathIO.DirectorySeparatorChar), path)
            : PathIO.Combine(basePathWithVersion, path);
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data) => $"{methodInfo.Name} ({Path}, v{GetVersion(data!)})";

    private static int GetVersion(object[] data)
    {
        var assemblies = (ReferenceAssemblies)data[1];
        var harmonyAssemblyPath = assemblies.Assemblies.Single(s => s.Contains("0Harmony"));
        var parts = harmonyAssemblyPath.Split(PathIO.DirectorySeparatorChar);
        var version = Version.Parse(parts[^4]);
        return version.Major;
    }
}