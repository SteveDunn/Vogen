using System;

namespace Vogen;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class InstanceAttribute : Attribute
{
    public object Value { get; }

    public string Name { get; }

    public InstanceAttribute(string name, object value) => (Name, Value) = (name, value);
}