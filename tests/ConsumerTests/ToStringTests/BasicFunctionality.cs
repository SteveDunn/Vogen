using Vogen.Tests.Types;

namespace ConsumerTests.ToStringTests;

public class BasicFunctionality
{
    [SkippableIfBuiltWithNoValidationFlagFact]
    public void ToString_does_not_throw_for_something_uninitialized()
    {
#pragma warning disable VOG010
        Age age = new Age();
        age.ToString().Should().Be("[UNINITIALIZED]");
#pragma warning restore VOG010
    }

    [SkippableIfNotBuiltWithNoValidationFlagFact]
    public void ToString_does_not_show_uninitialized_when_no_validation_is_on()
    {
#pragma warning disable VOG010
        Age age = new Age();
        age.ToString().Should().Be("0");
#pragma warning restore VOG010
    }

    [Fact]
    public void With_collections_it_uses_the_underlying_types_ToString() => FileHash.From(new Hash<byte>([1, 2, 3])).ToString().Should().Be("Vogen.Tests.Types.Hash`1[System.Byte]");

    [Fact]
    public void ToString_uses_generated_method()
    {
        Age.From(18).ToString().Should().Be("18");
        Age.From(100).ToString().Should().Be("100");
        Age.From(1_000).ToString().Should().Be("1000");

        Name.From("fred").ToString().Should().Be("fred");
        Name.From("barney").ToString().Should().Be("barney");
        Name.From("wilma").ToString().Should().Be("wilma");
    }
}