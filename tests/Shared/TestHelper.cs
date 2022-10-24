using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Shared;

public static class TestHelper
{
    public static (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput<T>(string source)
        where T : IIncrementalGenerator, new()
    {
        var results = new ProjectBuilder()
            .WithSource(source)
            .WithTargetFramework(TargetFramework.Net6_0)
            .GetGeneratedOutput<T>();
        
        return results;
    }

    public static (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput<T>(string source, TargetFramework targetFramework)
        where T : IIncrementalGenerator, new()
    {
        var results = new ProjectBuilder()
            .WithSource(source)
            .WithTargetFramework(targetFramework)
            .GetGeneratedOutput<T>();
        
        return results;
    }

    public static string ShortenForFilename(string input)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

        //make sure the hash is only alpha numeric to prevent characters that may break the url
        return string.Concat(Convert.ToBase64String(hash).ToCharArray().Where(char.IsLetterOrDigit).Take(10));

    }
}