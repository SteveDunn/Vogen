using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace ConsumerTests.BugFixTests.BugFix639;

[ValueObject<string>(fromPrimitiveCasting: CastOperator.Implicit)]
public partial class C_With
{
    private static string NormalizeInput(string input) => input.ToUpper();
}

[ValueObject<string>(fromPrimitiveCasting: CastOperator.Implicit)]
public partial class C_Without;

/// <summary>
/// Fixes bug https://github.com/SteveDunn/Vogen/issues/639 where any
/// `NormalizeInput` method was not called when implicitly converting a primitive to a value object
/// </summary>
public class Tests
{
    [Fact]
    public void Should_call_if_present()
    {
        C_With vo = "abc";
        vo.Value.Should().Be("ABC");
    }

    [Fact]
    public void Should_not_call_if_not_present()
    {
        C_Without vo = "abc";
        vo.Value.Should().Be("abc");
    }
}
