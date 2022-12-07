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

namespace Shared
{
    public class ProjectBuilder
    {
        private static readonly ConcurrentDictionary<string, Lazy<Task<string[]>>> _cache = new(StringComparer.Ordinal);

        private static readonly KeyValuePair<string, ReportDiagnostic>[] _suppressedDiagnostics =
        {
            new("CS8019", ReportDiagnostic.Suppress), // Unnecessary using directive
            new("CS1701", ReportDiagnostic.Suppress) //  Assuming assembly reference
        };

        public IList<MetadataReference> References { get; } = new List<MetadataReference>();

        public TargetFramework TargetFramework { get; private set; } = TargetFramework.Net6_0;

        public ProjectBuilder WithTargetFramework(TargetFramework targetFramework)
        {
            TargetFramework = targetFramework;
            return this;
        }

        public void AddNuGetReference(string packageName, string version, string pathPrefix)
        {
            foreach (var reference in GetNuGetReferences(packageName, version, pathPrefix).Result)
            {
                References.Add(MetadataReference.CreateFromFile(reference));
            }
        }

        private void AddNuGetReferences()
        {
            switch (TargetFramework)
            {
                case TargetFramework.NetCoreApp3_1:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "3.1.0", "ref/netcoreapp3.1/");
                    AddNuGetReference("linq2db", "3.7.0", "lib/netcoreapp3.1/");
                    AddNuGetReference("Microsoft.EntityFrameworkCore", "5.0.17", "lib/netstandard2.1/");
                    AddNuGetReference("Dapper", "2.0.123", "lib/netstandard2.0/");
                    break;

                case TargetFramework.Net4_6_1:
                    AddNuGetReference("Microsoft.NETFramework.ReferenceAssemblies.net461", "1.0.0", "build/.NETFramework/v4.6.1/");
                    AddNuGetReference("NETStandard.Library", "2.0.3", "build/netstandard2.0/ref/");
                    AddNuGetReference("linq2db", "3.0.0", "lib/net46/");
                    AddNuGetReference("Microsoft.EntityFrameworkCore", "3.1.31", "lib/netstandard2.0/");
                    AddNuGetReference("Dapper", "2.0.123", "lib/net461/");
                    AddNuGetReference("System.Text.Json", "7.0.0", "lib/net462/");
                    break;

                case TargetFramework.Net4_8:
                    AddNuGetReference("Microsoft.NETFramework.ReferenceAssemblies.net48", "1.0.0", "build/.NETFramework/v4.8/");
                    AddNuGetReference("linq2db", "3.7.0", "lib/netstandard2.1/");
                    AddNuGetReference("Microsoft.EntityFrameworkCore", "5.0.17", "lib/netstandard2.1/");
                    AddNuGetReference("Dapper", "2.0.123", "lib/netstandard2.0/");
                    AddNuGetReference("System.Text.Json", "7.0.0", "lib/netstandard2.0/");
                    break;

                case TargetFramework.Net5_0:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "5.0.0", "ref/net5.0/");
                    AddNuGetReference("linq2db", "3.7.0", "lib/netstandard2.1/");
                    AddNuGetReference("Microsoft.EntityFrameworkCore", "5.0.17", "lib/netstandard2.1/");
                    AddNuGetReference("Dapper", "2.0.123", "lib/net5.0/");

                    break;

                case TargetFramework.Net6_0:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "6.0.0", "ref/net6.0/");
                    AddNuGetReference("linq2db", "3.7.0", "lib/netstandard2.1/");
                    AddNuGetReference("Microsoft.EntityFrameworkCore", "6.0.0", "lib/net6.0/");
                    AddNuGetReference("Dapper", "2.0.123", "lib/net5.0/");
                    break;

