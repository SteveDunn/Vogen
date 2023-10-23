using FluentAssertions.Execution;

namespace ConsumerTests.CastOperators;

public class ForStructs
{
    [Fact]
    public void Defaulting()
    {
        using var _ = new AssertionScope();
        
        var vo = Struct_default.From("abc");

        string prim = (string) vo;

        prim.Should().Be(vo.Value);

        var vo2 = (Struct_default) prim;

        vo2.Value.Should().Be(prim);
    }

    [Fact]
    public void Just_implicit_to_primitive()
    {
        using var _ = new AssertionScope();
        
        var vo = Struct_implicit_to_primitive_nothing_from_primitive.From("abc");

        string prim = vo;

        prim.Should().Be(vo.Value);
    }

    [Fact]
    public void Implicit_both_ways()
    {
        using var _ = new AssertionScope();
        
        var vo = Struct_implicit_both_ways.From("abc");

        string prim = vo;

        prim.Should().Be(vo.Value);
        
        Struct_implicit_both_ways vo2 = prim;
        vo2.Should().Be(vo);
    }
}
