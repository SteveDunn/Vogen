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
                TargetFramework.Net4_6_1 => "4.6.1",
                TargetFramework.Net4_8 => "4.8",
                TargetFramework.NetCoreApp3_1 => "3.1",
                TargetFramework.Net5_0 => "5.0",
                TargetFramework.Net6_0 => "6.0",
                TargetFramework.Net7_0 => "7.0",
                TargetFramework.Net8_0 => "8.0",
                //TargetFramework.NetStandard2_0 => "2.0",
//                TargetFramework.NetStandard2_1 => "2.1",
                _ => throw new InvalidOperationException($"Don't know about target frame {targetFramework}")
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