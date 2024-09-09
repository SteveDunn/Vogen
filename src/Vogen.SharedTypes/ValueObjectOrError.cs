namespace Vogen;

/// <summary>
/// Represents a value object or an error.
/// Returned by the generated TryFrom method, and allows
/// instances to be checked without need to catch and inspect
/// exceptions.
/// </summary>
/// <typeparam name="T">The type of the wrapper.</typeparam>
public sealed class ValueObjectOrError<T>
{
    private readonly bool _isSuccess;
    private readonly T? _valueObject;
    private readonly Validation? _error;

    /// <summary>
    /// Creates a new instance representing a successful result
    /// containing the supplied value object.
    /// </summary>
    /// <param name="valueObject">The value object</param>
    public ValueObjectOrError(T valueObject)
    {
        _isSuccess = true;
        _valueObject = valueObject;
        _error = Validation.Ok;
    }

    /// <summary>
    /// Creates a new instance representing a failure.
    /// </summary>
    /// <param name="error">The <see cref="Validation"/> object representing the error.</param>
    public ValueObjectOrError(Validation error)
    {
        _isSuccess = false;
        _valueObject = default!;
        _error = error;
    }

    /// <summary>
    /// Determines whether the operation represented by the current instance is successful.
    /// </summary>
    /// <value>
    /// <c>true</c> if the operation is successful; otherwise, <c>false</c>.
    /// </value>
    public bool IsSuccess => _isSuccess;

    /// <summary>
    /// Returns the error, or if successful, a <see cref="Validation"/> representing 'OK'.
    /// </summary>
    public Validation Error => _isSuccess ? Validation.Ok : _error!;

    /// <summary>
    /// Returns the wrapper, or throws an exception if the result failed.
    /// </summary>
    /// <typeparam name="T">The type of the wrapper.</typeparam>
    /// <exception cref="System.InvalidOperationException">Throw when this type holds an error.</exception>
    public T ValueObject =>
        _isSuccess ? _valueObject : throw new System.InvalidOperationException("Cannot access the value object as it is not valid: " + _error!.ErrorMessage);
}
