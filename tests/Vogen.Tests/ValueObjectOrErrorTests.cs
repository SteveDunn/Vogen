using System;
using FluentAssertions;
using Xunit;

namespace Vogen.ValueObjectOrErrorTests;

public class Foo
{
}

public class ValueObjectOrErrorTests
{
    [Fact]
    public void Returns_value_when_success()
    {
        var source = new Foo();
        ValueObjectOrError<Foo> f = new(source);
        f.IsSuccess.Should().BeTrue();
        f.ValueObject.Should().Be(source);
    }

    [Fact]
    public void Returns_error_when_invalid()
    {
        var error = Validation.Invalid("oh no!");
        ValueObjectOrError<Foo> f = new(error);
        f.IsSuccess.Should().BeFalse();
        f.Error.Should().Be(error);
        Action a = () => _ = f.ValueObject;
        a.Should().ThrowExactly<InvalidOperationException>().WithMessage("Cannot access the value object as it is not valid: oh no!");
    }
}