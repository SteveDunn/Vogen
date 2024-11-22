#if NULLABLE_DISABLED_BUILD
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#endif

using System;
using System.Threading.Tasks;
using Bogus;
using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Vogen.Examples.SerializationAndConversion.Mongo;


[ValueObject]
public readonly partial struct Age;

[ValueObject<string>]
public readonly partial struct Name;

[UsedImplicitly]
public class Person
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Name Name { get; set; }
    public Age Age { get; set; }
}

[UsedImplicitly]
public class MongoScenario : IScenario
{
    public async Task Run()
    {

        try
        {
            string? runnerOs = Environment.GetEnvironmentVariable("RUNNER_OS");
        
            bool isLocalOrLinuxOnGitHub = string.IsNullOrEmpty(runnerOs) || runnerOs == "Linux";
        
            if (!isLocalOrLinuxOnGitHub)
            {
                Console.WriteLine("Skipping because not running locally or on Linux on a GitHub action.");
                return;
            }
        
            MongoDbContainer container = new MongoDbBuilder().WithImage("mongo:latest").Build();
        
            await container.StartAsync();
        
            var client = new MongoClient(container.GetConnectionString());

            var database = client.GetDatabase("testDatabase");
            var collection = database.GetCollection<Person>("peopleCollection");

            BsonSerializer.RegisterSerializer(new NameBsonSerializer());
            BsonSerializer.RegisterSerializer(new AgeBsonSerializer());
        
            // or, use the generated one for all value objects...
            // BsonSerializationRegisterForVogen_Examples.TryRegister();

            var personFaker = new Faker<Person>()
                .RuleFor(p => p.Name, f => Name.From(f.Name.FirstName()))
                .RuleFor(p => p.Age, f => Age.From(DateTime.Now.Year - f.Person.DateOfBirth.Year));

            foreach (Person eachPerson in personFaker.Generate(10))
            {
                await collection.InsertOneAsync(eachPerson);
            }
        
            Console.WriteLine("Inserted people... Now finding them...");

            IAsyncCursor<Person> people = await collection.FindAsync("{}");
            await people.ForEachAsync((person) => Console.WriteLine($"{person.Name} is {person.Age}"));

            await container.DisposeAsync();
        }
        catch (Exception e) when (e.Message.StartsWith("Docker is either not running"))
        {
            Console.WriteLine("Docker is not running - skipping this scenario");
        }
    }
}



// Note, if you don't want any generated BSON serializers, you can specify your own generic one like the one below.
// Be aware that you'll need specify static abstracts generation in global config for this to work:
// [assembly: VogenDefaults(
//      staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon | StaticAbstractsGeneration.InstanceMethodsAndProperties, 
//      conversions: Conversions.Default | Conversions.Bson)]

 // ReSharper disable once UnusedType.Global
 public class BsonVogenSerializer<TValue, TUnderlyingTypeValue>  
    : SerializerBase<TValue> where TValue : IVogen<TValue, TUnderlyingTypeValue>
{
    private readonly IBsonSerializer<TUnderlyingTypeValue> _serializer = BsonSerializer.LookupSerializer<TUnderlyingTypeValue>();

    public override TValue Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => 
        TValue.From(_serializer.Deserialize(context, args));

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValue value) => 
        _serializer.Serialize(context, args, value.Value);
}
