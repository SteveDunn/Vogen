using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.ExplicitlySpecifyTypeInValueObjectAttributeAnalyzer>;

namespace AnalyzerTests
{
    public class ExplicitlySpecifyTypeInValueObjectAttributeAnalyzerTests
    {
        //No diagnostics expected to show up
        [Fact]
        public async Task NoDiagnosticsForEmptyCode()
        {
            var test = @"";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task Not_triggered_when_type_is_not_specified_and_no_global_config()
        {
            var source = $$"""
                           using Vogen;
                           namespace Whatever;

                           [ValueObject]
                           public partial struct {|#0:MyVo|} { }
                           """;
            await Run(source, []);
        }

        [Fact]
        public async Task Triggered_when_type_is_not_specified_and_global_config_says_it_should_be()
        {
            var source = $$"""
                           using Vogen;

                           [assembly: VogenDefaults(explicitlySpecifyTypeInValueObject: true)]

                           namespace Whatever;

                           [ValueObject]
                           public partial struct {|#0:MyVo|} { }
                           """;
            await Run(
                source,
                WithDiagnostics("VOG029", DiagnosticSeverity.Error, "MyVo", 0));
        }

        [Fact]
        public async Task Not_triggered_when_type_is_not_specified_and_global_config_says_it_should_not_be()
        {
            var source = $$"""
                           using Vogen;

                           [assembly: VogenDefaults(explicitlySpecifyTypeInValueObject: false)]

                           namespace Whatever;

                           [ValueObject]
                           public partial struct {|#0:MyVo|} { }
                           """;
            await Run(source, []);
        }

        [Fact]
        public async Task Not_triggered_for_generic_attribute()
        {
            var source = $$"""
                           using Vogen;

                           [assembly: VogenDefaults(explicitlySpecifyTypeInValueObject: true)]

                           namespace Whatever;

                           [ValueObject<int>]
                           public partial struct {|#0:MyVo|} { }
                           """;
            await Run(source, []);
        }

        [Fact]
        public async Task Not_triggered_when_type_is_not_first_parameter()
        {
            var source = $$"""
                           using Vogen;

                           [assembly: VogenDefaults(explicitlySpecifyTypeInValueObject: true)]

                           namespace Whatever;

                           [ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
                           public partial struct {|#0:MyVo|} { }
                           """;
            await Run(source, []);
        }
        
        [Fact]
        public async Task Not_triggered_when_type_is_not_specified_and_global_config_is_empty()
        {
            var source = $$"""
                           using Vogen;

                           namespace Whatever;

                           [ValueObject(conversions: Conversions.None)]
                           public partial struct {|#0:MyVo|} { }
                           """;
            await Run(source, []);
        }

        [Fact]
        public async Task Ignores_malformed_arguments()
        {
            var source = $$"""
                           using Vogen;

                           namespace Whatever;

                           [ValueObject({|#0:thisIsNotAParameter|}: Conversions.None)]
                           public partial struct MyVo { }
                           """;

            await Run(
                source,
                [DiagnosticResult.CompilerError("CS1739").WithArguments("ValueObjectAttribute", "thisIsNotAParameter").WithLocation(0)]);
        }


        private static IEnumerable<DiagnosticResult> WithDiagnostics(string code, 
            DiagnosticSeverity severity,
            string arguments, 
            params int[] locations)
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
