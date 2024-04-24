# Serialize Value Objects

<note>
This topic is incomplete and is currently being improved.
</note>


Vogen integrates with other systems and technologies.

The generated Value Objects can be converted to and from JSON.

They can be used in Dapper, LinqToDB, and EF Core.

And it generates TypeConverter code, so that Value Objects can be used in things like ASP.NET Core MVC routes.

Integration is handled by the `conversions` parameter in the `ValueObject` attribute. The current choices are:

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
    /// This will be the value provided in the <see cref="ValueObjectAttribute"/>, which falls back to
    /// <see cref="TypeConverter"/> and <see cref="NewtonsoftJson"/>
    /// </summary>
    Default = TypeConverter | SystemTextJson,

    /// <summary>
    /// Creates a <see cref="TypeConverter"/> for converting from the value object to and from a string
    /// </summary>
    TypeConverter = 1 << 1,

    /// <summary>
    /// Creates a Newtonsoft.Json.JsonConverter for serializing the value object to its primitive value
    /// </summary>
    NewtonsoftJson = 1 << 2,

    /// <summary>
    /// Creates a System.Text.Json.Serialization.JsonConverter for serializing the value object to its primitive value
    /// </summary>
    SystemTextJson = 1 << 3,

    /// <summary>
    /// Creates an EF Core Value Converter for extracting the primitive value
    /// </summary>
    EfCoreValueConverter = 1 << 4,

    /// <summary>
    /// Creates a Dapper TypeHandler for converting to and from the type
    /// </summary>
    DapperTypeHandler = 1 << 5,

    /// <summary>
    /// Creates a LinqToDb ValueConverter for converting to and from the type
    /// </summary>
    LinqToDbValueConverter = 1 << 6,
    
    /// <summary>
    /// Sets the SerializeFn and DeSerializeFn members in JsConfig in a static constructor.
    /// </summary>
    ServiceStackDotText = 1 << 7
}
```

The default, as specified above in the `Defaults` property, is `TypeConverter` and `SystemTextJson`.

[//]: # (TODO: merge this in)

Other converters/serializers are:

* Newtonsoft.Json (NSJ)
* ServiceStack.Text
* Dapper
* EFCore
* [LINQ to DB](https://github.com/linq2db/linq2db)
* protobuf-net (see the FAQ section below)

They are controlled by the `Conversions` enum. The following has serializers for NSJ and STJ:

```c#
[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(float))]
public readonly partial struct Celsius { }
```

If you don't want any conversions, then specify `Conversions.None`.

If you want your own conversion, then again specify none, and implement them yourself, just like any other type.  But be aware that even serializers will get the same compilation errors for `new` and `default` when trying to create VOs.

If you want to use Dapper, remember to register it—something like this:

```c#
SqlMapper.AddTypeHandler(new Customer.DapperTypeHandler());
```

See the examples folder for more information.
