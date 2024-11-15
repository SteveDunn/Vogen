using System;

namespace Vogen;

/// <summary>
/// Represent a marker for BSON generation.
/// </summary>
/// <typeparam name="T">The type of the value object.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class BsonSerializerAttribute<T> : ConversionMarkerAttribute
{
}