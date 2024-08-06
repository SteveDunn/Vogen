using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Vogen.Rules;
using VerifyCS = AnalyzerTests.Verifiers.CSharpCodeFixVerifier<Vogen.Rules.NormalizeInputMethodAnalyzer, Vogen.Rules.NormalizeInputMethodFixers.AddStaticToExistingMethodCodeFixProvider>;

namespace AnalyzerTests.NormalizationMethodAnalysis;

public class AddStaticToExistingNormalizeInputMethodAnalyzerTests
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
    private int {|#0:NormalizeInput|}(int input)
    {
        // this is my method that normalizes input
        var newValue = input * 2;
        return newValue;
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
    private static int NormalizeInput(int input)
    {
        // this is my method that normalizes input
        var newValue = input * 2;
        return newValue;
    }
}
""");

        var expectedDiagnostic = VerifyCS
            .Diagnostic(NormalizeInputMethodAnalyzer.RuleNotStatic)
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