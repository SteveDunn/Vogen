using FluentAssertions.Execution;

namespace ConsumerTests.CastOperators.Structs;

public class ForStructs
{
    [Fact]
    public void Defaulting()
    {
        using var _ = new AssertionScope();
        
        Vo originalVo = Vo.From("abc");

        Vo voCastFromString = (Vo) "abc";
        string stringCastFromVo = (string)originalVo;

        voCastFromString.Should().Be(originalVo);
        voCastFromString.Value.Should().Be(stringCastFromVo);

        stringCastFromVo.Should().Be(originalVo.Value);

        var voRecastFromCastedString = (Vo) stringCastFromVo;
        voRecastFromCastedString.Value.Should().Be(stringCastFromVo);
    }

    [Fact]
    public void Just_implicit_to_primitive()
    {
        using var _ = new AssertionScope();
        
        var vo = Implicit_to_primitive_nothing_from_primitive.From("abc");

        string prim = vo;

        prim.Should().Be(vo.Value);
    }

    [Fact]
    public void Implicit_casting_both_ways()
    {
        using var _ = new AssertionScope();
        
        var vo = Implicit_both_ways.From("abc");

        string prim = vo;

        prim.Should().Be(vo.Value);
        
        Implicit_both_ways vo2 = prim;
        vo2.Should().Be(vo);
    }
    
    [Fact]
    public void Implicit_both_ways_with_normalization()
    {
        using var _ = new AssertionScope();
        
        var vo = Structs.Implicit_both_ways_with_normalization.From("abc");

        string prim = vo;

        prim.Should().Be(vo.Value);
        
        Implicit_both_ways_with_normalization vo2 = prim;
        vo2.Should().Be(vo);
        vo2.Value.Should().Be("ABC");
    }
    
}


[ValueObject<string>]
public partial class Vo;

[ValueObject<string>(toPrimitiveCasting: CastOperator.Implicit, fromPrimitiveCasting: CastOperator.None)]
public partial class Implicit_to_primitive_nothing_from_primitive;

[ValueObject<string>(toPrimitiveCasting: CastOperator.Implicit, fromPrimitiveCasting: CastOperator.Implicit)]
public partial class Implicit_both_ways;

[ValueObject<string>(toPrimitiveCasting: CastOperator.Implicit, fromPrimitiveCasting: CastOperator.Implicit)]
public partial class Implicit_both_ways_with_normalization
{
    private static string NormalizeInput(string input) => input.ToUpper();
}
