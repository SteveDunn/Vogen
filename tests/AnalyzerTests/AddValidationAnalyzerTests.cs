using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using SmallTests.AnalyzerTests;
using Vogen;
using VerifyCS = AnalyzerTests.Verifiers.CSharpCodeFixVerifier<Vogen.Rules.AddValidationAnalyzer, Vogen.Rules.AddValidationAnalyzerCodeFixProvider>;

namespace AnalyzerTests
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
    [ValueObject(typeof(int))]
    public partial class {|#0:TypeName|}
    {   
    }
}");

            var expectedOutput = LineEndingsHelper.Normalize(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject(typeof(int))]
    public partial class TypeName
    {
        private static Validation Validate(int input)
        {
            bool isValid = true; // todo: your validation
            return isValid ? Validation.Ok : Validation.Invalid(""[todo: describe the validation]"");
        }
    }
}");

            var expectedDiagnostic =
                VerifyCS.Diagnostic("AddValidationMethod").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");

            var loc = typeof(ValueObjectAttribute).Assembly.Location;

            // var referenceAssemblies = ReferenceAssemblies.Default
            //     .AddAssemblies(
            //         ImmutableArray.Create("Vogen", "Vogen.SharedTypes", loc.Replace(".dll", string.Empty))
            var referenceAssemblies = ReferenceAssemblies.NetStandard.NetStandard20.AddAssemblies(
                ImmutableArray.Create("Vogen", "Vogen.SharedTypes", loc.Replace(".dll", string.Empty)));

            var test = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { input },
                },

                CompilerDiagnostics = CompilerDiagnostics.Suggestions,
                ReferenceAssemblies = referenceAssemblies,
                FixedCode = expectedOutput,
                ExpectedDiagnostics = { expectedDiagnostic },
            };

            test.DisabledDiagnostics.Add("CS1591");

            await test.RunAsync();
        }


#if NET7_0_OR_GREATER
        [Fact]
        public async Task Generic_CodeFixTriggeredForVoWithNoValidateMethod()
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
    [ValueObject<int>]
    public partial class {|#0:TypeName|}
    {   
    }
}");

            var expectedOutput = LineEndingsHelper.Normalize(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject<int>]
    public partial class TypeName
    {
        private static Validation Validate(int input)
        {
            bool isValid = true; // todo: your validation
            return isValid ? Validation.Ok : Validation.Invalid(""[todo: describe the validation]"");
        }
    }
}");

            var expectedDiagnostic =
                VerifyCS.Diagnostic("AddValidationMethod").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");

            var loc = typeof(ValueObjectAttribute).Assembly.Location;

            var referenceAssemblies = ReferenceAssemblies.Net.Net60
                .AddAssemblies(
                    ImmutableArray.Create("Vogen", "Vogen.SharedTypes", loc.Replace(".dll", string.Empty))
                );

            var test = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { input },
                },

                CompilerDiagnostics = CompilerDiagnostics.Suggestions,
                ReferenceAssemblies = referenceAssemblies,
                FixedCode = expectedOutput,
                ExpectedDiagnostics = { expectedDiagnostic },
            };

            test.DisabledDiagnostics.Add("CS1591");

            await test.RunAsync();
        }
#endif

    }
}
