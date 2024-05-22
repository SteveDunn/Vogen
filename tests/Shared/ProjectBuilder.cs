using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vogen;

namespace Shared;

public record struct NuGetPackage(string PackageName, string Version, string PathPrefix);

public class ProjectBuilder
{
    private static readonly ConcurrentDictionary<string, Lazy<Task<string[]>>> _cache = new(StringComparer.Ordinal);

    private static readonly KeyValuePair<string, ReportDiagnostic>[] _suppressedDiagnostics =
    {
        new("CS8019", ReportDiagnostic.Suppress), // Unnecessary using directive
        new("CS1701", ReportDiagnostic.Suppress) //  Assuming assembly reference
    };

    private readonly IList<MetadataReference> _references = new List<MetadataReference>();
    private string _userSource = string.Empty;
    private TargetFramework? _targetFramework;
    private bool _excludeStj = false;
    private LanguageVersion _languageVersion = LanguageVersion.Default;

    public ProjectBuilder WithTargetFramework(TargetFramework targetFramework)
    {
        _targetFramework = targetFramework;
        return this;
    }

    public ProjectBuilder ShouldExcludeSystemTextJson(bool excludeStj = false)
    {
        _excludeStj = excludeStj;
        return this;
    }

    public ProjectBuilder WithLanguageVersion(LanguageVersion languageVersion)
    {
        _languageVersion = languageVersion;
        return this;
    }

    public void AddNuGetReference(string packageName, string version, string pathPrefix)
    {
        foreach (string reference in GetNuGetReferences(packageName, version, pathPrefix).Result)
        {
            _references.Add(MetadataReference.CreateFromFile(reference));
        }
    }

