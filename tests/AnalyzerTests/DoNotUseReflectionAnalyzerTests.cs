using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotUseReflectionAnalyzer>;

namespace AnalyzerTests
{
    public class DoNotUseReflectionAnalyzerTests
    {
        private class Types : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return ["partial class"];
                yield return ["partial struct"];
                yield return ["readonly partial struct"];
                yield return ["partial record class"];
                yield return ["partial record struct"];
                yield return ["readonly partial record struct"];
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
                WithDiagnostics("VOG025", DiagnosticSeverity.Error, 0));
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
                WithDiagnostics("VOG025", DiagnosticSeverity.Error, 0));
        }

        private static IEnumerable<DiagnosticResult> WithDiagnostics(string code, DiagnosticSeverity severity, params int[] locations)
        {
            foreach (var location in locations)
            {
                yield return VerifyCS.Diagnostic(code).WithSeverity(severity).WithLocation(location)
                    .WithArguments("MyVo");
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
                ReferenceAssemblies = References.Net80AndOurs.Value,
            };

            test.ExpectedDiagnostics.AddRange(expected);

            await test.RunAsync();
        }
    }
}
