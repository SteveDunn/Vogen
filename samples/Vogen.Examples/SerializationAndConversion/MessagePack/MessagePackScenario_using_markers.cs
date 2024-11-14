#if NULLABLE_DISABLED_BUILD
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#endif

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MessagePack;
using MessagePack.Formatters;

namespace Vogen.Examples.SerializationAndConversion.MessagePackScenario.UsingMarkers;

[ValueObject<int>]
public readonly partial struct Age;

[ValueObject<int>]
public readonly partial struct PersonId;

[ValueObject<string>]
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

[MessagePack<PersonId>]
[MessagePack<Name>]
[MessagePack<Age>]
public partial class Markers;

[UsedImplicitly]
public class MessagePackScenario_using_markers : IScenario
{
    public Task Run()
    {
        var customResolver = MessagePack.Resolvers.CompositeResolver.Create(
            Markers.MessagePackFormatters,
            [MessagePack.Resolvers.StandardResolver.Instance]
        );

        var options = MessagePackSerializerOptions.Standard.WithResolver(customResolver);

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
