using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Vogen.Examples.SerializationAndConversion.Mongo;


[ValueObject]
public readonly partial struct Age;

[ValueObject<string>]
public readonly partial struct Name;

public class Person
{
    public Name Name { get; set; }
    public Age Age { get; set; }
}

public class MongoScenario : IScenario
{
    public async Task Run()
    {
        RunIt();
        await Task.CompletedTask;
    }

    public static void RunIt()
    {
        string connectionString = "mongodb://root:secret@localhost:27017";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("testDatabase");
        var collection = database.GetCollection<Person>("peopleCollection");

        // BsonSerializer.RegisterSerializer(new BsonSerializerAdapter<Age, int>(BsonSerializer.LookupSerializer<int>(), age => age.Value, Age.From));
        // BsonSerializer.RegisterSerializer(new BsonSerializerAdapter<Name, string>(BsonSerializer.LookupSerializer<string>(), age => age.Value, Name.From));
        BsonSerializer.RegisterSerializer(new NameBsonSerializer());
        BsonSerializer.RegisterSerializer(new AgeBsonSerializer());
        // BsonSerializer.RegisterSerializer(new BsonVogenSerializer<Name, string>());
        // BsonSerializer.RegisterSerializer(new BsonVogenSerializer<Age, int>());
        Person p = new Person
        {
            Age = Age.From(44),
            Name = Name.From("Barney Rubble")
        };

        collection.InsertOneAsync(p).GetAwaiter().GetResult();
        
        // var serializer = MongoDB.Bson.Serialization.BsonSerializer.LookupSerializer<Person>();
        //
        // var bsonDocument = new BsonDocument();
        // using (var writer = new BsonDocumentWriter(bsonDocument))
        // {
        //     serializer.Serialize(writer, typeof(Person), p, null);
        // }

        
    }
}


public class BsonSerializerAdapter_old<TValue, TUnderlyingTypeValue>(
    IBsonSerializer<TUnderlyingTypeValue> serializer,
    Func<TValue, TUnderlyingTypeValue> to,
    Func<TUnderlyingTypeValue, TValue> from)
    : SerializerBase<TValue>
{
    public override TValue Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => from.Invoke(serializer.Deserialize(context, args));

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValue value)
        => serializer.Serialize(context, args, to.Invoke(value));
}

// public class BsonVogenSerializer<TValue, TUnderlyingTypeValue>  
//     : SerializerBase<TValue> where TValue : IVogen<TValue, TUnderlyingTypeValue>
// {
//     private readonly IBsonSerializer<TUnderlyingTypeValue> _serializer = BsonSerializer.LookupSerializer<TUnderlyingTypeValue>();
//
//     public override TValue Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => 
//         TValue.From(_serializer.Deserialize(context, args));
//
//     public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValue value) => 
//         _serializer.Serialize(context, args, value.Value);
// }

public class BsonNameSerializer : SerializerBase<Name>
{
    private readonly IBsonSerializer<string> _serializer = BsonSerializer.LookupSerializer<string>();

    public override Name Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => 
        Name.From(_serializer.Deserialize(context, args));

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Name value) => 
        _serializer.Serialize(context, args, value.Value);
}