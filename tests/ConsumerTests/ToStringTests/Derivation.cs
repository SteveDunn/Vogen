using FluentAssertions;
using Vogen;
using Xunit;

namespace ConsumerTests.ToStringTests;

public class Derivation
{
    [Fact]
    public void ToString_uses_users_method() => 
        Vo.From(18).ToString().Should().Be("ToString_Vo!");

    [Fact]
    public void ToString_uses_derived_method() => 
        Vod1.From(18).ToString().Should().Be("derived1!");

    [Fact]
    public void ToString_uses_least_derived_method() => 
        Vod2.From(18).ToString().Should().Be("derived2!");

    [Fact]
    public void ToString_on_a_record_uses_least_derived_method() => 
        Vor1.From(18).ToString().Should().Be("Vor1!");
}

public class D1
{
    public override string ToString() => "derived1!";
}

public class D2 : D1
{
    public new virtual string ToString() => "derived2!";
}

public class D3 : D2
{
}

[ValueObject]
public partial class Vo
{
    public override string ToString() => "ToString_Vo!";
}

[ValueObject]
public partial class Vod1 : D1
{
}

[ValueObject]
public partial class Vod2 : D3
{
}

public record R1
{
    public override string ToString() => "R1!";
}

[ValueObject]
public partial record Vor1 : R1
{
    public override sealed string ToString() => "Vor1!";
}
