using System.IO;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace ConsumerTests.BsonTests;

[ValueObject]
public readonly partial struct Age;

[ValueObject<string>]
public readonly partial struct Name;

public class Person
{
    public Name Name { get; init; }
    public Age Age { get; init; }
}

public class BsonTests
{
    private readonly IBsonSerializer<Person> _lookupSerializer;

    public BsonTests() => _lookupSerializer = BsonSerializer.LookupSerializer<Person>();

    // The register for all serializers is generated and is called in ModuleInitialization.cs (BsonSerializationRegisterForConsumerTests.TryRegister())
    [Fact]
    public void Value_objects_are_written_as_primitives()
    {
        Person person = new()
        {
            Age = Age.From(42),
            Name = Name.From("Fred Flintstone")
        };

        TextWriter sw = new StringWriter();

        IBsonWriter writer = new JsonWriter(sw); 
        BsonSerializationContext context = BsonSerializationContext.CreateRoot(writer);

        _lookupSerializer.Serialize(context, person);
        
        sw.Flush();
        sw.ToString().Should().Be($$"""{ "Name" : "Fred Flintstone", "Age" : 42 }""");
    }

    [Fact]
    public void Value_objects_are_read_as_primitives()
    {
        using IBsonReader reader = new JsonReader("""{ "Name" : "Fred Flintstone", "Age" : 42 }""");
        var context = BsonDeserializationContext.CreateRoot(reader);
        
        Person person = _lookupSerializer.Deserialize(context);
        person.Age.Value.Should().Be(42);
        person.Name.Value.Should().Be("Fred Flintstone");
    }

    [Fact]
    public void Missing_values_are_allowed_when_configured()
    {
        using IBsonReader reader = new JsonReader("""{ "Age" : 42 }""");
        var context = BsonDeserializationContext.CreateRoot(reader);
        
        Person person = _lookupSerializer.Deserialize(context);
        person.Age.Value.Should().Be(42);
        person.Name.IsInitialized().Should().BeFalse();
    }
}