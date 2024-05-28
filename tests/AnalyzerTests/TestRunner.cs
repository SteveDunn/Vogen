using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Shared;

namespace AnalyzerTests
{
    public class TestRunner<T> where T : IIncrementalGenerator, new()
    {
        private readonly TargetFramework[] _allFrameworks = {
            TargetFramework.Net8_0,
#if THOROUGH
            TargetFramework.Net6_0,
            TargetFramework.Net7_0,
            TargetFramework.Net4_6_1,
            TargetFramework.Net4_8,
            TargetFramework.NetCoreApp3_1,
            TargetFramework.Net5_0,
#endif
        };

        private Action<ImmutableArray<Diagnostic>>? _validationMethod;
        private string? _source;

        public async Task RunOnAllFrameworks(bool ignoreInitialCompilationErrors = false)
        {
            await RunOn(
                ignoreInitialCompilationErrors,
                _allFrameworks);
        }

        public TestRunner<T> WithSource(string source)
        {
            _source = source;
            return this;
        }
        
        public TestRunner<T> ValidateWith(Action<ImmutableArray<Diagnostic>> method)
        {
            _validationMethod = method;

            return this;
        }

        private async Task RunOn(bool ignoreInitialCompilationErrors, params TargetFramework[] frameworks)
        {
            _ = _source ?? throw new InvalidOperationException("No source!");
            _ = _validationMethod ?? throw new InvalidOperationException("No validation method!");

            // Skips tests targeting specific frameworks that were excluded above
            // NOTE: Requires [SkippableFact] attribute to be added to single-framework tests
            Skip.If(frameworks.Length == 0);

            foreach (var eachFramework in frameworks)
            {
                using var scope = new AssertionScope();

                var (diagnostics, _) = await GetGeneratedOutput(
                    _source,
                    eachFramework,
                    ignoreInitialCompilationErrors);

                _validationMethod(diagnostics);
            }
        }

        private static async Task<(ImmutableArray<Diagnostic> Diagnostics, SyntaxTree[] GeneratedSource)> GetGeneratedOutput(
            string source,
            TargetFramework targetFramework, 
            bool ignoreInitialCompilationErrors)
        {
            var results =await new ProjectBuilder()
                // .WithMicrosoftCodeAnalysisNetAnalyzers()
                .WithUserSource(source)
                .WithTargetFramework(targetFramework)
                .GetGeneratedOutput<T>(ignoreInitialCompilationErrors);

            return results;
        }

    }
}