    private void AddNuGetReferences()
    {
        if(_targetFramework is null)
        {
            throw new InvalidOperationException("No target framework!");
        }

        switch (_targetFramework)
        {
            case TargetFramework.NetCoreApp3_1:
                AddNuGetReference("Microsoft.NETCore.App.Ref", "3.1.0", "ref/netcoreapp3.1/");
                AddNuGetReference("linq2db", "3.7.0", "lib/netcoreapp3.1/");
                AddNuGetReference("Microsoft.EntityFrameworkCore", "5.0.17", "lib/netstandard2.1/");
                AddNuGetReference("Dapper", "1.60.6", "lib/netstandard2.0/");
                AddNuGetReference("ServiceStack.Text", "5.14.0", "lib/netstandard2.0");
                break;

            case TargetFramework.Net4_6_1:
                AddNuGetReference("Microsoft.NETFramework.ReferenceAssemblies.net461", "1.0.0", "build/.NETFramework/v4.6.1/");
                AddNuGetReference("NETStandard.Library", "2.0.3", "build/netstandard2.0/ref/");
                AddNuGetReference("linq2db", "3.0.0", "lib/net46/");
                AddNuGetReference("Microsoft.EntityFrameworkCore", "3.1.31", "lib/netstandard2.0/");
                AddNuGetReference("Dapper", "2.0.123", "lib/net461/");
                AddStjIfNeeded("8.0.0", "lib/net462/");
                AddNuGetReference("System.Memory", "4.5.5", "lib/net461/");
                AddNuGetReference("ServiceStack.Text", "4.0.62", "lib/net40");
                break;

            case TargetFramework.Net4_8:
                AddNuGetReference("Microsoft.NETFramework.ReferenceAssemblies.net48", "1.0.0", "build/.NETFramework/v4.8/");
                AddNuGetReference("linq2db", "3.7.0", "lib/netstandard2.1/");
                AddNuGetReference("Microsoft.EntityFrameworkCore", "5.0.17", "lib/netstandard2.1/");
                AddNuGetReference("Dapper", "2.0.123", "lib/netstandard2.0/");
                AddStjIfNeeded("8.0.0", "lib/netstandard2.0/");
                AddNuGetReference("System.Memory", "4.5.5", "lib/netstandard2.0/");
                AddNuGetReference("ServiceStack.Text", "6.11.0", "lib/netstandard2.0");
                break;

            case TargetFramework.Net5_0:
                AddNuGetReference("Microsoft.NETCore.App.Ref", "5.0.0", "ref/net5.0/");
                AddNuGetReference("linq2db", "3.7.0", "lib/netstandard2.1/");
                AddNuGetReference("Microsoft.EntityFrameworkCore", "5.0.17", "lib/netstandard2.1/");
                AddNuGetReference("Dapper", "2.0.123", "lib/net5.0/");
                AddNuGetReference("ServiceStack.Text", "5.5.0", "lib/netstandard2.0");
                break;

            case TargetFramework.Net6_0:
                AddNuGetReference("Microsoft.NETCore.App.Ref", "6.0.0", "ref/net6.0/");
                AddNuGetReference("linq2db", "3.7.0", "lib/netstandard2.1/");
                AddNuGetReference("Microsoft.EntityFrameworkCore", "6.0.0", "lib/net6.0/");
                AddNuGetReference("Dapper", "2.0.123", "lib/net5.0/");
                AddNuGetReference("ServiceStack.Text", "6.11.0", "lib/net6.0");

                break;

            case TargetFramework.Net7_0:
                AddNuGetReference("Microsoft.NETCore.App.Ref", "7.0.0", "ref/net7.0/");
                AddNuGetReference("linq2db", "4.3.0", "lib/net6.0/");
                AddNuGetReference("Microsoft.EntityFrameworkCore", "7.0.0", "lib/net6.0/");
                AddNuGetReference("Dapper", "2.0.123", "lib/net5.0/");
                AddNuGetReference("ServiceStack.Text", "6.11.0", "lib/net6.0");


                break;

            case TargetFramework.Net8_0:
                AddNuGetReference("Microsoft.NETCore.App.Ref", "8.0.0", "ref/net8.0/");
                AddNuGetReference("linq2db", "4.3.0", "lib/net6.0/");
                AddNuGetReference("Microsoft.EntityFrameworkCore", "8.0.0", "lib/net8.0/");
                AddNuGetReference("Dapper", "2.1.28", "lib/net7.0/");
                AddNuGetReference("ServiceStack.Text", "8.2.2", "lib/net8.0");

                break;

            case TargetFramework.AspNetCore8_0:
                AddNuGetReference("Microsoft.NETCore.App.Ref", "8.0.0", "ref/net8.0/");
                AddNuGetReference("Microsoft.AspNetCore.App.Ref", "8.0.0", "ref/net8.0/");
                AddNuGetReference("Swashbuckle.AspNetCore.SwaggerGen", "6.4.0", "lib/net6.0/");
                AddNuGetReference("Microsoft.OpenApi", "1.4.3.0", "lib/netstandard2.0/");
                break;

        }

        AddNuGetReference("System.Collections.Immutable", "1.5.0", "lib/netstandard2.0/");
            
        if (_targetFramework is not TargetFramework.Net7_0 and not TargetFramework.Net8_0)
        {
            AddNuGetReference("System.Numerics.Vectors", "4.5.0", "ref/netstandard2.0/");
        }


        AddNuGetReference("Microsoft.CSharp", "4.7.0", "lib/netstandard2.0/");  // To support dynamic type
        AddNuGetReference("Newtonsoft.Json", "13.0.2", "lib/netstandard2.0/"); 
    }

    private void AddStjIfNeeded(string version, string pathPrefix)
    {
        if (!_excludeStj)
        {
            AddNuGetReference("System.Text.Json", version, pathPrefix);
        }
    }

