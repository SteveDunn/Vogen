using System;
using Shared;

namespace SnapshotTests
{
    /// <summary>
    /// We have tests targeting different versions of the framework and different locales.
    /// This method uses the major and minor version of the runtime version.
    /// </summary>
    public static class SnapshotUtils
    {
        public static string GetSnapshotDirectoryName(TargetFramework targetFramework, string locale = "")
        {
            string shortened = targetFramework switch
            {
                TargetFramework.Net4_8 => "4.8",
                TargetFramework.Net8_0 => "8.0",
                TargetFramework.AspNetCore8_0 => "AspNetCore8.0",
                TargetFramework.Net9_0 => "9.0",
                TargetFramework.AspNetCore9_0 => "AspNetCore9.0",
                _ => throw new InvalidOperationException($"Don't know about target framework {targetFramework}")
            };

            var s = $"snap-v{shortened}";
        
            if (locale.Length > 0)
            {
                s += $"-{locale}";
            }

            return s;
        }
    }
}