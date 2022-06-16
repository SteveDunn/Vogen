#nullable disable
using System;
using System.Text.Json;
using FluentAssertions;
using Vogen;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MediumTests.SerializationAndConversionTests;

[ValueObject(underlyingType: typeof(double), conversions: Conversions.Default, customizations: Customizations.TreatDoublesAsStringsInSystemTextJson)]
public partial class HolderId
{
}

public class MyThing
{
    public HolderId HolderId { get; set; }
}

public class CustomizationTests
{
    [Fact]
    public void CanSerializeAndDeserializeAsString()
    {
        var holderId = HolderId.From(720742592373919744);

        string serialized = JsonSerializer.Serialize(holderId);
        var deserialized = JsonSerializer.Deserialize<HolderId>(serialized);

        deserialized.Value.Should().Be(720742592373919744);
    }

    [Fact]
    public void CanSerializeAndDeserializeWhenVoIsInAComplexObject()
    {
        var holderId = HolderId.From(720742592373919744);

        var t = new MyThing
        {
            HolderId = holderId
        };

        string serialized = JsonSerializer.Serialize(t);
        var deserialized = JsonSerializer.Deserialize<MyThing>(serialized);

        deserialized.HolderId.Value.Should().Be(720742592373919744);
    }
}

// class MyJsonConverter : global::System.Text.Json.Serialization.JsonConverter<HolderId>
// {
//     public override HolderId Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
//     {
//         return HolderId.From(double.Parse(reader.GetString()));
//     }
//
//     public override void Write(System.Text.Json.Utf8JsonWriter writer, HolderId value, global::System.Text.Json.JsonSerializerOptions options)
//     {
//         writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
//     }
// }

// class HolderIdSystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<HolderId>
// {
//     public override HolderId Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
//     {
//         return HolderId.From(reader.GetDouble());
//     }
//
//     public override void Write(System.Text.Json.Utf8JsonWriter writer, HolderId value, global::System.Text.Json.JsonSerializerOptions options)
//     {
//         writer.WriteNumberValue(value.Value);
//     }
// }

public class StringConverter : System.Text.Json.Serialization.JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {

        if (reader.TokenType == JsonTokenType.Number)
        {
            var stringValue = reader.GetInt64();
            return stringValue.ToString();
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }

        throw new System.Text.Json.JsonException();
    }

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Int64);

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}

