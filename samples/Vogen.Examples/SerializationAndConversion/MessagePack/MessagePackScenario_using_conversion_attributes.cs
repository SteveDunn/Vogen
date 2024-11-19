#if NULLABLE_DISABLED_BUILD
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#endif

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;

namespace Vogen.Examples.SerializationAndConversion.MessagePackScenario.UsingConversionAttributes;

[ValueObject<int>(conversions: Conversions.MessagePack)]
public readonly partial struct Age;

[ValueObject<int>(conversions: Conversions.MessagePack)]
public readonly partial struct PersonId;

[ValueObject<string>(conversions: Conversions.MessagePack)]
public readonly partial struct Name;

[UsedImplicitly]
[MessagePackObject]
public class Person
{
    [Key(0)]
    public PersonId Id { get; set; }
    
    [Key(1)]
    public Name Name { get; set; }
    
    [Key(2)]
    public Age Age { get; set; }
}

[UsedImplicitly]
public class MessagePackScenario_using_conversion_attributes : IScenario
{
    public Task Run()
    {
        var options = MessagePackSerializerOptions.Standard;

        var originalObject = new Person
        {
            Id = PersonId.From(123),
            Name = Name.From("Test"),
            Age = Age.From(42)
        };

        byte[] serializedObject = MessagePackSerializer.Serialize(originalObject, options);

        var deserializedObject = MessagePackSerializer.Deserialize<Person>(serializedObject, options);

        Console.WriteLine($"Id: {deserializedObject.Id}, Name: {deserializedObject.Name}, Active: {deserializedObject.Age}");

        return Task.CompletedTask;
    }
}
