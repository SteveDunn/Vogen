using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.Comparable;

[UsesVerify]
public class EquuatableGenerationTests
{
    [Fact]
    public Task Using_underlying_uses_int_comparison()
    {
        string source = $$"""
                               using System;
                               using Vogen;
                               namespace Whatever;
                               
                               [ValueObject(typeof(string), stringComparison: StringComparison.OrdinalIgnoreCase)]
                               public partial class Name
                               {
                                   private static Validation Validate(string value)
                                   {
                                       if (value.Length > 0)
                                           return Validation.Ok;
                                       
                                       return Validation.Invalid("name cannot be empty");
                                   }
                               }
                               """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Using_omit_does_not_generate_comparable_code()
    {
        string source = $$"""
                               using Vogen;
                               namespace Whatever;                               
                               [ValueObject(comparison: ComparisonGeneration.Omit)]
                               public partial class MyVo { }
                               """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }
}
