using System;

namespace Vogen;

/// <summary>
/// Converters used to to serialize/deserialize Value Objects
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
}