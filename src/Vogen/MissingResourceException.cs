using System;

namespace Vogen;

internal class MissingResourceException : Exception
{
    public MissingResourceException(string message) : base(message)
    {
    }
}