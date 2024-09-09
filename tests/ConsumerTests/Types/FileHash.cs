using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vogen.Tests.Types;

/// <summary>
/// Vogen uses equality from the underlying type, so collections aren't, by default,
/// equatable. They use reference equality, and even though they implement IEquatable,
/// that is no contract to say that it's 'deep' equatable.
/// This example uses an underlying type of `Hash`, which is an array and has its own
/// `IEquatable` which uses `MutableEquatableArray` from the TypeShape library. 
/// </summary>
[ValueObject<Hash<byte>>(Conversions.None)]
[JsonConverter(typeof(JsonConverter))]
public readonly partial struct FileHash
{
    public class JsonConverter : JsonConverter<FileHash>
    {
        public override FileHash Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => 
            From(JsonSerializer.Deserialize<byte[]>(ref reader, options)!);
    
        public override void Write(Utf8JsonWriter writer, FileHash value, JsonSerializerOptions options) => 
            JsonSerializer.Serialize(writer, (byte[])value._value!, options);
    }    
}
