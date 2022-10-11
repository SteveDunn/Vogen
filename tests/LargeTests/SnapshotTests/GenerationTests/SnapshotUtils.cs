using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace LargeTests.SnapshotTests.GenerationTests
{
    /// <summary>
    /// We have tests targeting different versions of the framework and different locales.
    /// This method uses the major and minor version of the runtime version.
    /// </summary>
    public static class SnapshotUtils
    {
        public static string ShortenForFilename(string input)
        {
            using var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

            //make sure the hash is only alpha numeric to prevent characters that may break the url
            return string.Concat(Convert.ToBase64String(hash).ToCharArray().Where(char.IsLetterOrDigit).Take(10));

        }

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