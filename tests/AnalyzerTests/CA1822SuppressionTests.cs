using System.Collections.Immutable;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Shared;
using Vogen;
using Vogen.Diagnostics;
using Vogen.Suppressors;

namespace AnalyzerTests;

public class CA1822SupressionTests
{
    [Fact]
    public async Task Suppressed_for_both_Validate_and_NormalizeInput()
    {
        var source = $$"""
using Vogen;

namespace Whatever;

[ValueObject<int>]
public partial class CustomerId 
{
     private Validation Validate(int value) => Validation.Ok; 
     private int NormalizeInput(int value) => value; 
}
""";

        (ImmutableArray<Diagnostic> Diagnostics, SyntaxTree[] GeneratedSource) r = 
            await CreateBuilder(source).GetGeneratedOutput<ValueObjectGenerator>(ignoreInitialCompilationErrors: false);
        
            r.Diagnostics.Should().HaveCount(2);
            r.Diagnostics.Should().Contain(d => d.Id == RuleIdentifiers.ValidateMethodMustBeStatic);
            r.Diagnostics.Should().Contain(d => d.Id == RuleIdentifiers.NormalizeInputMethodMustBeStatic);
    }

    [Fact]
    public async Task Does_not_suppress_other_methods()
    {
        var source = $$"""
using Vogen;

namespace Whatever;

[ValueObject<int>]
public partial class CustomerId 
{
     private Validation UsersMethod1(int value) => Validation.Ok; 
     private int UsersMethod2(int value) => value; 
}
""";

        (ImmutableArray<Diagnostic> Diagnostics, SyntaxTree[] GeneratedSource) r = 
            await CreateBuilder(source).GetGeneratedOutput<ValueObjectGenerator>(ignoreInitialCompilationErrors: false);
        
            r.Diagnostics.Should().HaveCount(2);
            r.Diagnostics[0].Id.Should().Be("CA1822");
            r.Diagnostics[1].Id.Should().Be("CA1822");
    }

    private static ProjectBuilder CreateBuilder(string source) => new ProjectBuilder()
        .WithUserSource(source)
        .WithAnalyzer<CA1822DecoratedMethodSuppressor>()
        .WithTargetFramework(TargetFramework.Net8_0)
        .WithMicrosoftCodeAnalysisNetAnalyzers("CA1822");
}