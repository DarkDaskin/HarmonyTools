using System.Runtime.CompilerServices;

// Make nullable reference type attributes available to test sources.
// This is an easier alternative to adding support for NuGet package content files.
[assembly: InternalsVisibleTo("HarmonyTools.Test.Source.V1")]
[assembly: InternalsVisibleTo("HarmonyTools.Test.Source.V2")]
[assembly: InternalsVisibleTo("TestProject")]