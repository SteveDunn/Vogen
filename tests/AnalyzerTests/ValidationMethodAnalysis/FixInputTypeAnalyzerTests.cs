using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using SmallTests.AnalyzerTests;
using Vogen.Rules;
using VerifyCS = AnalyzerTests.Verifiers.CSharpCodeFixVerifier<Vogen.Rules.ValidationMethodAnalyzer, Vogen.Rules.ValidateMethodFixers.FixInputTypeCodeFixProvider>;

namespace AnalyzerTests.ValidationMethodAnalysis;

public class FixInputTypeTests
{
    [Fact]
    public async Task Triggered_when_input_is_string_but_primitive_is_int()
    {
        var input = LineEndingsHelper.Normalize("""
using System;
using Vogen;

namespace ConsoleApplication1;

[ValueObject]
public partial class TypeName
{   
    private static Validation {|#0:Validate|}(string input)
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
            .Diagnostic(ValidationMethodAnalyzer.RuleWrongInputType)
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

    [Fact]
    public async Task Triggered_when_input_is_int_but_primitive_is_string()
    {
        var input = LineEndingsHelper.Normalize("""
using System;
using Vogen;

namespace ConsoleApplication1;

[ValueObject<string>]
public partial class TypeName
{   
    private static Validation {|#0:Validate|}(int input)
    {
        return Validation.Invalid("oh no!");
    }
}
""");

        var expectedOutput = LineEndingsHelper.Normalize("""
using System;
using Vogen;

namespace ConsoleApplication1;

[ValueObject<string>]
public partial class TypeName
{   
    private static Validation Validate(string input)
    {
        return Validation.Invalid("oh no!");
    }
}
""");

        var expectedDiagnostic = VerifyCS
            .Diagnostic(ValidationMethodAnalyzer.RuleWrongInputType)
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

    [Theory]
    [InlineData("int", "string")]
    [InlineData("string", "int")]
    [InlineData("bool", "string")]
    [InlineData("string", "bool")]
    [InlineData("float", "int")]
    [InlineData("int", "float")]
    [InlineData("decimal", "float")]
    [InlineData("float", "decimal")]
    public async Task Triggered_for_various_mismatches(string primitiveType, string methodType)
    {
        var input = LineEndingsHelper.Normalize($$"""
using System;
using Vogen;

namespace ConsoleApplication1;

[ValueObject<{{primitiveType}}>]
public partial class TypeName
{   
    private static Validation {|#0:Validate|}({{methodType}} input)
    {
        return Validation.Invalid("oh no!");
    }
}
""");

        var expectedOutput = LineEndingsHelper.Normalize($$"""
using System;
using Vogen;

namespace ConsoleApplication1;

[ValueObject<{{primitiveType}}>]
public partial class TypeName
{   
    private static Validation Validate({{primitiveType}} input)
    {
        return Validation.Invalid("oh no!");
    }
}
""");

        var expectedDiagnostic = VerifyCS
            .Diagnostic(ValidationMethodAnalyzer.RuleWrongInputType)
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