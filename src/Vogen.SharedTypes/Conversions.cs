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
    ServiceStackDotText = 1 << 7,

    /// <summary>
    /// Creates a BSON serializer for each value object.
    /// </summary>
    Bson = 1 << 8,

    /// <summary>
    /// Creates and registers a codec and copier for Microsoft Orleans.
    /// This feature requires .NET 8 and C#12 and cannot be polly-filled.
    /// </summary>
    Orleans = 1 << 9
}