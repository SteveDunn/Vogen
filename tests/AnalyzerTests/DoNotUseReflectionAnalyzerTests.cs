using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Vogen;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotUseReflectionAnalyzer>;

namespace AnalyzerTests
{
    namespace NotSystem
    {
        public static class Activator
        {
            public static T? CreateInstance<T>() => default(T);
        }
    }

    public class DoNotUseReflectionAnalyzerTests
    {
        [Fact]
        public void Allows_using_Activate_CreateInstance_from_another_namespace()
        {
            var x = NotSystem.Activator.CreateInstance<string>();
        }

        private class Types : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new[] {"partial class"};
                yield return new[] {"partial struct"};
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
        public async Task Disallows_generic_method(string type)
        {
            var source = $@"using Vogen;
using System;

namespace Whatever;

public class Test {{
    public Test() {{
        var c = {{|#0:Activator.CreateInstance<MyVo>()|}};
    }}
}}

[ValueObject(typeof(int))]
public {type} MyVo {{ }}
";
            
            await Run(
                source,
                WithDiagnostics("VOG025", DiagnosticSeverity.Error, "MyVo", 0));
        }

        [Theory]
        [ClassData(typeof(Types))]
        public async Task Disallows_non_generic_method(string type)
        {
            var source = $@"using Vogen;
using System;

namespace Whatever;

public class Test {{
    public Test() {{
        var c = {{|#0:Activator.CreateInstance(typeof(MyVo))|}};
    }}
}}

[ValueObject(typeof(int))]
public {type} MyVo {{ }}
";
            
            await Run(
                source,
                WithDiagnostics("VOG025", DiagnosticSeverity.Error, "MyVo", 0));
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
