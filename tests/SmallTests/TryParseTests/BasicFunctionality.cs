using System.Globalization;
using FluentAssertions;
using Vogen;
using Xunit;

namespace SmallTests.TryParseTests;

[ValueObject(typeof(int))]
public partial struct IntVo { }

[ValueObject(typeof(byte))]
public partial struct ByteVo { }

[ValueObject(typeof(char))]
public partial struct CharVo { }

[ValueObject(typeof(decimal))]
public partial struct DecimalVo { }

[ValueObject(typeof(double))]
public partial struct DoubleVo { }

public class BasicFunctionality
{
    [Fact]
    public void Integers()
    {
        {
            IntVo.TryParse("1024", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1024);
        }

        {
            IntVo.TryParse("1,000", NumberStyles.AllowThousands, null, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000);
        }
    }

    [Fact]
    public void Decimals()
    {
        {
            DecimalVo.TryParse("1024.56", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1024.56m);
        }

        {
            DecimalVo.TryParse("1,000.24", NumberStyles.AllowThousands, null, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000.25m);
        }
    }
}