namespace Vogen.Diagnostics;

/// <summary>
/// Identifies the type of error / diagnostic encountered during emission
/// </summary>
public enum DiagnosticCode
{
    None = 0,
    TypeCannotBeNested = 1,
    MustSpecifyUnderlyingType = 1,
    UnderlyingTypeMustNotBeSameAsValueObject = 2,
    UnderlyingTypeCannotBeCollection = 3,
    ValidationMustReturnValidationType = 4,
    ValidationMustBeStatic = 5,
    InstanceMethodCannotHaveNullArgumentName = 6,
    InstanceMethodCannotHaveNullArgumentValue = 7,
    CannotHaveUserConstructors = 8,
    UsingDefaultProhibited = 9,
    UsingNewProhibited = 10,
    InvalidConversions = 11
}