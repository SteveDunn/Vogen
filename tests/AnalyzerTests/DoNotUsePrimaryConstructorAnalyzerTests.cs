using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Vogen;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotUsePrimaryConstructorAnalyzer>;

namespace AnalyzerTests
{
    public class DoNotUsePrimaryConstructorAnalyzerTests
    {
        private class Types : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new[] {"readonly partial struct"};
                yield return new[] {"partial record class"};
                yield return new[] {"partial record struct"};
                yield return new[] {"readonly partial record struct"};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        //No diagnostics expected to show up
        [Fact]
        public async Task NoDiagnosticsForEmptyCode()
        {
            var test = @"";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Theory]
        [ClassData(typeof(Types))]
        public async Task Primary_constructors_with_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;
namespace Whatever {{
    [ValueObject(typeof(int))]
    public {type} {{|#0:MyVo(int SomethingElse)|}} {{ }}
}}

namespace System.Runtime.CompilerServices {{
    internal static class IsExternalInit {{}}
}}
";
            await Run(
                source,
                WithDiagnostics("VOG018", DiagnosticSeverity.Error, "MyVo", 0));
        }
        
        [Theory]
        [ClassData(typeof(Types))]
        public async Task Primary_constructors_with_multiple_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public {type} MyVo(int SomethingElse, string Name, int Age) {{ }}
";
            await Run(
                source,
                WithDiagnostics("VOG018", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }
        [Theory]
        [ClassData(typeof(Types))]
        public async Task Primary_constructors_with_empty_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public {type} MyVo() {{ }}
";
            await Run(
                source,
                WithDiagnostics("VOG018", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }

        private static IEnumerable<DiagnosticResult> WithDiagnostics(string code, DiagnosticSeverity severity,
            string arguments, params int[] locations)
        {
            foreach (var location in locations)
            {
                yield return VerifyCS.Diagnostic(code).WithSeverity(severity).WithLocation(location)
                    .WithArguments("MyVo");
            }
        }

        private async Task Run(string source, IEnumerable<DiagnosticResult> expected)
        {
            var loc = typeof(ValueObjectAttribute).Assembly.Location;

            var referenceAssemblies = ReferenceAssemblies.Default
                .AddAssemblies(
                    ImmutableArray.Create("Vogen", "Vogen.SharedTypes", loc.Replace(".dll", string.Empty))
                );

            var test = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { source },
                },

                CompilerDiagnostics = CompilerDiagnostics.Errors,
                ReferenceAssemblies = referenceAssemblies,
            };

            test.ExpectedDiagnostics.AddRange(expected);

            await test.RunAsync();
        }
    }
}
