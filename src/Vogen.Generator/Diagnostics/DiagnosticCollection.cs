using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generator.Diagnostics;

internal class DiagnosticCollection : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _entries = new();
    
    public bool HasErrors { get; private set; }

    private static readonly DiagnosticDescriptor _typeCannotBeNested = CreateDescriptor(
        DiagnosticCode.TypeCannotBeNested,
        "Types cannot be nested",
        "Type '{0}' cannot be nested - remove it from inside {1}");

    private static readonly DiagnosticDescriptor _mustSpecifyUnderlyingType = CreateDescriptor(
        DiagnosticCode.MustSpecifyUnderlyingType,
        "Types cannot be nested",
        "Type '{0}' must specify an underlying type");

    private static readonly DiagnosticDescriptor _underlyingTypeCannotBeCollection = CreateDescriptor(
        DiagnosticCode.UnderlyingTypeCannotBeCollection,
        "Underlying type cannot be collection",
        "Type '{0}' has an underlying type of {1} which is not valid");

    private static readonly DiagnosticDescriptor _underlyingTypeMustNotBeSameAsValueObject = CreateDescriptor(
        DiagnosticCode.UnderlyingTypeMustNotBeSameAsValueObject,
        "Invalid underlying type",
        "Type '{0}' has the same underlying type - must specify a primitive underlying type");

    private static readonly DiagnosticDescriptor _validationMustReturnValidationType = CreateDescriptor(
        DiagnosticCode.ValidationMustReturnValidationType,
        "Validation returns incorrect type",
        "{0} must return a Validation type");

    private static readonly DiagnosticDescriptor _validationMustBeStatic = CreateDescriptor(
        DiagnosticCode.ValidationMustBeStatic,
        "Validation must be static",
        "{0} must be static");

    public void AddTypeCannotBeNested(INamedTypeSymbol typeModel, INamedTypeSymbol container) => 
        AddDiagnostic(_typeCannotBeNested, typeModel.Locations, typeModel.Name, container.Name);

    public void AddValidationMustReturnValidationType(MethodDeclarationSyntax member) => 
        AddDiagnostic(_validationMustReturnValidationType, member.GetLocation(), member.Identifier);

    public void AddValidationMustBeStatic(MethodDeclarationSyntax member) => 
        AddDiagnostic(_validationMustBeStatic, member.GetLocation(), member.Identifier);

    public void AddMustSpecifyUnderlyingType(INamedTypeSymbol underlyingType) => 
        AddDiagnostic(_mustSpecifyUnderlyingType, underlyingType.Locations, underlyingType.Name);

    public void AddUnderlyingTypeMustNotBeSameAsValueObjectType(INamedTypeSymbol underlyingType) => 
        AddDiagnostic(_underlyingTypeMustNotBeSameAsValueObject, underlyingType.Locations, underlyingType.Name);

    public void AddUnderlyingTypeCannotBeCollection(INamedTypeSymbol voClass, INamedTypeSymbol underlyingType) => 
        AddDiagnostic(_underlyingTypeCannotBeCollection, voClass.Locations, voClass.Name, underlyingType);

    private static DiagnosticDescriptor CreateDescriptor(DiagnosticCode code, string title, string messageFormat, DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        string[] tags = severity == DiagnosticSeverity.Error ? new[] { WellKnownDiagnosticTags.NotConfigurable } : Array.Empty<string>();

        return new DiagnosticDescriptor(code.Format(), title, messageFormat, "RestEaseGeneration", severity, isEnabledByDefault: true, customTags: tags);
    }

    private void AddDiagnostic(DiagnosticDescriptor descriptor, IEnumerable<Location> locations, params object?[] args)
    {
        var locationsList = (locations as IReadOnlyList<Location>) ?? locations.ToList();
        
        AddDiagnostic(Diagnostic.Create(descriptor, locationsList.Count == 0 ? Location.None : locationsList[0], locationsList.Skip(1), args));
    }

    private void AddDiagnostic(DiagnosticDescriptor descriptor, Location? location, params object?[] args) => 
        AddDiagnostic(Diagnostic.Create(descriptor, location ?? Location.None, args));

    private void AddDiagnostic(Diagnostic diagnostic)
    {
        if (diagnostic.Severity == DiagnosticSeverity.Error)
        {
            HasErrors = true;
        }
        
        _entries.Add(diagnostic);
    }

    // Try and get the location of the whole 'Foo foo', and not just 'foo'
    private static IEnumerable<Location> SymbolLocations(ISymbol symbol)
    {
        var declaringReferences = symbol.DeclaringSyntaxReferences;

        return declaringReferences.Length > 0
            ? declaringReferences.Select(x => x.GetSyntax().GetLocation())
            : symbol.Locations;
    }

    public IEnumerator<Diagnostic> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}