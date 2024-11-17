using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Vogen.Rules;
using VerifyCS = AnalyzerTests.Verifiers.CSharpCodeFixVerifier<Vogen.Rules.NormalizeInputMethodAnalyzer, Vogen.Rules.NormalizeInputMethodFixers.AddMethodCodeFixProvider>;

namespace AnalyzerTests.NormalizationMethodAnalysis;

public class NormalizeInputMethodAnalyzerTests
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
    public async Task Not_triggered_for_existing_pristine_method()
    {
        var input = LineEndingsHelper.Normalize(
            @"
using System;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject(typeof(int))]
    public partial class {|#0:TypeName|}
    {   
        private static int NormalizeInput(int input) => input;
    }
}");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { input }
            },

            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net90AndOurs.Value,
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }

    [Theory]
    [InlineData("ValueObject<int>", "int")]
    [InlineData("ValueObject", "int")]
    [InlineData("ValueObject(typeof(int))", "int")]
    [InlineData("ValueObject<string>", "string")]
    [InlineData("ValueObject(typeof(string))", "string")]
    public async Task Triggered_when_method_is_missing(string valueObjectDeclaration, string underlyingType)
    {
        var input = LineEndingsHelper.Normalize($$"""
using System;
using Vogen;

namespace ConsoleApplication1;

[{{valueObjectDeclaration}}]
public partial class {|#0:TypeName|}
{   
}
""");

        var expectedOutput = LineEndingsHelper.Normalize($$"""
 using System;
 using Vogen;

 namespace ConsoleApplication1;

 [{{valueObjectDeclaration}}]
 public partial class TypeName
 {
     private static {{underlyingType}} NormalizeInput({{underlyingType}} input)
     {
         // todo: normalize (sanitize) your input;
         return input;
     }
 }
 """);

        var expectedDiagnostic =
            VerifyCS.Diagnostic("AddNormalizeInputMethod").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { input }
            },

            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net90AndOurs.Value,
            FixedCode = expectedOutput,
            ExpectedDiagnostics = { expectedDiagnostic },
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }

    [Theory]
    [InlineData("ValueObject<int>", "string")]
    [InlineData("ValueObject", "string")]
    [InlineData("ValueObject(typeof(int))", "float")]
    [InlineData("ValueObject<string>", "int")]
    [InlineData("ValueObject(typeof(string))", "decimal")]
    public async Task Triggered_when_method_uses_takes_wrong_type(string valueObjectDeclaration, string underlyingType)
    {
        var input = LineEndingsHelper.Normalize($$"""
using System;
using Vogen;

namespace ConsoleApplication1;

[{{valueObjectDeclaration}}]
public partial class TypeName
{   
    private static {{underlyingType}} {|#0:NormalizeInput|}({{underlyingType}} input) => default!;
}
""");

        var expectedDiagnostic =
            VerifyCS.Diagnostic(NormalizeInputMethodAnalyzer.RuleWrongInputType).WithSeverity(DiagnosticSeverity.Info).WithLocation(0)
                .WithArguments("TypeName");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { input }
            },

            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net90AndOurs.Value,
            ExpectedDiagnostics = { expectedDiagnostic },
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }
}