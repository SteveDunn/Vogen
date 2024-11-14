using MessagePack;
using MessagePack.Formatters;
using Vogen.IntegrationTests.TestTypes;

namespace ConsumerTests.SerializationAndConversionTests;

public class FooFormatter : IMessagePackFormatter<Bar>
{
    public Bar Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var age = reader.ReadInt32();
        var name = reader.ReadString();

        return new Bar
        {
            Age = age,
            Name = name!
        };
    }

    public void Serialize(ref MessagePackWriter writer, Bar value, MessagePackSerializerOptions options)
    {
        writer.Write(value.Age);
        writer.Write(value.Name);
    }
}