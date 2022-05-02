using System;
using FluentAssertions;
using Xunit;

namespace Vogen.Tests.NormalizeInput;

public class NormalizeInputTests
{
    
    [Fact]
    public void WhenInputIsNormalized_ReturnsTheNormalizedValue() => NormalizedToMax128.From(129).Value.Should().Be(128);

    [Fact]
    public void WhenInputIsNormalized_DoesNotThrowIfNormalizedToValidValue()
    {
        Action a = () => NormalizedToMax128WithValidation.From(129);
        a.Should().NotThrow();
    }

    [Fact]
    public void WhenInputIsNormalized_ThrowsIfNormalizedToValidValue()
    {
        Action a = () => NormalizedToMax128WithValidation.From(-1);
        a.Should().ThrowExactly<ValueObjectValidationException>();
    }
    
    [Fact]
    public void WhenInputIsNormalizedWithASynonymousInputParameter_ReturnsTheNormalizedValue() => Normalized_WithSynonymousInputParam.From(129).Value.Should().Be(128);
    
    [Fact]
    public void WhenInputIsNormalizedWithASynonymousReturnType_ReturnsTheNormalizedValue() => Normalized_WithSynonymousReturnType.From(129).Value.Should().Be(128);

    [Fact]
    public void WhenInputIsNormalizedWithCamelCasedMethodName_ReturnsTheNormalizedValue() => Normalized_WithCamelCasedMethodName.From(129).Value.Should().Be(128);
    
}



[ValueObject(underlyingType:typeof(int))]
public partial class NormalizedToMax128
{
    private static Int32 NormalizeInput(int input) => Math.Min(128, input);
}

[ValueObject(underlyingType:typeof(int))]
public partial class Normalized_WithSynonymousInputParam
{
    private static int NormalizeInput(Int32 input) => Math.Min(128, input);
}

[ValueObject(underlyingType:typeof(int))]
public partial class Normalized_WithSynonymousReturnType
{
    private static Int32 NormalizeInput(int input) => Math.Min(128, input);
}

[ValueObject(underlyingType:typeof(int))]
public partial class Normalized_WithCamelCasedMethodName
{
    private static int NormalizeInput(int input) => Math.Min(128, input);
}

[ValueObject(underlyingType:typeof(int))]
public partial class NormalizedToMax128WithValidation
{
    private static Validation Validate(int value) => value <= 128 && value >= 0 ? Validation.Ok : Validation.Invalid("xxxx");

    private static Int32 NormalizeInput(int input) => Math.Min(128, input);
}
