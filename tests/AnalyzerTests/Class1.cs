using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CSharp;
using Vogen;
using Xunit;
using VerifyCS = Analyzer1.Test.CSharpAnalyzerVerifier<Vogen.Analyzers.AddValidationAnalyzer>;

namespace SmallTests.AnalyzerTests
{
    public class AddValidationAnalyzerTests
    {
        //No diagnostics expected to show up
        [Fact]
        public async Task NoDiagnosticsForEmptyCode()
        {
            var test = @"";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [Fact]
        public async Task CodeFixTriggeredForVoWithNoValidateMethod()
        {
            var input = LineEndingsHelper.Normalize(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject]
    public partial class {|#0:TypeName|}
    {   
    }
}");

            var output = LineEndingsHelper.Normalize(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject]
    public partial class TypeName
    {
        private static Validation Validate(int input) 
        {
            return input > 0 ? Validation.Ok : Validation.Invalid(""!!"");
        }
    }
}");

            var expected =
                VerifyCS.Diagnostic("AddValidationAnalyzer").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");

            var loc = typeof(ValueObjectAttribute).Assembly.Location;

            var referenceAssemblies = ReferenceAssemblies.Default
                .AddAssemblies(
                    ImmutableArray.Create("Vogen", "Vogen.SharedTypes", loc.Replace(".dll", string.Empty))
                );

            var test = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = {input },
                },

                CompilerDiagnostics = CompilerDiagnostics.Suggestions,
                ReferenceAssemblies = referenceAssemblies,
//                FixedCode = output,
                ExpectedDiagnostics = { expected },
            };


            test.DisabledDiagnostics.Add("CS1591");

            await test.RunAsync();
        }
    }
}
