using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    private static readonly string BasePath = @"..\..\..\..\HarmonyTools.Test.Source";
    private static readonly Dictionary<int, ReferenceAssemblies> ReferenceAssembliesPerVersion = new();

    public string Path { get; } = path ?? throw new ArgumentNullException(nameof(path));

    static CodeDataSourceAttribute()
    {
        var latestInstance = MSBuildLocator.QueryVisualStudioInstances()
            .Where(v => !v.MSBuildPath.Contains("preview"))
            .OrderByDescending(v => v.Version)
            .First();
        //FixTrailingSlash(latestInstance);
        MSBuildLocator.RegisterInstance(latestInstance);

        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        foreach (var version in Versions)
            ReferenceAssembliesPerVersion.Add(version, GetReferenceAssemblies(version));
    }

    private static void FixTrailingSlash(VisualStudioInstance instance)
    {
        foreach (var field in instance.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (field.Name.Contains("Path"))
            {
                var value = (string)field.GetValue(instance)!;
                if (!value.EndsWith("\\"))
                {
                    value += "\\";
                    field.SetValue(instance, value);
                }
            }
        }
    }

    private static ReferenceAssemblies GetReferenceAssemblies(int version)
    {
        var projectFilePath = PathIO.Combine($"{BasePath}.V{version}", $"HarmonyTools.Test.Source.V{version}.csproj");
        using var workspace = MSBuildWorkspace.Create();
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
            var fullPath = PathIO.Combine($"{BasePath}.V{version}", Path);
            if (File.Exists(fullPath))
                yield return [File.ReadAllText(fullPath), ReferenceAssembliesPerVersion[version]];
        }
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data) => $"{methodInfo.Name} ({Path}, v{GetVersion(data)})";

    private static int GetVersion(object[] data)
    {
        var assemblies = (ReferenceAssemblies)data[1];
        var harmonyAssemblyPath = assemblies.Assemblies.Single(s => s.Contains("0Harmony"));
        var parts = harmonyAssemblyPath.Split(PathIO.DirectorySeparatorChar);
        var version = Version.Parse(parts[^4]);
        return version.Major;
    }
}