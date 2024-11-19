using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using System.Threading.Tasks;
using VerifyCS = AnalyzerTests.Verifiers.CSharpCodeFixVerifier<Vogen.Rules.PreferReadonlyStructAnalyzer, Vogen.Rules.MakeStructReadonlyFixers.MakeStructReadonlyCodeFixProvider>;

namespace AnalyzerTests;

public class PreferReadonlyStructsAnalyzerTests
{
    [Theory]
    [InlineData("class")]
    [InlineData("record")]
    [InlineData("record class")]
    public async Task Does_not_trigger_if_not_struct(string type)
    {
        var source = $$"""
                       using Vogen;

                       namespace Whatever;

                       [ValueObject<int>]
                       public partial {{type}} CustomerId { }
                       """;

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { source }
            },
            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net90AndOurs.Value,
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }

    [Theory]
    [InlineData("struct")]
    [InlineData("record struct")]
    public async Task Does_not_trigger_when_struct_is_readonly(string type)
    {
        var source = $$"""
                       using Vogen;

                       namespace Whatever;

                       [ValueObject<int>]
                       public readonly partial {{type}} CustomerId { }
                       """;

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { source }
            },
            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net90AndOurs.Value,
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }

    [Theory]
    [InlineData("struct", "")]
    [InlineData("record struct", "")]
    [InlineData("struct", "<int>")]
    [InlineData("record struct", "<int>")]
    [InlineData("struct", "<string>")]
    [InlineData("record struct", "<string>")]
    public async Task Triggers_when_struct_is_not_partial(string modifier, string genericType)
    {
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject{{genericType}}]
                       public partial {{modifier}} {|#0:DocumentId|} { }
                       """;

        var fixedCode = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject{{genericType}}]
                       public readonly partial {{modifier}} {|#0:DocumentId|} { }
                       """;

        var expectedDiagnostic = VerifyCS
            .Diagnostic("VOG033")
            .WithSeverity(DiagnosticSeverity.Info)
            .WithLocation(0)
            .WithArguments("DocumentId");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { source }
            },
            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net90AndOurs.Value,
            ExpectedDiagnostics = { expectedDiagnostic },
            FixedCode = fixedCode
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }
}
