using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Diagnostics;

internal static class DiagnosticItems
{
    private static readonly DiagnosticDescriptor _typeCannotBeNested = CreateDescriptor(
        DiagnosticCode.TypeCannotBeNested,
        "Types cannot be nested",
        "Type '{0}' cannot be nested - remove it from inside {1}");

    private static readonly DiagnosticDescriptor _usingDefaultProhibited = CreateDescriptor(
        DiagnosticCode.UsingDefaultProhibited,
        "Using default of Value Objects is prohibited",
        "Type '{0}' cannot be constructed with default as it is prohibited.");

    private static readonly DiagnosticDescriptor _usingNewProhibited = CreateDescriptor(
        DiagnosticCode.UsingNewProhibited,
        "Using new to create Value Objects is prohibited. Please use the From method for creation.",
        "Type '{0}' cannot be constructed with 'new' as it is prohibited.");

    private static readonly DiagnosticDescriptor _cannotHaveUserConstructors = CreateDescriptor(
        DiagnosticCode.CannotHaveUserConstructors,
        "Cannot have user defined constructors",
        "Cannot have user defined constructors, please use the From method for creation.");

    private static readonly DiagnosticDescriptor _underlyingTypeCannotBeCollection = CreateDescriptor(
        DiagnosticCode.UnderlyingTypeCannotBeCollection,
        "Underlying type cannot be collection",
        "Type '{0}' has an underlying type of {1} which is not valid");

    private static readonly DiagnosticDescriptor _invalidConversions = CreateDescriptor(
        DiagnosticCode.InvalidConversions,
        "Invalid Conversions",
        "The Conversions specified do not match any known conversions - see the Conversions type");

    private static readonly DiagnosticDescriptor _underlyingTypeMustNotBeSameAsValueObject = CreateDescriptor(
        DiagnosticCode.UnderlyingTypeMustNotBeSameAsValueObject,
        "Invalid underlying type",
        "Type '{0}' has the same underlying type - must specify a primitive underlying type");

    private static readonly DiagnosticDescriptor _validationMustReturnValidationType = CreateDescriptor(
        DiagnosticCode.ValidationMustReturnValidationType,
        "Validation returns incorrect type",
        "{0} must return a Validation type");

    private static readonly DiagnosticDescriptor _normalizeInputMethodMustReturnSameUnderlyingType = CreateDescriptor(
        DiagnosticCode.NormalizeInputMethodMustReturnSameUnderlyingType,
        "NormalizeInput returns incorrect type",
        "{0} must return the same underlying type");
    
    private static readonly DiagnosticDescriptor _normalizeInputMethodTakeOneParameterOfUnderlyingType = CreateDescriptor(
        DiagnosticCode.NormalizeInputMethodTakeOneParameterOfUnderlyingType,
        "NormalizeInput accepts wrong type",
        "{0} must accept one parameter of the same type as the underlying type");

    private static readonly DiagnosticDescriptor _validationMustBeStatic = CreateDescriptor(
        DiagnosticCode.ValidationMustBeStatic,
        "Validation must be static",
        "{0} must be static");

    private static readonly DiagnosticDescriptor _normalizeInputMethodMustBeStatic = CreateDescriptor(
        DiagnosticCode.NormalizeInputMethodMustBeStatic,
        "NormalizeInput must be static",
        "{0} must be static");

    private static readonly DiagnosticDescriptor _instanceMethodCannotHaveNullArgumentName = CreateDescriptor(
        DiagnosticCode.InstanceMethodCannotHaveNullArgumentName,
        "Instance attribute cannot have null name",
        "{0} cannot have a null name");

    private static readonly DiagnosticDescriptor _instanceMethodCannotHaveNullArgumentValue = CreateDescriptor(
        DiagnosticCode.InstanceMethodCannotHaveNullArgumentValue,
        "Instance attribute cannot have null value",
        "{0} cannot have a null value");

    private static readonly DiagnosticDescriptor _customExceptionMustDeriveFromException = CreateDescriptor(
        DiagnosticCode.CustomExceptionMustDeriveFromException,
        "Invalid custom exception",
        "{0} must derive from System.Exception");

    private static readonly DiagnosticDescriptor _customExceptionMustHaveValidConstructor = CreateDescriptor(
        DiagnosticCode.CustomExceptionMustHaveValidConstructor,
        "Invalid custom exception",
        "{0} must have at least 1 public constructor with 1 parameter of type System.String");

