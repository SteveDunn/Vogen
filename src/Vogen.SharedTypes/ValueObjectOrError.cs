namespace Vogen;

public sealed class ValueObjectOrError<T>
{
    private readonly bool _isSuccess;
    private readonly T _valueObject;
    private readonly Validation _error;

    public ValueObjectOrError(T valueObject)
    {
        _isSuccess = true;
        _valueObject = valueObject;
        _error = Validation.Ok;
    }

    public ValueObjectOrError(Validation error)
    {
        _isSuccess = false;
        _valueObject = default;
        _error = error;
    }

    public bool IsSuccess => _isSuccess;
        
    public Validation Error => _isSuccess ? Validation.Ok : _error;

    public T ValueObject =>
        _isSuccess ? _valueObject : throw new System.InvalidOperationException("Cannot access the value object as it is not valid: " + _error.ErrorMessage);
}
