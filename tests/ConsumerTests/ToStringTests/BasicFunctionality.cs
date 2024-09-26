using Vogen.Tests.Types;

namespace ConsumerTests.ToStringTests;

public class BasicFunctionality
{
    [Fact]
    public void Defaults_to_empty_string_if_primitive_returns_null()
    {
        VoWrappingNaughtyPrimitive v = VoWrappingNaughtyPrimitive.From(new NaughtyPrimitive());
        v.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void Does_not_use_ToString_from_record()
    {
        MyRecordVo record = MyRecordVo.From(123);
        record.ToString().Should().Be("123");
    }

    [Fact]
    public void Generates_correct_nullability_for_when_vo_record_derives_from_a_record()
    {
        MyDerivedRecordVo record = MyDerivedRecordVo.From(123);
        record.ToString().Should().Be("123");
    }
    
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

[ValueObject<NaughtyPrimitive>]
public partial class VoWrappingNaughtyPrimitive
{
}

public class NaughtyPrimitive
{
    public override string? ToString() => null;
}

[ValueObject]
public partial record MyRecordVo;

public record BaseRecord;

[ValueObject]
public partial record MyDerivedRecordVo : BaseRecord;
