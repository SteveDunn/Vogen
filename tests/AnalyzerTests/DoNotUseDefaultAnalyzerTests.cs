using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Vogen;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotUseDefaultAnalyzer>;

namespace AnalyzerTests
{
    public class DoNotUseDefaultAnalyzerTests
    {
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
        public async Task Disallow_default_for_creating_value_objects(string type)
        {
            var source = $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public {type} MyVo {{ }}

public class Test {{
    public Test() {{
        MyVo c = {{|#0:default|}};
        MyVo c2 = {{|#1:default(MyVo)|}};
    }}
}}
";
            await Run(
                source,
                WithDiagnostics("V0G009", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }

        [Theory]
        [ClassData(typeof(Types))]
        public async Task Disallow_default_for_method_return_type(string type)
        {
            var source = $@"
using Vogen;
namespace Whatever;

[ValueObject]
public {type} MyVo {{ }}

public class Test {{
    public MyVo Get() => {{|#0:default|}};
    public MyVo Get2() => {{|#1:default(MyVo)|}};
}}
";

            await Run(
                source,
                WithDiagnostics("VOG009", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }

        [Theory]
        [ClassData(typeof(Types))]
        public async Task Disallow_default_from_local_function(string type)
        {
            var source = $@"
using Vogen;
namespace Whatever;

[ValueObject]
public {type} MyVo {{ }}

public class Test {{
    public Test() {{
        MyVo Get() => {{|#0:default|}};
        MyVo Get2() => {{|#1:default(MyVo)|}};
    }}
}}
";

            await Run(
                source,
                WithDiagnostics("VOG009", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }

        [Theory]
        [ClassData(typeof(Types))]
        public async Task Disallow_default_from_func(string type)
        {
            var source = $@"
using System;
using System.Threading.Tasks;
using Vogen;
namespace Whatever;

[ValueObject]
public {type} MyVo {{ }}

public class Test {{
        Func<MyVo> f = () =>  {{|#0:default(MyVo)|}};
        Func<MyVo> f2 = () =>  {{|#1:default|}};
        Func<int, int, MyVo, string, MyVo> f3 = (a,b,c,d) =>  {{|#2:default(MyVo)|}};
        Func<int, int, MyVo, string, MyVo> f4 = (a,b,c,d) =>  {{|#3:default|}};
        Func<int, int, MyVo, string, Task<MyVo>> f5 = async (a,b,c,d) => await Task.FromResult({{|#4:default(MyVo)|}});
}}
";

            await Run(
                source,
                WithDiagnostics("VOG009", DiagnosticSeverity.Error, "MyVo", 0, 1, 2, 3, 4));
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
