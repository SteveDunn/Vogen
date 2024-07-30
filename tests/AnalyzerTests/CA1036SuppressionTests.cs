using System.Collections.Immutable;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Shared;
using Vogen;
using Vogen.Suppressors;

namespace AnalyzerTests;

public class CA1036SuppressionTests
{
    [Fact]
    public async Task Suppressed_for_value_object()
    {
        var source = $$"""
using System;
using Vogen;

namespace Whatever;

[ValueObject<int>]
public partial class CustomerId : IComparable<CustomerId>
{
}
""";

        (ImmutableArray<Diagnostic> Diagnostics, SyntaxTree[] GeneratedSource) r = 
            await CreateBuilder(source).GetGeneratedOutput<ValueObjectGenerator>(ignoreInitialCompilationErrors: false);
        
            r.Diagnostics.Should().HaveCount(0);
    }

    [Fact]
    public async Task Does_not_suppress_other_types()
    {
        var source = $$"""
using System;
using Vogen;

namespace Whatever;

public partial class CustomerId : IComparable<CustomerId>
{
	public int CompareTo(CustomerId other)
	{
		return 1;
	}
}
""";

        (ImmutableArray<Diagnostic> Diagnostics, SyntaxTree[] GeneratedSource) r = 
            await CreateBuilder(source).GetGeneratedOutput<ValueObjectGenerator>(ignoreInitialCompilationErrors: false);
        
            r.Diagnostics.Should().HaveCount(1);
            r.Diagnostics[0].Id.Should().Be("CA1036");
    }

    private static ProjectBuilder CreateBuilder(string source) => new ProjectBuilder()
        .WithUserSource(source)
        .WithAnalyzer<CA1036ComparisonSuppressor>()
        .WithTargetFramework(TargetFramework.Net8_0)
        .WithMicrosoftCodeAnalysisNetAnalyzers("CA1036");
}