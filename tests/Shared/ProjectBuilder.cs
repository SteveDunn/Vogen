using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Vogen;

namespace Shared;

public record struct NuGetPackage(string PackageName, string Version, string PathPrefix);

public sealed class ProjectBuilder
{
    public IList<DiagnosticAnalyzer> DiagnosticAnalyzers { get; } = new List<DiagnosticAnalyzer>();
    public IList<DiagnosticResult> ExpectedDiagnosticResults { get; } = new List<DiagnosticResult>();

    public string? DefaultAnalyzerId { get; set; }
    public string? DefaultAnalyzerMessage { get; set; }

    private static readonly ConcurrentDictionary<string, Lazy<Task<string[]>>> _cache = new(StringComparer.Ordinal);

    private static readonly KeyValuePair<string, ReportDiagnostic>[] _suppressedDiagnostics =
    {
        new("CS8019", ReportDiagnostic.Suppress), // Unnecessary using directive
        new("CS1701", ReportDiagnostic.Suppress) //  Assuming assembly reference
    };

    private readonly IList<MetadataReference> _references = new List<MetadataReference>();
    private string _userSource = string.Empty;
    private TargetFramework? _targetFramework;
    private bool _excludeStj;
    private LanguageVersion _languageVersion = LanguageVersion.Default;
    private string _assemblyName = "generator";

    public ProjectBuilder WithTargetFramework(TargetFramework targetFramework)
    {
        _targetFramework = targetFramework;
        return this;
    }

    public ProjectBuilder WithMicrosoftCodeAnalysisNetAnalyzers(params string[] ruleIds) =>
        WithAnalyzerFromNuGet(
            "Microsoft.CodeAnalysis.NetAnalyzers",
            "9.0.0",
            "analyzers/dotnet/cs/Microsoft.CodeAnalysis",
            ruleIds);

    public ProjectBuilder ShouldReportDiagnostic(params DiagnosticResult[] expectedDiagnosticResults)
    {
        foreach (var diagnostic in expectedDiagnosticResults)
        {
            ExpectedDiagnosticResults.Add(diagnostic);
        }

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

    public ProjectBuilder WithAnalyzerFromNuGet(string packageName, string version, string path, string[] ruleIds)
    {
        var ruleFound = false;
        var references = GetNuGetReferences(packageName, version, path).Result;
        foreach (var reference in references)
        {
            var assembly = Assembly.LoadFrom(reference);
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || !typeof(DiagnosticAnalyzer).IsAssignableFrom(type))
                    continue;

                var instance = (DiagnosticAnalyzer) Activator.CreateInstance(type);
                if (instance.SupportedDiagnostics.Any(d => ruleIds.Contains(d.Id, StringComparer.Ordinal)))
                {
                    DiagnosticAnalyzers.Add(instance);
                    ruleFound = true;
                }
            }
        }

        if (!ruleFound)
            throw new InvalidOperationException("Rule id not found");

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
        if (_targetFramework is null)
        {
            throw new InvalidOperationException("No target framework!");
        }

