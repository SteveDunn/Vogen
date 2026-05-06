using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotUseGetValueOrDefaultAnalyzer>;

namespace AnalyzerTests
{
    public class DoNotUseGetValueOrDefaultAnalyzerTests
    {
        private class StructTypes : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return ["partial struct"];
                yield return ["readonly partial struct"];
                yield return ["partial record struct"];
                yield return ["readonly partial record struct"];
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Fact]
        public async Task NoDiagnosticsForEmptyCode()
        {
            var test = @"";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Theory]
        [ClassData(typeof(StructTypes))]
        public async Task Disallow_parameterless_GetValueOrDefault_on_nullable_value_objects(string type)
        {
            var source = $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public {type} MyVo {{ }}

public class Test {{
    public Test() {{
        MyVo? value = null;
        var result = {{|#0:value.GetValueOrDefault()|}};
    }}
}}
";

            await Run(source, WithDiagnostics("VOG038", DiagnosticSeverity.Error, "MyVo", 0));
        }

        [Theory]
        [ClassData(typeof(StructTypes))]
        public async Task Allow_GetValueOrDefault_with_explicit_default_value(string type)
        {
            var source = $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public {type} MyVo {{ }}

public class Test {{
    public Test() {{
        MyVo? value = null;
        var result = value.GetValueOrDefault(default);
    }}
}}
";

            await Run(source, []);
        }

        [Fact]
        public async Task Allow_parameterless_GetValueOrDefault_on_non_value_object_nullable_structs()
        {
            var source = @"namespace Whatever;

public class Test {
    public Test() {
        int? value = null;
        var result = value.GetValueOrDefault();
    }
}
";

            await VerifyCS.VerifyAnalyzerAsync(source);
        }

        private static IEnumerable<DiagnosticResult> WithDiagnostics(string code, DiagnosticSeverity severity,
            string arguments, params int[] locations)
        {
            foreach (var location in locations)
            {
                yield return VerifyCS.Diagnostic(code)
                    .WithSeverity(severity)
                    .WithLocation(location)
                    .WithArguments(arguments);
            }
        }

        private async Task Run(string source, IEnumerable<DiagnosticResult> expected)
        {
            var test = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { source },
                },

                CompilerDiagnostics = CompilerDiagnostics.Errors,
                ReferenceAssemblies = References.Net90AndOurs.Value,
            };

            test.ExpectedDiagnostics.AddRange(expected);

            await test.RunAsync();
        }
    }
}
