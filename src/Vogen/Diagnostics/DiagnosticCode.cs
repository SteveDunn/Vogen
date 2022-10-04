namespace Vogen.Diagnostics;

/// <summary>
/// Identifies the type of error / diagnostic encountered during emission
/// </summary>
public enum DiagnosticCode
{
    None = 0,
    TypeCannotBeNested = 1,
    UnderlyingTypeMustNotBeSameAsValueObject = 2,
    UnderlyingTypeCannotBeCollection = 3,
    ValidationMustReturnValidationType = 4,
    ValidationMustBeStatic = 5,
    InstanceMethodCannotHaveNullArgumentName = 6,
    InstanceMethodCannotHaveNullArgumentValue = 7,
    CannotHaveUserConstructors = 8,
    UsingDefaultProhibited = 9,
    InvalidConversions = 11,
    CustomExceptionMustDeriveFromException = 12,
    CustomExceptionMustHaveValidConstructor = 13,
    NormalizeInputMethodMustBeStatic = 14,
    NormalizeInputMethodMustReturnSameUnderlyingType = 15,
    NormalizeInputMethodTakeOneParameterOfUnderlyingType = 16,
    TypeCannotBeAbstract = 17,
    InvalidCustomizations = 19,
    RecordToStringOverloadShouldBeSealed = 20,
    TypeShouldBePartial = 21,
    InvalidDeserializationStrictness = 22,
    InstanceValueCannotBeConverted = 23,
    DuplicateTypesFound = 24,
    UsingActivatorProhibited = 25
}