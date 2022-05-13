using System;

namespace Vogen;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class InstanceAttribute : Attribute
{
    public object Value { get; }

    public string Name { get; }
    
    public string TripleSlashComment { get; }

    public InstanceAttribute(string name, object value, string tripleSlashComment = "") =>
        (Name, Value, TripleSlashComment) = (name, value, tripleSlashComment);
}