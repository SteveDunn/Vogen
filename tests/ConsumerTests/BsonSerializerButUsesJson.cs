using System.IO;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace ConsumerTests;

internal static class BsonSerializerButUsesJson
{
    public static string Serialize<T>(T input)
    {
        TextWriter sw = new StringWriter();

        IBsonWriter writer = new JsonWriter(sw); 
        BsonSerializationContext context = BsonSerializationContext.CreateRoot(writer);

        BsonSerializer.LookupSerializer<T>().Serialize(context, input);
        
        sw.Flush();

        return sw.ToString()!;
    }

    public static T Deserialize<T>(string input)
    {
        IBsonReader reader = new JsonReader(input);
        var context = BsonDeserializationContext.CreateRoot(reader);
        return BsonSerializer.LookupSerializer<T>().Deserialize(context);
    }
}