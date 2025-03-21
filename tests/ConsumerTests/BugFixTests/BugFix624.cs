using System.Text.Json;

namespace ConsumerTests.BugFixTests.BugFix624;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial class NonStrictReferenceVo;

[ValueObject<string>(conversions: Conversions.SystemTextJson, deserializationStrictness: DeserializationStrictness.DisallowNulls)]
public partial class StrictReferenceVo;

[ValueObject(conversions: Conversions.SystemTextJson)]
public partial struct ValueVo;

public class StrictContainer
{
    // ReSharper disable once NullableWarningSuppressionIsUsed
    public StrictReferenceVo Rvo { get; set; } = null!;
}

public class NonStrictContainer
{
    // ReSharper disable once NullableWarningSuppressionIsUsed
    public NonStrictReferenceVo Rvo { get; set; } = null!;
}

/// <summary>
/// Fixes bug https://github.com/SteveDunn/Vogen/issues/624 where STJ
/// deserialization would allow a null value object if it was a reference type
/// </summary>
public class Tests
{
    [Fact]
    public void Should_throw_if_null()
    {
        string json = $$"""{"Rvo":null}""";
        
        Action a = () => JsonSerializer.Deserialize<StrictContainer>(json);
        a.Should().ThrowExactly<JsonException>()
            .WithMessage("The JSON value could not be converted to ConsumerTests.BugFixTests.BugFix624.StrictReferenceVo.*")
            .WithMessage("*Path: $.Rvo |*")
            .WithInnerException<ValueObjectValidationException>()
            .WithMessage("Cannot create a value object with null.");
    }

    [Fact]
    public void Can_override_null_behaviour_to_not_throw()
    {
        string json = $$"""{"Rvo":null}""";
        
        var x = JsonSerializer.Deserialize<NonStrictContainer>(json)!;
        x.Rvo.Should().BeNull();
    }
}
