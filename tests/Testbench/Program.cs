using System;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using N1;
using N2;
using Testbench.SubNamespace;
using Vogen;
// ReSharper disable UnusedVariable

namespace Testbench;

[MessagePack<MyInt>()]
[MessagePack<MyId>()]
[MessagePack<MyBool>()]
[MessagePack<Name>()]
[MessagePack<StartDate>()]
[MessagePack<MyString>()]
[EfCoreConverter<MyInt>]
[BsonSerializer<MyInt>]
[BsonSerializer<classFromEscapedNamespaceWithReservedUnderlyingType>]
public partial class MyMarkers;

[ValueObject<bool>]
public partial struct MyBool
{
}

public readonly record struct @decimal;

[ValueObject(typeof(@decimal))]
public partial class classFromEscapedNamespaceWithReservedUnderlyingType
{
}


public static class Program
{
    public static void Main()
    {
        // Create an instance of the sample class
        var originalObject = new Sample
        {
            Id = MyId.From(Guid.NewGuid()),
            Name = Name.From("Test"),
            StartDate = StartDate.From(DateTimeOffset.Now),
            Active = MyBool.From(true)
        };


        IMessagePackFormatter[] messagePackFormatters = MyMarkers.MessagePackFormatters;
        
//messagePackFormatters = messagePackFormatters.Append(new MyGuidFormatter()).ToArray();
        
        var customResolver = MessagePack.Resolvers.CompositeResolver.Create(
            messagePackFormatters,
//            new IMessagePackFormatter[] { new MyMarkers.MyIdMessagePackFormatter(), new MyMarkers.NameMessagePackFormatter(), new MyMarkers.MyBoolMessagePackFormatter() },
            [MessagePack.Resolvers.StandardResolver.Instance]
        );

        var options = MessagePackSerializerOptions.Standard.WithResolver(customResolver);

        byte[] serializedObject = MessagePackSerializer.Serialize(originalObject, options);


// Deserialize the byte array back to the Sample object using the custom options
        var deserializedObject = MessagePackSerializer.Deserialize<Sample>(serializedObject, options);

// Display the deserialized object
        Console.WriteLine($"Id: {deserializedObject.Id}, Name: {deserializedObject.Name}, Active: {deserializedObject.Active}, StartDate: {deserializedObject.StartDate:o}");

    }
}

// public class MyGuidFormatter : IMessagePackFormatter<MyId>
// {
//     public void Serialize(ref MessagePackWriter writer, MyId value, MessagePackSerializerOptions options)
//     {
//         IMessagePackFormatter<Guid>? r = StandardResolver.Instance.GetFormatter<Guid>();
//         if (r is null) throw new MessagePackSerializationException("");
//         r.Serialize(ref writer, value.Value, options);
//     }
//     
//     public MyId Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
//     {
//         IMessagePackFormatter<Guid>? r = StandardResolver.Instance.GetFormatter<Guid>();
//         if (r is null) throw new MessagePackSerializationException("");
//         Guid? g = r?.Deserialize(ref reader, options);
//         
//         return MyId.From(g!.Value);
//     }
// }

[MessagePackObject]
public class Sample
{
    [MessagePack.Key(0)]
    public MyId Id { get; set; }

    [MessagePack.Key(1)] public Name Name { get; set; } = Name.From("");
    [MessagePack.Key(2)] public MyBool Active { get; set; } = MyBool.From(false);
    [MessagePack.Key(3)] public StartDate StartDate { get; set; }
}

[ValueObject<DateTimeOffset>]
public partial struct StartDate;

[ValueObject<Guid>]
public partial struct MyId;