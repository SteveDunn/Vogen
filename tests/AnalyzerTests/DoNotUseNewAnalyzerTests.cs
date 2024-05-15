using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using Vogen;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotUseNewAnalyzer>;
// ReSharper disable CoVariantArrayConversion

namespace AnalyzerTests
{
    public class DoNotUseNewAnalyzerTests
    {
        // A pattern for 'placeholders' for errors. These are stripped out when running tests
        // that require both the user source and generated source.
        private static readonly Regex _placeholderPattern = new(@"{\|#\d+:", RegexOptions.Compiled);

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

        private class Classes : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return ["partial class"];
                yield return ["partial record class"];
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
#if NET7_0_OR_GREATER
        [Theory(DisplayName = "Bug https://github.com/SteveDunn/Vogen/issues/389")]
        [ClassData(typeof(Types))]
        public async Task Disallow_new_for_creating_value_objects_using_generic_attribute(string type)
        {
            var source = $@"using Vogen;
namespace Whatever;

[ValueObject<int>()]
public {type} MyVo {{ }}

public class Test {{
    public Test() {{
        var c = {{|#0:new MyVo()|}};
        MyVo c2 = {{|#1:new()|}};
    }}
}}
";
            await Run(
                source,
                WithDiagnostics("VOG010", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }
#endif
        [Theory]
        [ClassData(typeof(Types))]
        public async Task Disallow_new_for_creating_value_objects(string type)
        {
            var source = $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public {type} MyVo {{ }}

public class Test {{
    public Test() {{
        var c = {{|#0:new MyVo()|}};
        MyVo c2 = {{|#1:new()|}};
    }}
}}
";
            await Run(
                source,
                WithDiagnostics("VOG010", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }

        [Theory]
        [ClassData(typeof(Types))]
        public async Task Disallow_new_for_method_return_type(string type)
        {
            var source = $@"
using Vogen;
namespace Whatever;

[ValueObject]
public {type} MyVo {{ }}

public class Test {{
    public MyVo Get() => {{|#0:new MyVo()|}};
    public MyVo Get2() => {{|#1:new MyVo()|}};
}}
";

            await Run(
                source,
                WithDiagnostics("VOG010", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }

        [Theory]
        [ClassData(typeof(Types))]
        public async Task Disallow_new_from_local_function(string type)
        {
            var source = $$"""

                           using Vogen;
                           namespace Whatever;

                           [ValueObject]
                           public {{type}} MyVo { }

                           public class Test {
                               public Test() {
                                   MyVo Get() => {|#0:new MyVo()|};
                                   MyVo Get2() => {|#1:new()|};
                               }
                           }

                           """;

            await Run(
                source,
                WithDiagnostics("VOG010", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }

        [Theory]
        [ClassData(typeof(Types))]
        public async Task Allow_as_public_static_field_in_a_VO(string type)
        {
            var userSource = $$"""

                           using Vogen;
                           namespace Whatever
                           {
                               [ValueObject]
                               public {{type}} MyVo { 
                                    public static MyVo Unspecified = new MyVo(-1);
                               }
                           }

                           """;
            string[] sources = CombineUserAndGeneratedSource(userSource);

            await Run(sources, Enumerable.Empty<DiagnosticResult>());
        }

        [Theory]
        [ClassData(typeof(Types))]
        public async Task Disallow_as_private_static_field_in_a_VO(string type)
        {
            var userSource = $$"""

                           using Vogen;
                           namespace Whatever
                           {
                               [ValueObject]
                               public {{type}} MyVo { 
                                    private static MyVo Unspecified1 = {|#0:new MyVo(-1)|};
                                    private static MyVo Unspecified2 = {|#1:new(-1)|};
                               }
                           }

                           """;

            string[] sources = CombineUserAndGeneratedSource(userSource);
            
            await Run(sources, WithDiagnostics("VOG027", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }

        [Theory]
        [ClassData(typeof(Classes))]
        public async Task Disallow_as_non_static_field_in_a_VO(string type)
        {
            var userSource = $$"""

                           using Vogen;
                           namespace Whatever
                           {
                               [ValueObject]
                               public {{type}} MyVo { 
                                    public MyVo Unspecified1 = {|#0:new MyVo(-1)|};
                                    public MyVo Unspecified2 = {|#1:new(-1)|};
                               }
                           }

                           """;

            string[] sources = CombineUserAndGeneratedSource(userSource);
            
            await Run(sources, WithDiagnostics("VOG027", DiagnosticSeverity.Error, "MyVo", 0, 1));
        }

        private static string[] CombineUserAndGeneratedSource(string userSource)
        {
            PortableExecutableReference peReference = MetadataReference.CreateFromFile(typeof(ValueObjectAttribute).Assembly.Location);

            var strippedSource = _placeholderPattern.Replace(userSource, string.Empty).Replace("|}", string.Empty);
            
            (ImmutableArray<Diagnostic> Diagnostics, SyntaxTree[] GeneratedSources) output = new ProjectBuilder()
                .WithUserSource(strippedSource)
                .WithTargetFramework(TargetFramework.Net8_0)
                .GetGeneratedOutput<ValueObjectGenerator>(ignoreInitialCompilationErrors: true, peReference);

            if (output.Diagnostics.Length > 0)
            {
                throw new AssertFailedException(
                    $"""
                     Expected user source to be error and generated code to be free from errors:
                                                                     User source: {userSource}
                                                                     Errors: {string.Join(",", output.Diagnostics.Select(d => d.ToString()))}
                     """);
            }

            return [userSource, ..output.GeneratedSources.Select(o => o.ToString())];
        }

        [Theory]
        [ClassData(typeof(Types))]
        public async Task Disallow_new_from_func(string type)
        {
            var source = $@"
using System;
using System.Threading.Tasks;
using Vogen;
namespace Whatever;

[ValueObject]
public {type} MyVo {{ }}

public class Test {{
        Func<MyVo> f = () =>  {{|#0:new MyVo()|}};
        Func<MyVo> f2 = () =>  {{|#1:new()|}};
        Func<int, int, MyVo, string, MyVo> f3 = (a,b,c,d) =>  {{|#2:new MyVo()|}};
        Func<int, int, MyVo, string, MyVo> f4 = (a,b,c,d) =>  {{|#3:new()|}};
        Func<int, int, MyVo, string, Task<MyVo>> f5 = async (a,b,c,d) => await Task.FromResult({{|#4:new MyVo()|}});
}}
";

            await Run(
                source,
                WithDiagnostics("VOG010", DiagnosticSeverity.Error, "MyVo", 0, 1, 2, 3, 4));
        }

        [Fact(DisplayName = "Bug https://github.com/SteveDunn/Vogen/issues/182")]
        public async Task Analyzer_false_position_for_implicit_new_in_array_initializer()
        {
            var source = @"using System;
using System.Threading.Tasks;
using Vogen;

public class Test {
    Vo c = Create(new Object[]
    {
        // This call is the issue
        new()
    });

    static Vo Create(Object[] normalObject)
    {
        throw null; // we don't actually generate the VO in this test
    }
}

[ValueObject(typeof(int))]
public partial class Vo { }";

            await Run(
                source,
                Enumerable.Empty<DiagnosticResult>());
        }

        private static IEnumerable<DiagnosticResult> WithDiagnostics(string code, DiagnosticSeverity severity,
            string arguments, params int[] locations)
        {
            foreach (var location in locations)
            {
                yield return VerifyCS.Diagnostic(code).WithSeverity(severity).WithLocation(location)
                    .WithArguments(arguments);
            }
        }

        private async Task Run(string source, IEnumerable<DiagnosticResult> expected) => await Run([source], expected);

        private async Task Run(string[] sources, IEnumerable<DiagnosticResult> expected)
        {
            var test = new VerifyCS.Test
            {
                CompilerDiagnostics = CompilerDiagnostics.Errors,
                ReferenceAssemblies = References.Net80AndOurs.Value,
            };

            foreach (var eachSource in sources)
            {
                test.TestState.Sources.Add(eachSource);
            }

            test.ExpectedDiagnostics.AddRange(expected);

            await test.RunAsync();
        }
    }
}