    public static Diagnostic TypeCannotBeNested(INamedTypeSymbol typeModel, INamedTypeSymbol container) => 
        Create(_typeCannotBeNested, typeModel.Locations, typeModel.Name, container.Name);

    public static Diagnostic ValidationMustReturnValidationType(MethodDeclarationSyntax member) => 
        Create(_validationMustReturnValidationType, member.GetLocation(), member.Identifier);

    public static Diagnostic NormalizeInputMethodMustReturnUnderlyingType(MethodDeclarationSyntax member) => 
        Create(_normalizeInputMethodMustReturnSameUnderlyingType, member.GetLocation(), member.Identifier);

    public static Diagnostic NormalizeInputMethodTakeOneParameterOfUnderlyingType(MethodDeclarationSyntax member) => 
        Create(_normalizeInputMethodTakeOneParameterOfUnderlyingType, member.GetLocation(), member.Identifier);

    public static Diagnostic ValidationMustBeStatic(MethodDeclarationSyntax member) => 
        Create(_validationMustBeStatic, member.GetLocation(), member.Identifier);

    public static Diagnostic NormalizeInputMethodMustBeStatic(MethodDeclarationSyntax member) => 
        Create(_normalizeInputMethodMustBeStatic, member.GetLocation(), member.Identifier);

    public static Diagnostic UsingDefaultProhibited(Location locationOfDefaultStatement, string voClassName) => 
        BuildDiagnostic(_usingDefaultProhibited, voClassName, locationOfDefaultStatement);

    public static Diagnostic UsingNewProhibited(Location location, string voClassName) => 
        BuildDiagnostic(_usingNewProhibited, voClassName, location);

    public static Diagnostic CannotHaveUserConstructors(IMethodSymbol constructor) => 
        Create(_cannotHaveUserConstructors, constructor.Locations);

    public static Diagnostic UnderlyingTypeMustNotBeSameAsValueObjectType(INamedTypeSymbol underlyingType) => 
        Create(_underlyingTypeMustNotBeSameAsValueObject, underlyingType.Locations, underlyingType.Name);

    public static Diagnostic UnderlyingTypeCannotBeCollection(INamedTypeSymbol voClass, INamedTypeSymbol underlyingType) => 
        Create(_underlyingTypeCannotBeCollection, voClass.Locations, voClass.Name, underlyingType);

    public static Diagnostic InvalidConversions(Location location) => Create(_invalidConversions, location);

    public static Diagnostic InstanceMethodCannotHaveNullArgumentName(INamedTypeSymbol voClass) => 
        Create(_instanceMethodCannotHaveNullArgumentName, voClass.Locations, voClass.Name);

    public static Diagnostic InstanceMethodCannotHaveNullArgumentValue(INamedTypeSymbol voClass) => 
        Create(_instanceMethodCannotHaveNullArgumentValue, voClass.Locations, voClass.Name);

    public static Diagnostic CustomExceptionMustDeriveFromException(INamedTypeSymbol symbol) => 
        Create(_customExceptionMustDeriveFromException, symbol.Locations, symbol.Name);

    public static Diagnostic CustomExceptionMustHaveValidConstructor(INamedTypeSymbol symbol) => 
        Create(_customExceptionMustHaveValidConstructor, symbol.Locations, symbol.Name);

    private static DiagnosticDescriptor CreateDescriptor(DiagnosticCode code, string title, string messageFormat, DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        string[] tags = severity == DiagnosticSeverity.Error ? new[] { WellKnownDiagnosticTags.NotConfigurable } : Array.Empty<string>();

        return new DiagnosticDescriptor(code.Format(), title, messageFormat, "Vogen", severity, isEnabledByDefault: true, customTags: tags);
    }

    private static Diagnostic Create(DiagnosticDescriptor descriptor, string name, Location location) => Diagnostic.Create(descriptor, location, name);

    private static Diagnostic BuildDiagnostic(DiagnosticDescriptor descriptor, string name, Location location) => 
        Diagnostic.Create(descriptor, location, name);

    private static Diagnostic Create(DiagnosticDescriptor descriptor, IEnumerable<Location> locations, params object?[] args)
    {
        var locationsList = (locations as IReadOnlyList<Location>) ?? locations.ToList();

        Diagnostic diagnostic = Diagnostic.Create(
            descriptor, 
            locationsList.Count == 0 ? Location.None : locationsList[0],
            locationsList.Skip(1), 
            args);

        return diagnostic;
    }

    private static Diagnostic Create(DiagnosticDescriptor descriptor, Location? location, params object?[] args) => Diagnostic.Create(descriptor, location ?? Location.None, args);
}