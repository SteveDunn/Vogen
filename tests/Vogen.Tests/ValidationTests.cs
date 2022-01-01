using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Vogen.Tests.Types;
using Xunit; //using tests.Types.AnotherNamespace.AndAnother;

namespace Vogen.Tests;

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
public partial class MyVo_validate_with_PascalCase_method_name
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}

public class Validationtests
{
    [Fact]
    public void Valiation_passes()
    {
        Action camelCase = () => MyVo_validate_with_camelCase_method_name.From(-1);
        Action pascalCase = () => MyVo_validate_with_PascalCase_method_name.From(-1);

        camelCase.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be greater than zero");
        pascalCase.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be greater than zero");
    }
}