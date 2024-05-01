using Vogen.Tests.Types;

namespace Vogen.Tests.ValidationTests;

public class ValidationTests
{
    [Fact]
    public void Validation_passes()
    {
        Action camelCase = () => MyVo_validate_with_camelCase_method_name.From(-1);
        Action pascalCase = () => MyVo_validate_with_PascalCase_method_name.From(-1);

        camelCase.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be greater than zero");
        pascalCase.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be greater than zero");
    }

    [Fact]
    public void Validation_can_have_different_synonyms_for_input()
    {
        Action vo = () => MyVo_validate_with_synonymous_input_parameter.From(-1);

        vo.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be greater than zero");
    }

    [Fact]
    public void Validation_can_throw_a_different_exception()
    {
        Action vo = () => MyVo_throws_custom_exception.From(-1);

        vo.Should().ThrowExactly<InvalidAmountException>().WithMessage("must be greater than zero");
    }

    [Fact]
    public void Validation()
    {
        Func<Age> act = () => Age.From(12);
        act.Should().ThrowExactly<ValueObjectValidationException>();

        Func<EightiesDate> act2 = () => EightiesDate.From(new DateTime(1990, 1, 1));
        act2.Should().ThrowExactly<ValueObjectValidationException>();

        Func<EightiesDate> act3 = () => EightiesDate.From(new DateTime(1985, 6, 10));
        act3.Should().NotThrow();

        string[] validDaves = new[] { "dave grohl", "david beckham", "david bowie" };
        foreach (var name in validDaves)
        {
            Func<Dave> act4 = () => Dave.From(name);
            act4.Should().NotThrow();
        }

        string[] invalidDaves = new[] { "dafid jones", "fred flintstone", "davidoff cool water" };
        foreach (var name in invalidDaves)
        {
            Func<Dave> act5 = () => Dave.From(name);
            act5.Should().ThrowExactly<ValueObjectValidationException>();
        }

        Func<MinimalValidation> act6 = () => MinimalValidation.From(1);
        act6.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("[none provided]");
    }

    [Fact]
    public void No_validation()
    {
        Func<Anything> act = () => Anything.From(int.MaxValue);
        act.Should().NotThrow();
    }
}

[ValueObject(typeof(int))]
public partial class MyVo_validate_with_camelCase_method_name
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}

[ValueObject(typeof(int))]
public partial class MyVo_validate_with_synonymous_input_parameter
{
    // the input type is synonymous the same as the underlying type
    private static Validation validate(Int32 value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}


[ValueObject(typeof(int))]
public partial class MyVo_validate_with_PascalCase_method_name
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}

[ValueObject(throws: typeof(InvalidAmountException))]
public partial class MyVo_throws_custom_exception
{
    private static Validation Validate(int value) => 
        value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

// Does not compile:
// VOG012 : String must derive from System.Exception
// VOG013 : String must have at least 1 public constructor with 1 parameter of type System.String
[ValueObject(throws: typeof(string))]
public partial class MyVo_throws_non_exception
{
    private static Validation Validate(int value) => 
        value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

public class InvalidAmountException : Exception
{
    public InvalidAmountException()
    {
    }

    public InvalidAmountException(string? message) : base(message)
    {
    }
}
