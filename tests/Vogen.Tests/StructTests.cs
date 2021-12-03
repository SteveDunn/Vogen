using System;
using FluentAssertions;
using Vogen.Tests.Types;
using Xunit;

namespace Vogen.Tests.StructTests;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static Validation Validate(int value) =>
        value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

[ValueObject(typeof(string))]
public partial struct VendorName
{
    private static Validation Validate(string value) =>
        string.IsNullOrEmpty(value) ? Validation.Invalid("cannot be null or empty") : Validation.Ok;
}

public class StructTests_creating_with_parameterless_constructor
{
    [Fact]
    public void Throws_when_creating_with_constructor()
    {
        Action action = () => new CustomerId();
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("Validation skipped by attempting to use the default constructor. Please use the 'From' method for construction.");
    }
}