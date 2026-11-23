using System.Globalization;

namespace ConsumerTests.NormalizeParsed;

public class NormalizeParsedTests
{
    [Fact]
    public void Parse_PassesTheNormalizedStringToTheUnderlyingParser() =>
        Struct_WithEPrefixedToString.Parse("E12").Value.Should().Be(12);

    [Fact]
    public void TryParse_PassesTheNormalizedStringToTheUnderlyingParser()
    {
        bool success = Struct_WithEPrefixedToString.TryParse("E12", out Struct_WithEPrefixedToString result);

        success.Should().BeTrue();
        result.Value.Should().Be(12);
    }

    [Fact]
    public void TryParseWithFormatProvider_PassesTheNormalizedStringToTheUnderlyingParser()
    {
        bool success = Struct_WithEPrefixedToString.TryParse(
            "E12",
            CultureInfo.InvariantCulture,
            out Struct_WithEPrefixedToString result);

        success.Should().BeTrue();
        result.Value.Should().Be(12);
    }

    [Fact]
    public void Parse_StillRejectsUnparseableInput() =>
        FluentActions.Invoking(() => Struct_WithEPrefixedToString.Parse("not a number"))
            .Should().ThrowExactly<FormatException>();

    [Fact]
    public void TryParse_ReturnsFalseForUnparseableInput()
    {
        bool success = Struct_WithEPrefixedToString.TryParse("E", out _);

        success.Should().BeFalse();
    }

    [Fact]
    public void RoundTripsThroughToString() =>
        Struct_WithEPrefixedToString.Parse(Struct_WithEPrefixedToString.From(12).ToString()).Value.Should().Be(12);

    [Fact]
    public void WithoutNormalizeParsed_TheRawInputIsPassedThrough() =>
        Struct_WithoutNormalizeParsed.TryParse("E12", out _).Should().BeFalse();
}

[ValueObject<int>]
public partial struct Struct_WithEPrefixedToString
{
    private static string NormalizeParsed(string inputToParse) => inputToParse.TrimStart('E');

    public override string ToString() => $"E{Value}";
}

[ValueObject<int>]
public partial struct Struct_WithoutNormalizeParsed;
