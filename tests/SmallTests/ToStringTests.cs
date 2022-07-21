using FluentAssertions;
using Vogen;
using Vogen.Tests.Types;
using Xunit;

namespace SmallTests;

public class ToString_Derived1
{
    public override string ToString() => "derived1!";
}

public class ToString_Derived2 : ToString_Derived1
{
    public new virtual string ToString() => "derived2!";
}

public class ToString_Derived3 : ToString_Derived2
{
}

[ValueObject]
public partial class ToString_Vo
{
    public override string ToString() => "ToString_Vo!";
}

[ValueObject]
public partial class ToString_Vo_Derived1 : ToString_Derived1
{
}

[ValueObject]
public partial class ToString_Vo_Derived2 : ToString_Derived3
{
}

public class ToStringTests
{
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

    [Fact]
    public void ToString_uses_users_method() => 
        ToString_Vo.From(18).ToString().Should().Be("ToString_Vo!");

    [Fact]
    public void ToString_uses_derived_method() => 
        ToString_Vo_Derived1.From(18).ToString().Should().Be("derived1!");

    [Fact]
    public void ToString_uses_least_derived_method() => 
        ToString_Vo_Derived2.From(18).ToString().Should().Be("derived2!");
}