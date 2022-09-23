using System;
using System.Reflection;
using System.Runtime.Versioning;

namespace MediumTests.SnapshotTests
{
    /// <summary>
    /// We have tests targeting different versions of the framework and different locales.
    /// This method uses the major and minor version of the runtime version.
    /// </summary>
    public static class SnapshotUtils
    {
        public static string GetSnapshotDirectoryName(string locale = "")
        {
            var framework = Assembly
                .GetExecutingAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName!;

            string shortened = framework.Substring(framework.LastIndexOf("=", StringComparison.Ordinal) + 1);

            var s = $"snap-{shortened}";
        
            if (locale.Length > 0)
            {
                s += $"-{locale}";
            }

            return s;
        }
    }
}