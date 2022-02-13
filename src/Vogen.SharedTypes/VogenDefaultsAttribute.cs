namespace Vogen
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class VogenDefaultsAttribute : Attribute
    {
        public VogenDefaultsAttribute(
            Type? typeOfValidationException = null!,
            Conversions conversions = Conversions.Default)
        {
            TypeOfValidationException = typeOfValidationException ?? typeof(ValueObjectValidationException);
            Conversions = conversions;
        }

        public Type TypeOfValidationException { get; }

        public Conversions Conversions { get; }
    }
}

