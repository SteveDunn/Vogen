using System;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Shared;
using VerifyTests;
using VerifyXunit;

namespace LargeTests
{
    public class SnapshotRunner<T> where T : IIncrementalGenerator, new()
    {
        public SnapshotRunner([CallerFilePath]string caller = "")
        {
            int n = caller.LastIndexOf('\\');
            n = n > 0 ? n : caller.LastIndexOf('/');
            _path = Path.Combine(caller.Substring(0, n), "snapshots");
        }

        private readonly TargetFramework[] _allFrameworks = {
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

        public async Task RunOnAllFrameworks() => await RunOn(_allFrameworks);

        public SnapshotRunner<T> WithSource(string source)
        {
            _source = source;
            return this;
        }

        public SnapshotRunner<T> CustomizeSettings(Action<VerifySettings> settings)
        {
            _customizesSettings = settings;
            return this;
        }

        private async Task RunOn(params TargetFramework[] frameworks)
        {
            _ = _source ?? throw new InvalidOperationException("No source!");

            foreach (var eachFramework in frameworks)
            {
                VerifySettings? verifySettings = null;

                if (_customizesSettings is not null)
                {
                    verifySettings = new();
                    _customizesSettings(verifySettings);
                }

                using var scope = new AssertionScope();

                var (diagnostics, output) = GetGeneratedOutput(_source, eachFramework);
                diagnostics.Should().BeEmpty();

                var outputFolder = Path.Combine(_path, SnapshotUtils.GetSnapshotDirectoryName(eachFramework, _locale));

                // verifySettings ??= new VerifySettings();
                // verifySettings.AutoVerify();

                await Verifier.Verify(output, verifySettings).UseDirectory(outputFolder);
            }
        }

        private static (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput(string source, TargetFramework targetFramework)
        {
            var results = new ProjectBuilder()
                .WithSource(source)
                .WithTargetFramework(targetFramework)
                .GetGeneratedOutput<T>();

            return results;
        }

    }
}