        switch (_targetFramework)
        {
            case TargetFramework.Net4_8:
                AddNuGetReference("Microsoft.NETFramework.ReferenceAssemblies.net48", "1.0.0", "build/.NETFramework/v4.8/");
                AddNuGetReference("linq2db", "3.7.0", "lib/netstandard2.1/");
                AddNuGetReference("Microsoft.EntityFrameworkCore", "5.0.17", "lib/netstandard2.1/");
                AddNuGetReference("Dapper", "2.0.123", "lib/netstandard2.0/");
                AddStjIfNeeded("8.0.0", "lib/netstandard2.0/");
                AddNuGetReference("System.Memory", "4.5.5", "lib/netstandard2.0/");
                AddNuGetReference("ServiceStack.Text", "6.11.0", "lib/netstandard2.0");
                break;

            case TargetFramework.Net8_0:
                AddNuGetReference("Microsoft.NETCore.App.Ref", "8.0.0", "ref/net8.0/");
                AddNuGetReference("linq2db", "4.3.0", "lib/net6.0/");
                AddNuGetReference("Microsoft.EntityFrameworkCore", "8.0.10", "lib/net8.0/");
                AddNuGetReference("Dapper", "2.1.28", "lib/net7.0/");
                AddNuGetReference("ServiceStack.Text", "8.2.2", "lib/net8.0");
                AddNuGetReference("MongoDB.Bson", "2.27.0", "lib/netstandard2.0");
                break;

            case TargetFramework.AspNetCore8_0:
                AddNuGetReference("Microsoft.NETCore.App.Ref", "8.0.0", "ref/net8.0/");
                AddNuGetReference("Microsoft.AspNetCore.App.Ref", "8.0.10", "ref/net8.0/");
                AddNuGetReference("Swashbuckle.AspNetCore.SwaggerGen", "6.4.0", "lib/net6.0/");
                AddNuGetReference("Microsoft.OpenApi", "1.4.3.0", "lib/netstandard2.0/");
                AddNuGetReference("MongoDB.Bson", "2.27.0", "lib/netstandard2.0");
                break;

            case TargetFramework.Net9_0:
                AddNuGetReference("Microsoft.NETCore.App.Ref", "9.0.0", "ref/net9.0/");
                AddNuGetReference("linq2db", "4.3.0", "lib/net6.0/");
                AddNuGetReference("Microsoft.EntityFrameworkCore", "8.0.10", "lib/net8.0/");
                AddNuGetReference("Dapper", "2.1.28", "lib/net7.0/");
                AddNuGetReference("ServiceStack.Text", "8.2.2", "lib/net8.0");
                AddNuGetReference("MongoDB.Bson", "2.27.0", "lib/netstandard2.0");
                AddNuGetReference("Microsoft.AspNetCore.OpenApi", "9.0.6", "lib/net9.0");
                AddNuGetReference("Microsoft.OpenApi", "1.6.17", "lib/netstandard2.0/");
                break;

            case TargetFramework.AspNetCore9_0:
                AddNuGetReference("Microsoft.NETCore.App.Ref", "9.0.0", "ref/net9.0/");
                AddNuGetReference("Microsoft.AspNetCore.App.Ref", "9.0.0", "ref/net9.0/");
                AddNuGetReference("Swashbuckle.AspNetCore.SwaggerGen", "6.4.0", "lib/net6.0/");
                AddNuGetReference("MongoDB.Bson", "2.27.0", "lib/netstandard2.0");
                AddNuGetReference("Microsoft.AspNetCore.OpenApi", "9.0.6", "lib/net9.0");
                AddNuGetReference("Microsoft.OpenApi", "1.6.17", "lib/netstandard2.0/");
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        AddNuGetReference("System.Collections.Immutable", "1.5.0", "lib/netstandard2.0/");

        if (_targetFramework is not (TargetFramework.Net8_0 or TargetFramework.Net9_0))
        {
            AddNuGetReference("System.Numerics.Vectors", "4.5.0", "ref/netstandard2.0/");
        }

        AddNuGetReference("Microsoft.CSharp", "4.7.0", "lib/netstandard2.0/"); // To support dynamic type
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
                using var stream = await httpClient.GetStreamAsync(new Uri($"https://www.nuget.org/api/v2/package/{packageName}/{version}"))
                    .ConfigureAwait(false);
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
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }

            if (result.Count == 0)
            {
                throw new InvalidOperationException($"Did not add any DLLs as references for {packageName}, v {version}, at {path}!");
            }

