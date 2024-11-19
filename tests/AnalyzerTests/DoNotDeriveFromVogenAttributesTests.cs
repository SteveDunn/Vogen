using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotDeriveFromVogenAttributesAnalyzer>;

namespace AnalyzerTests
{
    public class DoNotDeriveFromVogenAttributesTests
    {
        //No diagnostics expected to show up
        [Fact]
        public async Task NoDiagnosticsForEmptyCode()
        {
            var test = @"";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task Disallow_generic()
        {
            var source = $$"""
                           using Vogen;
                           namespace Whatever;

                           public class {|#0:IntVoAttribute|} : ValueObjectAttribute<int>
                           {
                           }

                           [IntVoAttribute]
                           public partial struct MyVo { }
                           """;
            await Run(
                source,
                WithDiagnostics("VOG026", DiagnosticSeverity.Warning, "IntVoAttribute", 0));
        }

        [Fact]
        public async Task Disallow_non_generic()
        {
            var source = $$"""
                           using Vogen;
                           namespace Whatever;

                           public class {|#0:IntVoAttribute|} : ValueObjectAttribute
                           {
                           }

                           [IntVoAttribute]
                           public partial struct MyVo { }
                           """;
            await Run(
                source,
                WithDiagnostics("VOG026", DiagnosticSeverity.Warning, "IntVoAttribute", 0));
        }

        [Fact]
        public async Task Disallow_deriving_from_defaults_attribute()
        {
            var source = $$"""
                           using Vogen;
                           namespace Whatever;

                           public class {|#0:MyDefaultsAttribute|} : VogenDefaultsAttribute
                           {
                           }
                           """;
            await Run(
                source,
                WithDiagnostics("VOG026", DiagnosticSeverity.Warning, "MyDefaultsAttribute", 0));
        }

        [Fact]
        public async Task Disallow_deep_derivation()
        {
            var source = $$"""
                           using System;
                           using Vogen;
                           namespace Whatever;
                           
                           public class {|#0:A1Attribute|} : ValueObjectAttribute { }
                           public class {|#1:A2Attribute|} : A1Attribute { }
                           public class {|#2:A3Attribute|} : A2Attribute { }
                           public class {|#3:A4Attribute|} : A3Attribute { }

                           public class {|#4:IntVoAttribute|} : A4Attribute
                           {
                           }

                           [IntVoAttribute]
                           public partial struct MyVo { }
                           """;
            await Run(
                source,
                WithDiagnostics("VOG026", DiagnosticSeverity.Warning, ("A1Attribute", 0),("A2Attribute", 1),("A3Attribute", 2),("A4Attribute", 3),("IntVoAttribute", 4)));
        }

        [Fact]
        public async Task Disallow_when_also_deriving_from_interfaces()
        {
            var source = $$"""
                           using System;
                           using Vogen;
                           namespace Whatever;
                           
                           public interface I1 {};
                           public interface I2 {};
                           
                           public class {|#0:A1Attribute|} : ValueObjectAttribute, I1, I2 { }

                           public class {|#1:IntVoAttribute|} : A1Attribute
                           {
                           }

                           [IntVoAttribute]
                           public partial struct MyVo { }
                           """;
            await Run(
                source,
                WithDiagnostics("VOG026", DiagnosticSeverity.Warning, ("A1Attribute", 0), ("IntVoAttribute", 1)));
        }

        [Fact]
        public async Task Allow_non_Vogen_attributes()
        {
            var source = $$"""
                           using System;
                           using Vogen;
                           namespace Whatever;
                           
                           public class MyAttr : Attribute { }
                           
                           public class MyDerivedAttr : MyAttr { }

                           [ValueObjectAttribute<int>]
                           public partial struct MyVo { }
                           """;
            
            
            await Run(source, Enumerable.Empty<DiagnosticResult>());
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

        private static IEnumerable<DiagnosticResult> WithDiagnostics(string code, DiagnosticSeverity severity,
            params (string, int)[] locations)
        {
            foreach (var location in locations)
            {
                yield return VerifyCS.Diagnostic(code).WithSeverity(severity).WithLocation(location.Item2).WithArguments(location.Item1);
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
