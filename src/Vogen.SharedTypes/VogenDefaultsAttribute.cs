namespace Vogen
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class VogenDefaultsAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of a type that represents the default
        /// values used for value object generation.
        /// </summary>
        /// <param name="underlyingType">The primitive underlying type.</param>
        /// <param name="conversions">Any conversions that need to be done for this type, e.g. to be serialized etc.</param>
        /// <param name="throws">The type of exception that is thrown when validation fails.</param>
        public VogenDefaultsAttribute(
            Type? underlyingType = null!,
            Conversions conversions = Conversions.Default,
            Type? throws = null!)
        {
            UnderlyingType = underlyingType ?? typeof(int);
            TypeOfValidationException = throws ?? typeof(ValueObjectValidationException);
            Conversions = conversions;
        }

        public Type UnderlyingType { get;  } = typeof(int);

        public Type TypeOfValidationException { get; } = typeof(ValueObjectValidationException);

        public Conversions Conversions { get; } = Conversions.Default;
    }
}