            return result.ToArray();
        }
    }

    public ProjectBuilder WithAnalyzer<T>(string? id = null, string? message = null) where T : DiagnosticAnalyzer, new() =>
        WithAnalyzer(new T(), id, message);

    public ProjectBuilder WithAnalyzer(DiagnosticAnalyzer diagnosticAnalyzer, string? id = null, string? message = null)
    {
        DiagnosticAnalyzers.Add(diagnosticAnalyzer);
        DefaultAnalyzerId = id;
        DefaultAnalyzerMessage = message;
        return this;
    }

    public ProjectBuilder WithUserSource(string userSource)
    {
        _userSource = userSource;

        return this;
    }

    public ProjectBuilder WithAssemblyName(string assemblyName)
    {
        _assemblyName = assemblyName;

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
    public async Task<(ImmutableArray<Diagnostic> Diagnostics, SyntaxTree[] GeneratedSource)> GetGeneratedOutput<T>(
        bool ignoreInitialCompilationErrors,
        MetadataReference? valueObjectAttributeMetadata = null)
        where T : IIncrementalGenerator, new()
    {
        var parseOptions = new CSharpParseOptions(languageVersion: _languageVersion);

        var usersSyntaxTree = CSharpSyntaxTree.ParseText(_userSource, parseOptions);
        var isExternalInitSyntaxTree = CSharpSyntaxTree.ParseText(
            @"    namespace System.Runtime.CompilerServices
    {
          internal static class IsExternalInit {}
    }
",
            parseOptions);

        MetadataReference r = valueObjectAttributeMetadata ??
                              MetadataReference.CreateFromFile(typeof(ValueObjectAttribute).Assembly.Location);

        _references.Add(r);

        AddNuGetReferences();

        var options = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            moduleName: "VogenTests",
            specificDiagnosticOptions: _suppressedDiagnostics);

        var diagnostics = this.DiagnosticAnalyzers.SelectMany(
            analyzer => analyzer.SupportedDiagnostics.Select(
                diag => new KeyValuePair<string, ReportDiagnostic>(diag.Id, GetReportDiagnostic(diag))));

        diagnostics = diagnostics.Concat(_suppressedDiagnostics);

        options = options.WithSpecificDiagnosticOptions(diagnostics);

        var compilation = CSharpCompilation.Create(
            assemblyName: _assemblyName,
            syntaxTrees: [usersSyntaxTree, isExternalInitSyntaxTree],
            _references,
            options);

        AnalyzerConfigOptionsProvider optionsProvider = new TestAnalyzerConfigOptionsProvider(new Dictionary<string, string>());

        ImmutableArray<Diagnostic> initialDiags;

        if (DiagnosticAnalyzers.Count != 0)
        {
            var cwa = compilation.WithAnalyzers(
                ImmutableArray.CreateRange(DiagnosticAnalyzers),
                new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, optionsProvider));

            initialDiags = await cwa.GetAnalyzerDiagnosticsAsync(CancellationToken.None).ConfigureAwait(false);
        }
        else
        {
            initialDiags = compilation.GetDiagnostics();
        }

        //var initialDiags = compilation.GetDiagnostics();
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
            // uncomment to write out the source files - do a `dotnet new classlib` in that folder
            // and load it up in an IDE
#if DEBUG
            if (Debugger.IsAttached)
            {
                DumpSource(outputCompilation);
            }
#endif
            return (finalDiags, []);
        }

        return (generatorDiags, outputCompilation.SyntaxTrees.Except(compilation.SyntaxTrees).ToArray());
    }

    // ReSharper disable once UnusedMember.Local
    private static void DumpSource(Compilation outputCompilation)
    {
        string path = Path.Combine(Path.GetTempPath(), "vogen-source");

        int i = 0;

        foreach (var eachSyntaxTree in outputCompilation.SyntaxTrees)
        {
            string source = eachSyntaxTree.GetText().ToString();
            var filePath = eachSyntaxTree.FilePath;
            if (filePath.Length == 0)
            {
                filePath = $"file{++i}.cs";
            }

            string combinedPath = Path.Combine(path, filePath);

            Directory.CreateDirectory(Path.GetDirectoryName(combinedPath)!);

            File.WriteAllText(combinedPath, source);
        }
    }

    private static ReportDiagnostic GetReportDiagnostic(DiagnosticDescriptor descriptor)
    {
        return descriptor.DefaultSeverity switch
        {
            DiagnosticSeverity.Hidden => ReportDiagnostic.Hidden,
            DiagnosticSeverity.Info => ReportDiagnostic.Info,
            DiagnosticSeverity.Warning => ReportDiagnostic.Warn,
            DiagnosticSeverity.Error => ReportDiagnostic.Error,
            _ => ReportDiagnostic.Info, // Ensure the analyzer is enabled for the test
        };
    }
}

internal sealed class TestAnalyzerConfigOptionsProvider(Dictionary<string, string>? values) : AnalyzerConfigOptionsProvider
{
    private readonly Dictionary<string, string> _values = values ?? [];

    public override AnalyzerConfigOptions GlobalOptions => new TestAnalyzerConfigOptions(_values);
    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => new TestAnalyzerConfigOptions(_values);
    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => new TestAnalyzerConfigOptions(_values);

    private sealed class TestAnalyzerConfigOptions(Dictionary<string, string> values) : AnalyzerConfigOptions
    {
        private readonly Dictionary<string, string> _values = values;

        public override bool TryGetValue(string key, out string value)
        {
            return _values.TryGetValue(key, out value);
        }
    }
}