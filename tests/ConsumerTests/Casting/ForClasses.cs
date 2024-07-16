using FluentAssertions.Execution;

namespace ConsumerTests.CastOperators;

public class ForClasses
{
    [Fact]
    public void Defaulting()
    {
        using var _ = new AssertionScope();
        
        var vo = Class_default.From("abc");

        string prim = (string) vo;

        prim.Should().Be(vo.Value);

        var vo2 = (Class_default) prim;

        vo2.Value.Should().Be(prim);
    }

    [Fact]
    public void Just_implicit_to_primitive()
    {
        using var _ = new AssertionScope();
        
        var vo = Class_implicit_to_primitive_nothing_from_primitive.From("abc");

        string prim = vo;

        prim.Should().Be(vo.Value);
    }

    [Fact]
    public void Implicit_both_ways()
    {
        using var _ = new AssertionScope();
        
        var vo = Class_implicit_both_ways.From("abc");

        string prim = vo;

        prim.Should().Be(vo.Value);
        
        Class_implicit_both_ways vo2 = prim;
        vo2.Should().Be(vo);
    }

    [Fact]
    public void Implicit_both_ways_with_normalization()
    {
        using var _ = new AssertionScope();
        
        var vo = Class_implicit_both_ways_with_normalization.From("abc");

        string prim = vo;

        prim.Should().Be(vo.Value);
        
        Class_implicit_both_ways_with_normalization vo2 = prim;
        vo2.Should().Be(vo);
        vo2.Value.Should().Be("ABC");
    }
}
