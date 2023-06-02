using System.Globalization;

namespace ConsumerTests.TryParseTests;

public class Tests
{
    [Fact]
    public void Integers()
    {
        {
            IntVoNoValidation.TryParse("1024", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1024);
        }

        {
            IntVoNoValidation.TryParse("1,000", NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000);
        }
    }

    [Fact]
    public void Decimals()
    {
        {
            DecimalVo.TryParse("1024.56", NumberStyles.Any, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1024.56m);
        }

        {
            DecimalVo.TryParse("1,000.25", NumberStyles.Number, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000.25m);
        }

        {
            DecimalVo.TryParse("1.000,25", NumberStyles.Number, new CultureInfo("de"), out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000.25m);
        }
    }

    [Fact]
    public void Bytes()
    {
        ByteVo.TryParse("12", out var ivo).Should().BeTrue();
        ivo.Value.Should().Be(12);
    }

    [Fact]
    public void Chars()
    {
        CharVo.TryParse("a", out var ivo).Should().BeTrue();
        ivo.Value.Should().Be('a');
    }

    [Fact]
    public void Double()
    {
        DoubleVo.TryParse("123.45", NumberStyles.Any, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
        ivo.Value.Should().Be(123.45);
    }

    [Fact]
    public void When_parsing_fails()
    {
        IntVo.TryParse("fifty", out var ivo).Should().BeFalse();

        Action a = () => _ = ivo.Value;

        a.Should().ThrowExactly<ValueObjectValidationException>();
    }

    [Fact]
    public void When_parsing_succeeds_but_fails_validation()
    {
        Action a = () => IntVo.TryParse("100", out var _);

        a.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be less than 100");
    }
}