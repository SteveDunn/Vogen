using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsumerTests.BugFixTests.BugFix578;

public class Bug578
{
    /// <summary>
    /// Fixes bug https://github.com/SteveDunn/Vogen/issues/344 where a field that is a ValueObject and is null when
    /// deserialized by Newtonsoft.Json, throws an exception instead of returning null.
    /// </summary>
    [Fact]
    public void Bug758_Deserializing_skipped_check_again_known_instances()
    {
        var directNonEmpty = TestId.From("foobar");
        var directEmpty = TestId.Empty;
        var nonEmptyFromJson = JsonSerializer.Deserialize<TestContainer>("{ \"testId\": \"barfoo\" }");
        var emptyFromJson = JsonSerializer.Deserialize<TestContainer>("{ \"testId\": \"\" }");

        directNonEmpty.Value.Should().Be("foobar");
        nonEmptyFromJson!.TestId.Value.Should().Be("barfoo");
        directEmpty.Value.Should().Be("");
        emptyFromJson!.TestId.Value.Should().Be("");
        
    }
}

[ValueObject<string>]
[Instance("Empty", "")]
public partial record TestId
{
    private static Validation Validate(string value) => !string.IsNullOrWhiteSpace(value)
        ? Validation.Ok
        : Validation.Invalid("Test ID must be a non-blank, non-empty string.");

    private static string NormalizeInput(string value) => value.Trim();
}

public record TestContainer(
    [property: JsonPropertyName("testId")]
    TestId TestId);