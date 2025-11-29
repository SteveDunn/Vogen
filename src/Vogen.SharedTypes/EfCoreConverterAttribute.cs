using System;

namespace Vogen;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EfCoreConverterAttribute<T> : ConversionMarkerAttribute
{
}

/// <summary>
/// Decorated partial classes with these attributes will ensure that Vogen generates an OpenApi schema for the value object specified.
/// Add multiple attributes to generate multiple Open API registrationss.
/// NOTE: Only Open API Version 2.0 and greater is supported.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class OpenApiMarkerAttribute<T> : ConversionMarkerAttribute
{
}

public class ConversionMarkerAttribute : Attribute
{
}