using System;
using System.Runtime.Serialization;

namespace Vogen;

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
    
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to GetObjectData
#endif
    protected ValueObjectValidationException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}