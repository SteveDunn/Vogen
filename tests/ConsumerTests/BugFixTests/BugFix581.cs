using System.Text.Json;

namespace ConsumerTests.BugFixTests.BugFix581;

public class ObjectContainerForVo
{
    public required ObjectType Ot { get; set; }
}

[ValueObject<Guid>(Conversions.Default)]
public readonly partial struct ObjectType
{
}

/// <summary>
/// Fixes bug https://github.com/SteveDunn/Vogen/issues/581 where GUIDs that are deserialized
/// with null, throw a NullReferenceException instead of System.Text.JsonException.
/// </summary>
public class Tests
{
    [Fact]
    public void Should_throw_Stj_exception_with_null_passed()
    {
        Action a = () => JsonSerializer.Deserialize<ObjectContainerForVo>("""{"Ot":null}""");
        a.Should().ThrowExactly<JsonException>().WithMessage("The JSON value could not be converted to ConsumerTests.BugFixTests.BugFix581.ObjectType. Path: $.Ot | LineNumber: 0 | BytePositionInLine: 10.");
    }

    [Fact]
    public void Should_throw_Stj_exception_with_invalid_guid_passed()
    {
        Action a = () => JsonSerializer.Deserialize<ObjectContainerForVo>("""{"Ot":"Trevor woz ere"}""");
        a.Should().ThrowExactly<JsonException>().WithMessage("The JSON value could not be converted to ConsumerTests.BugFixTests.BugFix581.ObjectType. Path: $.Ot | LineNumber: 0 | BytePositionInLine: 22.");
    }
}
