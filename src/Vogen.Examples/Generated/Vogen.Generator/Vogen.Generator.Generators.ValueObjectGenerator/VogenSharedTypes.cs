
namespace Vogen
{
    using System;
    using System.Runtime.Serialization;

    public class Validation
    {
        public string ErrorMessage { get; }

        public static readonly Validation Ok = new Validation(string.Empty);

        private Validation(string reason) => ErrorMessage = reason;

        public static Validation Invalid(string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
            {
                return new Validation("[none provided]");
    }

    return new Validation(reason);
}
}

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

[Serializable]
public class ValueObjectValidationException : Exception
{
    public ValueObjectValidationException()
    {
    }

    public ValueObjectValidationException(string message) : base(message)
    {
    }

    public ValueObjectValidationException(string message, Exception inner) : base(message, inner)
    {
    }

    protected ValueObjectValidationException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
}