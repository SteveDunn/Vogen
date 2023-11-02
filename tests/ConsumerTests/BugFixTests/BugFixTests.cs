using Newtonsoft.Json;

namespace ConsumerTests.BugFixTests;

[ValueObject(typeof(int), toPrimitiveCasting: CastOperator.None, fromPrimitiveCasting: CastOperator.None)]
public partial struct Bug502Vo
{
    public static implicit operator int(Bug502Vo vo) => vo._value;
}

public class BugFixTests
{
    /// <summary>
    /// Fixes bug https://github.com/SteveDunn/Vogen/issues/344 where a field that is a ValueObject and is null when
    /// deserialized by Newtonsoft.Json, throws an exception instead of returning null.
    /// </summary>
    [Fact]
    public void Bug344_Can_deserialze_a_null_field()
    {
        var p = new Person
        {
            Age = Age.From(42)
        };

        var serialized = JsonConvert.SerializeObject(p);
        var deserialized = JsonConvert.DeserializeObject<Person>(serialized)!;

        deserialized.Age.Should().Be(Age.From(42));
        deserialized.Name.Should().BeNull();
        deserialized.Address.Should().BeNull();
    }

    /// <summary>
    /// Fixes bug https://github.com/SteveDunn/Vogen/issues/502 where a VO cannot have a user supplied implicit cast.
    /// </summary>
    [Fact]
    public void Bug502_cannot_have_implicit_cast_operator()
    {
        var b = Bug502Vo.From(42);

        int n = b;
        n.Should().Be(42);
    }
}

public class Person
{
    public Age Age { get; init; }
        
    public Name? Name { get; set; }
        
    public Address? Address { get; set; }
}
    
[ValueObject(conversions: Conversions.NewtonsoftJson)] public partial struct Age { }
    
[ValueObject(typeof(string), conversions: Conversions.NewtonsoftJson)] public partial class Name { }
    
[ValueObject(typeof(string), conversions: Conversions.NewtonsoftJson)] public partial struct Address { }