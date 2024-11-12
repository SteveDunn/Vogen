using System;
using MessagePack;
using MessagePack.Formatters;
using N1;
using N2;
using Vogen;
// ReSharper disable UnusedVariable

namespace Testbench;

[MessagePack<MyInt>()]
[MessagePack<MyId>()]
[MessagePack<MyBool>()]
[MessagePack<Name>()]
[MessagePack<MyString>()]
[EfCoreConverter<MyInt>]
internal partial class MyMarkers;

[ValueObject<bool>]
public partial struct MyBool
{
}

public static class Program
{
    public static void Main()
    {
        // Create an instance of the sample class
        var originalObject = new Sample
        {
            Id = MyId.From(123),
            Name = Name.From("Test"),
            Active = MyBool.From(true)
        };


// Caret is currently at line 47

// Create custom resolver with the MyIdFormatter
        var customResolver = MessagePack.Resolvers.CompositeResolver.Create(
            MyMarkers.MessagePackFormatters,
//            new IMessagePackFormatter[] { new MyMarkers.MyIdMessagePackFormatter(), new MyMarkers.NameMessagePackFormatter(), new MyMarkers.MyBoolMessagePackFormatter() },
            new IFormatterResolver[] { MessagePack.Resolvers.StandardResolver.Instance }
        );

        var options = MessagePackSerializerOptions.Standard.WithResolver(customResolver);

        byte[] serializedObject = MessagePackSerializer.Serialize(originalObject, options);


// Deserialize the byte array back to the Sample object using the custom options
        var deserializedObject = MessagePackSerializer.Deserialize<Sample>(serializedObject, options);

// Display the deserialized object
        Console.WriteLine($"Id: {deserializedObject.Id}, Name: {deserializedObject.Name}, Active: {deserializedObject.Active}");

    }
}


[MessagePackObject]
public class Sample
{
    [MessagePack.Key(0)]
    public MyId Id { get; set; }

    [MessagePack.Key(1)] public Name Name { get; set; } = Name.From("");
    [MessagePack.Key(2)] public MyBool Active { get; set; } = MyBool.From(false);
}

[ValueObject<int>]
public partial struct MyId;

[ValueObject<string>]
public partial struct Name;


