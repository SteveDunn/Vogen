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
        private static readonly ConcurrentDictionary<string, Lazy<Task<string[]>>> s_cache = new(StringComparer.Ordinal);

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
                // case TargetFramework.NetStandard2_0:
                //     AddNuGetReference("NETStandard.Library", "2.0.3", "build/netstandard2.0/ref/");
                //     break;

                // case TargetFramework.NetStandard2_1:
                //     AddNuGetReference("NETStandard.Library.Ref", "2.1.0", "ref/netstandard2.1/");
                //     break;

                case TargetFramework.NetCoreApp3_1:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "3.1.0", "ref/netcoreapp3.1/");
                    break;

                case TargetFramework.Net4_6_1:
                    AddNuGetReference("Microsoft.NETFramework.ReferenceAssemblies.net461", "1.0.0", "build/.NETFramework/v4.6.1/");
                    break;

                case TargetFramework.Net4_8:
                    AddNuGetReference("Microsoft.NETFramework.ReferenceAssemblies.net48", "1.0.0", "build/.NETFramework/v4.8/");
                    break;

                case TargetFramework.Net5_0:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "5.0.0", "ref/net5.0/");
                    break;

                case TargetFramework.Net6_0:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "6.0.0", "ref/net6.0/");
                    break;

                case TargetFramework.Net7_0:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "7.0.0-rc.1.22426.10", "ref/net7.0/");
                    break;

                case TargetFramework.AspNetCore5_0:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "5.0.0", "ref/net5.0/");
                    AddNuGetReference("Microsoft.AspNetCore.App.Ref", "5.0.0", "ref/net5.0/");
                    break;

                case TargetFramework.AspNetCore6_0:
                    AddNuGetReference("Microsoft.NETCore.App.Ref", "6.0.0", "ref/net6.0/");
                    AddNuGetReference("Microsoft.AspNetCore.App.Ref", "6.0.0", "ref/net6.0/");
                    break;

                case TargetFramework.WindowsDesktop5_0:
                    AddNuGetReference("Microsoft.WindowsDesktop.App.Ref", "5.0.0", "ref/net5.0/");
                    break;
            }

            AddNuGetReference("System.Collections.Immutable", "1.5.0", "lib/netstandard2.0/");

            if (TargetFramework != TargetFramework.Net7_0)
            {
                AddNuGetReference("System.Numerics.Vectors", "4.5.0", "ref/netstandard2.0/");
            }

            AddNuGetReference("Microsoft.CSharp", "4.7.0", "lib/netstandard2.0/");  // To support dynamic type
        }

        private static Task<string[]> GetNuGetReferences(string packageName, string version, string path)
        {
            var task = s_cache.GetOrAdd(packageName + '@' + version + ':' + path, key =>
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

        public (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput<T>()
            where T : IIncrementalGenerator, new()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(Source);

            MetadataReference r = MetadataReference.CreateFromFile(typeof(ValueObjectAttribute).Assembly.Location);

            References.Add(r);

            AddNuGetReferences();

            var compilation = CSharpCompilation.Create(
                "generator",
                new[] { syntaxTree },
                References,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var originalTreeCount = compilation.SyntaxTrees.Length;
            var generator = new T();

            var driver = CSharpGeneratorDriver.Create(generator);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            var trees = outputCompilation.SyntaxTrees.ToList();

            return (diagnostics, trees.Count != originalTreeCount ? trees[trees.Count - 1].ToString() : string.Empty);
        }

    }
}