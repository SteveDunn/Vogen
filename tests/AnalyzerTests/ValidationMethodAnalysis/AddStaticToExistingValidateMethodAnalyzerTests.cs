using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Vogen.Rules;
using VerifyCS = AnalyzerTests.Verifiers.CSharpCodeFixVerifier<Vogen.Rules.ValidationMethodAnalyzer, Vogen.Rules.ValidateMethodFixers.AddStaticToExistingMethodCodeFixProvider>;

namespace AnalyzerTests.ValidationMethodAnalysis;

public class AddStaticToExistingValidateMethodAnalyzerTests
{
    [Fact]
    public async Task Triggered_for_non_static_method()
    {
        var input = LineEndingsHelper.Normalize("""
using System;
using Vogen;

namespace ConsoleApplication1;

[ValueObject]
public partial class TypeName
{   
    private Validation {|#0:Validate|}(int input)
    {
        return Validation.Invalid("oh no!");
    }
}
""");

        var expectedOutput = LineEndingsHelper.Normalize("""
using System;
using Vogen;

namespace ConsoleApplication1;

[ValueObject]
public partial class TypeName
{   
    private static Validation Validate(int input)
    {
        return Validation.Invalid("oh no!");
    }
}
""");

        var expectedDiagnostic = VerifyCS
            .Diagnostic(ValidationMethodAnalyzer.RuleNotStatic)
            .WithSeverity(DiagnosticSeverity.Info)
            .WithLocation(0)
            .WithArguments("TypeName");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { input }
            },

            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net80AndOurs.Value,
            FixedCode = expectedOutput,
            ExpectedDiagnostics = { expectedDiagnostic },
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }
}