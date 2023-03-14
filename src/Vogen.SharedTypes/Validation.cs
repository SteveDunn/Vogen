using System.Collections;
using System.Collections.Generic;

namespace Vogen;

public class Validation
{
    public string ErrorMessage { get; }

    /// <summary>
    /// Contains data related to validation.
    /// </summary>
    public Dictionary<object, object> Data { get; } = new Dictionary<object, object>();

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

    /// <summary>
    /// Adds the specified data to the validation.
    /// This data will be copied to the Data property of the thrown Exception.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>Validation.</returns>
    public Validation WithData(object key, object value)
    {
        Data[key] = value;
        return this;
    }
}