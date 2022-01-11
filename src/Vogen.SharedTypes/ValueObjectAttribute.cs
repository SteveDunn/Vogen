namespace Vogen
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class ValueObjectAttribute : Attribute
    {
        public Type UnderlyingType { get; }
        public Conversions Conversions { get; }

        public ValueObjectAttribute(
            Type underlyingType,
            Conversions conversions = Conversions.Default)
        {
            UnderlyingType = underlyingType;
            Conversions = conversions;
        }
    }
}

