using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.StringComparison;

[UsesVerify]
public class StringComparisonGenerationTests
{
    [Fact]
    public Task Generates_when_specified()
    {
        string source = $$"""
                               using System;
                               using Vogen;
                               namespace Whatever;
                               
                               [ValueObject(typeof(string), stringComparers: StringComparersGeneration.Generate)]
                               public partial class StringThing { }
                               """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
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

    [Fact]
    public Task Does_not_generate_the_equals_method_that_takes_a_StringComparison_when_not_a_string()
    {
        string source = $$"""
                               using System;
                               using Vogen;
                               namespace Whatever;
                               
                               [ValueObject(typeof(int))]
                               public partial class IntThing { }
                               """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }
}
