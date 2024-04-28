namespace ConsumerTests;

using AliasedInt = ValueObjectAttribute<int>;

// We can derive from the ValueObjectAttribute to create our own attributes, but seeing as
// the config is compile-time, we're limited to what we can override.
// At some point in the future, we may implement a way to specify defaults for each type
// as suggested in this issue: https://github.com/SteveDunn/Vogen/issues/331
// Update - April '24: allowing derived attributes limits Vogen's ability to use more performant APIs, in particular
// `ForAttributeWithMetadataName`. This is because we have to search for types with an attribute with `ValueObjectAttribute` 
// or *any attribute derived from it*.
// Starting with C# 12, the better alternative is to use type aliases, e.g. `using AliasedInt = ValueObjectAttribute<int>;` 

#pragma warning disable VOG026
public class IntVoAttribute : ValueObjectAttribute<int>
#pragma warning restore VOG026
{
}

#pragma warning disable VOG026
public class StringVoAttribute : ValueObjectAttribute<string>
#pragma warning restore VOG026
{
}

[IntVo]
public partial struct MyIntVo
{
}

[StringVo]
public partial struct MyStringVo
{
    private static Validation Validate(string input) => 
        input == "a" ? Validation.Ok : Validation.Invalid("input must be 'a'");
}

[AliasedInt]
public partial struct AliasedVo
{
}

public class DerivedAttributeTests
{
    [Fact]
    public void Can_use_derived_int_attribute()
    {
        MyIntVo.From(1).Should().Be(MyIntVo.From(1));
        MyStringVo.From("a").Should().Be(MyStringVo.From("a"));
    }
}

public class TypeAliasTests
{
    [Fact]
    public void Can_use_derived_int_attribute()
    {
        AliasedVo.From(1).Should().Be(AliasedVo.From(1));
    }
}
