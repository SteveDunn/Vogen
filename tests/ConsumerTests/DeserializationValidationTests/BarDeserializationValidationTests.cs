using System.Text.Json;
using Vogen.IntegrationTests.TestTypes;

namespace ConsumerTests.DeserializationValidationTests;

public class BarDeserializationValidationTests
{
    [Fact]
    public void Deserialization_systemtextjson_should_not_bypass_validation_pass()
    {
        var validValue = JsonSerializer.Serialize(MyVoBar_should_not_bypass_validation.From(new Bar(20, "John")));

        var actual = JsonSerializer.Deserialize<MyVoBar_should_not_bypass_validation>(validValue)!.Value;

        actual.Should().Be(new Bar(20, "John"));
    }

    [Fact]
    public void Deserialization_systemtextjson_should_not_bypass_validation_fail()
    {
        var invalidValue = JsonSerializer.Serialize(MyVoBar_should_not_bypass_validation.From(new Bar(20, "John"))).Replace("20", "5");

        Action vo = () => JsonSerializer.Deserialize<MyVoBar_should_not_bypass_validation>(invalidValue);

        vo.Should().ThrowExactly<JsonException>()
            .WithMessage("The JSON value could not be converted to ConsumerTests.DeserializationValidationTests.*")
            .WithMessage("*Path: $ |*")
            .WithInnerException<ValueObjectValidationException>()
            .WithMessage("Bar must be 18");
    }
}