                case TargetFramework.Net7_0:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "7.0.0", "ref/net7.0/");
                    AddNuGetReference("linq2db", "4.3.0", "lib/net6.0/");
                    AddNuGetReference("Microsoft.EntityFrameworkCore", "7.0.0", "lib/net6.0/");
                    AddNuGetReference("Dapper", "2.0.123", "lib/net5.0/");

                    break;

                case TargetFramework.AspNetCore5_0:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "5.0.0", "ref/net5.0/");
                    AddNuGetReference("Microsoft.AspNetCore.App.Ref", "5.0.0", "ref/net5.0/");
                    AddNuGetReference("linq2db", "3.7.0", "lib/netstandard2.1/");
                    AddNuGetReference("Microsoft.EntityFrameworkCore", "5.0.17", "lib/netstandard2.1/");
                    break;

                case TargetFramework.AspNetCore6_0:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "6.0.0", "ref/net6.0/");
                    AddNuGetReference("Microsoft.AspNetCore.App.Ref", "6.0.0", "ref/net6.0/");
                    AddNuGetReference("linq2db", "3.7.0", "lib/netstandard2.1/");
                    AddNuGetReference("Microsoft.EntityFrameworkCore", "5.0.17", "lib/netstandard2.1/");

                    break;

                case TargetFramework.WindowsDesktop5_0:
                    AddNuGetReference("Microsoft.WindowsDesktop.App.Ref", "5.0.0", "ref/net5.0/");
                    AddNuGetReference("linq2db", "3.7.0", "lib/netstandard2.1/");
                    AddNuGetReference("Microsoft.EntityFrameworkCore", "5.0.17", "lib/netstandard2.1/");

                    break;
            }

            AddNuGetReference("System.Collections.Immutable", "1.5.0", "lib/netstandard2.0/");
            
            if (TargetFramework != TargetFramework.Net7_0)
            {
                AddNuGetReference("System.Numerics.Vectors", "4.5.0", "ref/netstandard2.0/");
            }


            AddNuGetReference("Microsoft.CSharp", "4.7.0", "lib/netstandard2.0/");  // To support dynamic type
            AddNuGetReference("Newtonsoft.Json", "13.0.2", "lib/netstandard2.0/"); 
        }

        private static Task<string[]> GetNuGetReferences(string packageName, string version, string path)
        {
            var task = _cache.GetOrAdd(packageName + '@' + version + ':' + path, key =>
            {
                return new Lazy<Task<string[]>>(Download);
            });

            return task.Value;

            async Task<string[]> Download()
            {
                var tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Vogen.Tests", "ref", packageName + '@' + version);
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

                var dlls = Directory.GetFiles(tempFolder, "*.dll");

                // Filter invalid .NET assembly
                var result = new List<string>();
                foreach (var dll in dlls)
                {
                    if (Path.GetFileName(dll) == "System.EnterpriseServices.Wrapper.dll")
                        continue;

                    try
                    {
                        using var stream = File.OpenRead(dll);
                        using var peFile = new PEReader(stream);
                        // ReSharper disable once UnusedVariable
                        var metadataReader = peFile.GetMetadataReader();
                        result.Add(dll);
                    }
                    catch
                    {
                    }
                }

                if(result.Count == 0)
                {
                    throw new InvalidOperationException("Did not add any DLLs as references!");
                }

                return result.ToArray();
            }
        }

        public ProjectBuilder WithSource(string source)
        {
            Source = source;

            return this;
        }

        public string Source { get; private set; } = string.Empty;

        public (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput<T>(
            bool ignoreInitialCompilationErrors,
            MetadataReference? valueObjectAttributeMetadata = null)
            where T : IIncrementalGenerator, new()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(Source);
            var syntaxTree2 = CSharpSyntaxTree.ParseText(@"    namespace System.Runtime.CompilerServices
    {
          internal static class IsExternalInit {}
    }
");

            MetadataReference r = valueObjectAttributeMetadata ?? MetadataReference.CreateFromFile(typeof(ValueObjectAttribute).Assembly.Location);

            References.Add(r);

            AddNuGetReferences();

            var compilation = CSharpCompilation.Create(
                "generator",
                new[] { syntaxTree, syntaxTree2 },
                References,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, specificDiagnosticOptions: _suppressedDiagnostics));

            var initialDiags = compilation.GetDiagnostics();
            if (initialDiags.Length != 0 && !ignoreInitialCompilationErrors)
            {
                return (initialDiags, string.Empty);
            }

            var originalTreeCount = compilation.SyntaxTrees.Length;
            var generator = new T();

            var driver = CSharpGeneratorDriver.Create(generator );
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiags);

            var finalDiags = outputCompilation.GetDiagnostics();

// //            if (finalDiags.Length != 0 && !ignoreFinalCompilationErrors)
//             if (finalDiags.Length != 0 && !ignoreFinalCompilationErrors)
//             {
//                 return (finalDiags, string.Empty);
//             }

            if (generatorDiags.Length != 0 && !ignoreInitialCompilationErrors)
            {
                return (generatorDiags, string.Empty);
            }

            //if (finalDiags.Length != 0 && !ignoreFinalCompilationErrors)
            if (finalDiags.Length != 0)
            {
                return (finalDiags, string.Empty);
            }

            var trees = outputCompilation.SyntaxTrees.ToList();


            return (generatorDiags, trees.Count != originalTreeCount ? trees[trees.Count - 1].ToString() : string.Empty);
        }
    }
}