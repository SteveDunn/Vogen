using System;

namespace Vogen.SharedTypes
{

    public class ValueObjectAttribute : Attribute
    {
        public Type UnderlyingType { get; }

        public ValueObjectAttribute(Type underlyingType)
        {
            UnderlyingType = underlyingType;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class InstanceAttribute : Attribute
    {
        public object Value { get; }
        
        public string Name { get; }

        public InstanceAttribute(string name, object value) => (Name, Value) = (name, value);
    }
}