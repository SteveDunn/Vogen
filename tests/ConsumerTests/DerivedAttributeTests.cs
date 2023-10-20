namespace ConsumerTests;

// We can derive from the ValueObjectAttribute to create our own attributes, but seeing as
// the config is compile time, we're limited to what we can override.
// At some point in the future, we may implement a way to specify defaults for each type
// as suggested in this issue: https://github.com/SteveDunn/Vogen/issues/331

#if NET7_0_OR_GREATER
public class IntVoAttribute : ValueObjectAttribute<int>
{
}

public class StringVoAttribute : ValueObjectAttribute<string>
{
}

public class MyCustomException : Exception
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

public class DerivedAttributeTests
{
    [Fact]
    public void Can_use_derived_int_attribute()
    {
        MyIntVo.From(1).Should().Be(MyIntVo.From(1));
        MyStringVo.From("a").Should().Be(MyStringVo.From("a"));
    }
}
#endif
