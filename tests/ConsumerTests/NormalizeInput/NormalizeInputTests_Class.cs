using System;
using FluentAssertions;
using Vogen;
using Xunit;

namespace ConsumerTests.NormalizeInput;

public class NormalizeInputTests_Class
{
    
    [Fact]
    public void WhenInputIsNormalized_ReturnsTheNormalizedValue() => Struct_NormalizedToMax128.From(129).Value.Should().Be(128);

    [Fact]
    public void WhenInputIsNormalized_DoesNotThrowIfNormalizedToValidValue()
    {
        Action a = () => Struct_NormalizedToMax128WithValidation.From(129);
        a.Should().NotThrow();
    }

    [Fact]
    public void Class_WhenInputIsNormalized_ThrowsIfNormalizedToInvalidValue()
    {
        Action a = () => Class_NormalizedToMax128WithValidation.From(-1);
        a.Should().ThrowExactly<ValueObjectValidationException>();
    }

    [Fact]
    public void Class_WhenInputIsNormalizedWithASynonymousInputParameter_ReturnsTheNormalizedValue() =>
        Class_Normalized_WithSynonymousInputParam.From(129).Value.Should().Be(128);

    [Fact]
    public void Class_WhenInputIsNormalizedWithASynonymousReturnType_ReturnsTheNormalizedValue() =>
        Class_Normalized_WithSynonymousReturnType.From(129).Value.Should().Be(128);

    [Fact]
    public void Class_WhenInputIsNormalizedWithCamelCasedMethodName_ReturnsTheNormalizedValue() =>
        Class_Normalized_WithCamelCasedMethodName.From(129).Value.Should().Be(128);
}

[ValueObject(underlyingType:typeof(int))]
public partial class Class_Normalized_WithSynonymousInputParam
{
    private static int NormalizeInput(Int32 input) => Math.Min(128, input);
}

[ValueObject(underlyingType:typeof(int))]
public partial class Class_Normalized_WithSynonymousReturnType
{
    private static Int32 NormalizeInput(int input) => Math.Min(128, input);
}

[ValueObject(underlyingType:typeof(int))]
public partial class Class_Normalized_WithCamelCasedMethodName
{
    private static int normalizeInput(int input) => Math.Min(128, input);
}

[ValueObject(underlyingType:typeof(int))]
public partial class Class_NormalizedToMax128WithValidation
{
    private static Validation Validate(int value) => value <= 128 && value >= 0 ? Validation.Ok : Validation.Invalid("xxxx");

    private static Int32 NormalizeInput(int input) => Math.Min(128, input);
}
