using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.StringComparison;

[UsesVerify]
public class StringComparisonGenerationTests
{
    [Fact]
    public Task Generates_with_OrdinalIgnoreCase()
    {
        string source = $$"""
                               using System;
                               using Vogen;
                               namespace Whatever;
                               
                               [ValueObject(typeof(string), stringComparison: StringComparisonGeneration.OrdinalIgnoreCase)]
                               public partial class StringThing { }
                               """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Theory]
    [InlineData("CurrentCulture")]
    [InlineData("CurrentCultureIgnoreCase")]
    [InlineData("InvariantCulture")]
    [InlineData("InvariantCultureIgnoreCase")]
    [InlineData("Ordinal")]
    [InlineData("OrdinalIgnoreCase")]
    public Task Generates_with_different_comparisons(string comparison)
    {
        string source = $$"""
                               using System;
                               using Vogen;
                               namespace Whatever;
                               
                               [ValueObject(typeof(string), stringComparison: StringComparisonGeneration.{{comparison}})]
                               public partial class StringThing { }
                               """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(comparison)))
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Defaults_to_not_generating_anything()
    {
        string source = $$"""
                               using System;
                               using Vogen;
                               namespace Whatever;
                               
                               [ValueObject(typeof(string))]
                               public partial class StringThing { }
                               """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }
}
