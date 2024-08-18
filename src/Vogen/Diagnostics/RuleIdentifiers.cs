namespace Vogen.Diagnostics;

public static class RuleIdentifiers
{
    public const string AddValidationMethod = "AddValidationMethod";
    public const string AddStaticToExistingValidationMethod = "AddStaticToExistingValidationMethod";
    public const string FixInputTypeOfValidationMethod = "FixInputTypeOfValidationMethod";

    public const string AddNormalizeInputMethod = "AddNormalizeInputMethod";
    public const string AddStaticToExistingNormalizeInputMethod = "AddStaticToExistingNormalizeInputMethod";

    public const string TypeCannotBeNested = "VOG001";
    public const string UnderlyingTypeMustNotBeSameAsValueObject = "VOG002";
    public const string UnderlyingTypeCannotBeCollection = "VOG003";
    public const string ValidationMustReturnValidationType = "VOG004";
    public const string ValidateMethodMustBeStatic = "VOG005";
    public const string InstanceMethodCannotHaveNullArgumentName = "VOG006";
    public const string InstanceMethodCannotHaveNullArgumentValue = "VOG007";
    public const string CannotHaveUserConstructors = "VOG008";
    public const string DoNotUseDefault = "VOG009";
    public const string DoNotUseNew = "VOG010";
    public const string InvalidConversions = "VOG011";
    public const string CustomExceptionMustDeriveFromException = "VOG012";
    public const string CustomExceptionMustHaveValidConstructor = "VOG013";
    public const string NormalizeInputMethodMustBeStatic = "VOG014";
    public const string NormalizeInputMethodMustReturnSameUnderlyingType = "VOG015";
    public const string NormalizeInputMethodTakeOneParameterOfUnderlyingType = "VOG016";
    public const string TypeCannotBeAbstract = "VOG017";
    public const string InvalidCustomizations = "VOG019";
    public const string RecordToStringOverloadShouldBeSealed = "VOG020";
    public const string TypeShouldBePartial = "VOG021";
    public const string InvalidDeserializationStrictness = "VOG022";
    public const string InstanceValueCannotBeConverted = "VOG023";
    public const string DuplicateTypesFound = "VOG024";
    public const string DoNotUseReflection = "VOG025";
    public const string DoNotDeriveFromVogenAttributes = "VOG026";
    public const string IncorrectUseOfInstanceField = "VOG027";
    public const string IncorrectUseOfNormalizeInputMethod = "VOG028";
    public const string ExplicitlySpecifyTypeInValueObjectAttribute = "VOG029";
    public const string EfCoreTargetMustExplicitlySpecifyItsPrimitive = "VOG030";
    public const string EfCoreTargetMustBeAVo = "VOG031";
    public const string DoNotThrowFromUserCode = "VOG032";
    public const string UseReadonlyStructInsteadOfStruct = "VOG033";
}