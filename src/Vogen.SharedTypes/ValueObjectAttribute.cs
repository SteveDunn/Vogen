namespace Vogen
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class ValueObjectAttribute : Attribute
    {
        public ValueObjectAttribute(
            Type? underlyingType = null!,
            Conversions conversions = Conversions.Default)
        {
            UnderlyingType = underlyingType;
            Conversions = conversions;
        }

        public Type? UnderlyingType { get; }

        public Conversions Conversions { get; }
    }
}