    private static Task<string[]> GetNuGetReferences(string packageName, string version, string path)
    {
        var task = _cache.GetOrAdd($"{packageName}@{version}:{path}", _ => new Lazy<Task<string[]>>(Download));

        return task.Value;

        async Task<string[]> Download()
        {
            var tempFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Vogen.Tests",
                "ref",
                packageName + '@' + version);

            if (!Directory.Exists(tempFolder) || !Directory.EnumerateFileSystemEntries(tempFolder).Any())
            {
                Directory.CreateDirectory(tempFolder);
                using var httpClient = new HttpClient();
                using var stream = await httpClient.GetStreamAsync(new Uri($"https://www.nuget.org/api/v2/package/{packageName}/{version}")).ConfigureAwait(false);
                using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

                foreach (var entry in zip.Entries.Where(file => file.FullName.StartsWith(path, StringComparison.Ordinal)))
                {
                    entry.ExtractToFile(Path.Combine(tempFolder, entry.Name), overwrite: true);
                }
            }

            string[] nameAndPathsForDlls = Directory.GetFiles(tempFolder, "*.dll");

            // Filter invalid .NET assembly
            var result = new List<string>();
            foreach (string eachDllNameAndPath in nameAndPathsForDlls)
            {                                                                                                                         
                if (Path.GetFileName(eachDllNameAndPath) == "System.EnterpriseServices.Wrapper.dll")
                    continue;

                try
                {
                    using var stream = File.OpenRead(eachDllNameAndPath);
                    using var peFile = new PEReader(stream);
                    // ReSharper disable once UnusedVariable
                    var metadataReader = peFile.GetMetadataReader();
                    result.Add(eachDllNameAndPath);
                }
                catch
                {
                }
            }

            if(result.Count == 0)
            {
                throw new InvalidOperationException($"Did not add any DLLs as references for {packageName}, v {version}, at {path}!");
            }

            return result.ToArray();
        }
    }

    public ProjectBuilder WithUserSource(string userSource)
    {
        _userSource = userSource;

        return this;
    }

    public ProjectBuilder WithNugetPackages(IEnumerable<NuGetPackage> packages)
    {
        foreach (var nuGetPackage in packages)
        {
            AddNuGetReference(nuGetPackage.PackageName, nuGetPackage.Version, nuGetPackage.PathPrefix);
        }
        
        return this;
    }

    // filePath can be specified for when you want to get the non-default
    // generated file, for instance, the System.Text.Json converter factory that is generated.
    public (ImmutableArray<Diagnostic> Diagnostics, SyntaxTree[] GeneratedSource) GetGeneratedOutput<T>(
        bool ignoreInitialCompilationErrors,
        MetadataReference? valueObjectAttributeMetadata = null)
        where T : IIncrementalGenerator, new()
    {
        var parseOptions = new CSharpParseOptions(languageVersion: _languageVersion);
        
        var usersSyntaxTree = CSharpSyntaxTree.ParseText(_userSource, parseOptions);
        var isExternalInitSyntaxTree = CSharpSyntaxTree.ParseText(@"    namespace System.Runtime.CompilerServices
    {
          internal static class IsExternalInit {}
    }
", parseOptions);

        MetadataReference r = valueObjectAttributeMetadata ?? MetadataReference.CreateFromFile(typeof(ValueObjectAttribute).Assembly.Location);

        _references.Add(r);

        AddNuGetReferences();

        var compilation = CSharpCompilation.Create(
            assemblyName: "generator",
            syntaxTrees: new[] { usersSyntaxTree, isExternalInitSyntaxTree },
            _references,
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary, 
                specificDiagnosticOptions: _suppressedDiagnostics));

        var initialDiags = compilation.GetDiagnostics();
        if (initialDiags.Length != 0 && !ignoreInitialCompilationErrors)
        {
            return (initialDiags, []);
        }

        var generator = new T();

        var driver = CSharpGeneratorDriver
            .Create(generator)
            .WithUpdatedParseOptions(parseOptions.WithDocumentationMode(DocumentationMode.Diagnose));
        
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiags);

        var finalDiags = outputCompilation.GetDiagnostics();

        if (generatorDiags.Length != 0 && !ignoreInitialCompilationErrors)
        {
            return (generatorDiags, []);
        }

        if (finalDiags.Length != 0)
        {
            return (finalDiags, []);
        }

        return (generatorDiags, outputCompilation.SyntaxTrees.Except(compilation.SyntaxTrees).ToArray());
    }
}