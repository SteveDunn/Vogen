using System.Globalization;
using Vogen.Tests.Types;

namespace ConsumerTests.ParsingAndFormattingTests;

#pragma warning disable CS8625, CS8602
public class TryFormatTests
{
    [Fact]
    public void TryFormat_delegates_to_primitive()
    {
        Span<char> s = stackalloc char[8];
        Age.From(128).TryFormat(s, out int written, "x8", CultureInfo.InvariantCulture).Should().BeTrue();
        written.Should().Be(8);
        s.ToString().Should().Be("00000080");

        MyDecimal d = MyDecimal.From(1.23m);

        $"{d:0.000}".Should().Be("1.230");
        d.ToString("0.00", new CultureInfo("fr")).Should().Be("1,23");
        $"{d:0.000}".Should().Be("1.230");

        Span<char> s2 = stackalloc char[8];
        MyDecimal.From(1.23m).TryFormat(s2, out written, "000.00", CultureInfo.InvariantCulture).Should().BeTrue();
        written.Should().Be(6);
        s2[..written].ToString().Should().Be("001.23");
    }

    /// <summary>
    /// See https://github.com/SteveDunn/Vogen/issues/710
    /// An uninitialized VO, when used in a formatted string, used to hang because it returned
    /// false from `TryFormat`. False is only ever used in the provided span wasn't big enough, which
    /// caused the runtime to increase the size and retry. 
    /// </summary>
    [SkippableIfBuiltWithNoValidationFlagFact]
    public void Uninitialized_vos_output_nothing()
    {
        var vo = new TestContainer();
        vo.ToString().Should().Be("ID1:'' - ID2:'00000000-0000-0000-0000-000000000000'");
    }

    [SkippableIfNotBuiltWithNoValidationFlagFact]
    public void Uninitialized_vos_output_default_value_when_validation_is_turned_off()
    {
        var vo = new TestContainer();
        vo.ToString().Should().Be("ID1:'' - ID2:'00000000-0000-0000-0000-000000000000'");
    }
    
    public class TestContainer
    {
        public MyId Id1 { get; set; }
        public MyId Id2 { get; set; } = MyId.From(Guid.Empty);

        public override string ToString()
            => $"ID1:'{Id1}' - ID2:'{Id2}'";
    }
}

[ValueObject<Guid>]
public readonly partial struct MyId;
