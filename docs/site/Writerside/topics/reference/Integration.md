# Serialize Value Objects

Vogen integrates with various serializers and conversion mechanisms, including:

* **IConvertible** - Automatic for primitives that implement it, enabling dynamic type conversion and framework integration
* JSON (`System.Text.Json` and `Newtonsoft.Json`)
* BSON
* Dapper
* LinqToDB
* EF Core
* ASP.NET Core (for MVC routes etc.), by generating a `TypeConverter`
* protobuf-net (see the section in the [FAQ](FAQ.md#can-i-use-protobuf-net) for usage)

… and many others. See the `Conversions` attribute for a full list.

## IConvertible - Automatic Hoisting

When the underlying primitive type implements `System.IConvertible` (such as `int`, `float`, `decimal`, `DateTime`), Vogen automatically generates the `IConvertible` interface on your value object. This enables seamless integration with .NET APIs that expect `IConvertible` types, such as `Convert.ChangeType()`, data mapping frameworks, and reflection-based utilities.

**This is automatic and requires no configuration.** See [IConvertible](IConvertible.md) for detailed information.

## Explicit Conversion Configuration

For other conversion mechanisms, you control what to generate using the `conversions` parameter.

### Conversion code in the same project

Here are two types where we want serializers for MessagePack and Mongo's BSON format:

```c#
[ValueObject<string>(
    conversions: Conversions.MessagePack | Conversions.Bson)]
public readonly partial struct Name { }

[ValueObject<int>(
    conversions: Conversions.MessagePack | Conversions.Bson)]
public readonly partial struct Age { }
```

If you're following an architecture pattern where you separate infrastructure from other layers, then you'll want the generated converters to live in another project. This is described below.

## Conversion code in a different project

<note>
Currently, the following conversions can be used from another project, but as Vogen evolves other conversions will be supported (please raise a GitHub issue if you'd like to see something implemented):

* BSON
* EFCore
* MessagePack

</note>

Create a `partial class` (not a `struct`), and specify what converters you want for what type. For the two examples above, it'd be:

```c#
[BsonSerializer<Domain.Name>]
[BsonSerializer<Domain.Age>]
public partial class BsonConversions;

[MessagePack<Domain.Name>]
[MessagePack<Domain.Age>]
public partial class MessagePackConversions;
```

... or just have them in one class:

```c#
[BsonSerializer<Domain.Name>]
[MessagePack<Domain.Name>]
[BsonSerializer<Domain.Age>]
[MessagePack<Domain.Age>]
public partial class Conversions;
```

And reference them with something like `Conversions.NameBsonSerializer` etc. Here's an example to register all MessagePack formatters:  

```c#
        var customResolver = MessagePack.Resolvers.CompositeResolver.Create(
            Conversions.MessagePackFormatters,
            [MessagePack.Resolvers.StandardResolver.Instance]
        );
```

## Integrations

For generating conversions in the same project, use the `conversions` parameter in the `ValueObject` attribute:

```c#
using System;

namespace Vogen;

/// <summary>
/// Converters used to convert/serialize/deserialize Value Objects
/// </summary>
[Flags]
public enum Conversions
{
    // Used with HasFlag, so needs to be 1, 2, 4 etc

    /// <summary>
    /// Don't create any converters for the value object
    /// </summary>
    None = 0,

    /// <summary>
    /// Use the default converters for the value object.
    /// This will be the value provided in the 
    ///     <see cref="ValueObjectAttribute"/>, which falls back to
    /// <see cref="TypeConverter"/> and <see cref="NewtonsoftJson"/>
    /// </summary>
    Default = TypeConverter | SystemTextJson,

    /// <summary>
    /// Creates a <see cref="TypeConverter"/> for converting from the
    ///     value object to and from a string
    /// </summary>
    TypeConverter = 1 << 1,

    /// <summary>
    /// Creates a Newtonsoft.Json.JsonConverter for serializing the 
    ///     value object to its primitive value
    /// </summary>
    NewtonsoftJson = 1 << 2,

    /// <summary>
    /// Creates a System.Text.Json.Serialization.JsonConverter for 
    ///     serializing the value object to its primitive value
    /// </summary>
    SystemTextJson = 1 << 3,

    /// <summary>
    /// Creates an EF Core Value Converter for extracting the 
    ///     primitive value
    /// </summary>
    EfCoreValueConverter = 1 << 4,

    /// <summary>
    /// Creates a Dapper TypeHandler for converting to and 
    ///     from the type
    /// </summary>
    DapperTypeHandler = 1 << 5,

    /// <summary>
    /// Creates a LinqToDb ValueConverter for converting to and 
    ///     from the type
    /// </summary>
    LinqToDbValueConverter = 1 << 6,
    
    /// <summary>
    /// Sets the SerializeFn and DeSerializeFn members in JsConfig 
    ///     in a static constructor.
    /// </summary>
    ServiceStackDotText = 1 << 7,

    /// <summary>
    /// Creates a BSON serializer for each value object.
    /// </summary>
    Bson = 1 << 8,
    
    /// <summary>
    /// Creates and registers a codec and copier 
    ///     for Microsoft Orleans.
    /// This feature requires .NET 8 and C#12 and 
    ///     cannot be polly-filled.
    /// </summary>
    Orleans = 1 << 9,

    /// <summary>
    /// Generates implementation of IXmlSerializable.
    /// </summary>
    XmlSerializable = 1 << 10,

    /// <summary>
    /// Generates implementation of IMessagePackFormatter.
    /// </summary>
    MessagePack = 1 << 11
}
```

The default, as specified above in the `Defaults` property, is `TypeConverter` and `SystemTextJson`.

If you don't want any conversions, then specify `Conversions.None`.

If you want your own conversion, then again specify none and implement them yourself, just like any other type. Be aware that even serializers will get the same compilation errors for `new` and `default` when trying to create VOs.

There may be other steps that you need to do to use these integrations, for instance, for Dapper, register it—something like this:

```c#
SqlMapper.AddTypeHandler(new Customer.DapperTypeHandler());
```

See the [examples folder](https://github.com/SteveDunn/Vogen/tree/main/samples/Vogen.Examples/SerializationAndConversion) for more information.



