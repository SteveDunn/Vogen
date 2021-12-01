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

public class StructTests
{
    [Fact]
    public void Throws_when_accessing_Value_after_creating_with_default()
    {
        var sut = default(CustomerId);
        Func<int> action = () => sut.Value;
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("Validation skipped by default initialisation. Please use 'From(int)' for construction.");
    }

    [Fact]
    public void Throws_when_accessing_Value_after_creating_with_default_2()
    {
        var sut = default(VendorName);
        Func<string> action = () => sut.Value;
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("Validation skipped by default initialisation. Please use 'From(int)' for construction.");
    }
}