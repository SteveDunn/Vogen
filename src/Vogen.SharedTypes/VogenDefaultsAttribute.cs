namespace Vogen
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class VogenDefaultsAttribute : Attribute
    {
        public VogenDefaultsAttribute(
            Type? underlyingType = null!,
            Conversions conversions = Vogen.Conversions.Default,
            Type? typeOfValidationException = null!)
        {
            UnderlyingType = underlyingType ?? typeof(int);
            TypeOfValidationException = typeOfValidationException ?? typeof(ValueObjectValidationException);
            Conversions = conversions;
        }

        public Type UnderlyingType { get;  } = typeof(int);

        public Type TypeOfValidationException { get; } = typeof(ValueObjectValidationException);

        public Conversions Conversions { get; } = Conversions.Default;
    }
}

