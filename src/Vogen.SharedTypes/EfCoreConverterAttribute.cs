using System;

namespace Vogen;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EfCoreConverterAttribute<T> : Attribute
{
}