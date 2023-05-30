using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Shared;
using VerifyTests;
using VerifyXunit;
using Vogen;
using Xunit.Abstractions;

namespace SnapshotTests
{
    public class SnapshotRunner<T> where T : IIncrementalGenerator, new()
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly PortableExecutableReference _metadataRef = MetadataReference.CreateFromFile(typeof(ValueObjectAttribute).Assembly.Location);

        public SnapshotRunner([CallerFilePath] string caller = "")
        {
            int n = caller.LastIndexOf('\\');
            n = n > 0 ? n : caller.LastIndexOf('/');
            _path = Path.Combine(caller.Substring(0, n), "snapshots");
        }

        private readonly TargetFramework[] _allFrameworks =
        {
            TargetFramework.Net6_0,
            TargetFramework.Net7_0,
#if THOROUGH
            TargetFramework.Net4_6_1,
            TargetFramework.Net4_8,
            TargetFramework.NetCoreApp3_1,
            TargetFramework.Net5_0,
#endif
        };

        public SnapshotRunner<T> WithLocale(string locale)
        {
            _locale = locale;
            return this;
        }

        private string? _source;
        private readonly string _path;
        private Action<VerifySettings>? _customizesSettings;

        private string _locale = string.Empty;
        private bool _ignoreInitialCompilationErrors;
        private ITestOutputHelper? _logger;

        public async Task RunOnAllFrameworks() => await RunOn(_allFrameworks);

        public SnapshotRunner<T> WithSource(string source)
        {
            _source = source;
            return this;
        }

        public SnapshotRunner<T> IgnoreInitialCompilationErrors()
        {
            _ignoreInitialCompilationErrors = true;
            return this;
        }

        public SnapshotRunner<T> CustomizeSettings(Action<VerifySettings> settings)
        {
            _customizesSettings = settings;
            return this;
        }

        public async Task RunOn(params TargetFramework[] frameworks)
        {
            _ = _source ?? throw new InvalidOperationException("No source!");

#if NET7_0
            // Only run .NET 7 tests when using the .NET 7 SDK (prevents assembly versioning issues with <6.0)
            frameworks = frameworks.Where(framework => framework == TargetFramework.Net7_0).ToArray();
#elif NET6_0
            // Alternatively, only run non-net7 tests when using the .NET 6 target
            // as .NET 6 will use the .NET standard Vogen binary (without C#11 support)
            frameworks = frameworks.Where(framework => framework != TargetFramework.Net7_0).ToArray();
#endif

            // Skips tests targeting specific frameworks that were excluded above
            // NOTE: Requires [SkippableFact] attribute to be added to single-framework tests
            Skip.If(frameworks.Length == 0);

            VerifySettings? verifySettings = null;

            if (_customizesSettings is not null)
            {
                verifySettings = new();
                _customizesSettings(verifySettings);
            }

            await Parallel.ForEachAsync(frameworks, async (fx, _) => await Run(fx, verifySettings));
        }

        private async Task Run(TargetFramework framework, VerifySettings? verifySettings)
        {
            _ = _source ?? throw new InvalidOperationException("No source!");

            _logger?.WriteLine($"Running on {framework}");

            using var scope = new AssertionScope();

            var (diagnostics, output) = GetGeneratedOutput(_source, framework);
            diagnostics.Should().BeEmpty(@$"because the following source code should compile on {framework}: " + _source);

            var outputFolder = Path.Combine(_path, SnapshotUtils.GetSnapshotDirectoryName(framework, _locale));

#if RESET_SNAPSHOTS
                _logger?.WriteLine("** Auto verifying snapshots as RESET_SNAPSHOTS is true!");
                    
                    verifySettings ??= new VerifySettings();
                    verifySettings.AutoVerify();
#endif

            await Verifier.Verify(output, verifySettings).UseDirectory(outputFolder);
        }

        private (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput(string source, TargetFramework targetFramework)
        {
            var results = new ProjectBuilder()
                .WithSource(source)
                .WithTargetFramework(targetFramework)
                .GetGeneratedOutput<T>(_ignoreInitialCompilationErrors, _metadataRef);

            return results;
        }

        public SnapshotRunner<T> WithLogger(ITestOutputHelper logger)
        {
            _logger = logger;

            return this;
        }
    }
}