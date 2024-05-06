using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Vogen.Diagnostics;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace Vogen;

internal class BuildConfigurationFromAttributes
{
    private readonly bool _hasErroredAttributes;
    private readonly AttributeData _matchingAttribute;

    private readonly List<Diagnostic> _diagnostics;

    private INamedTypeSymbol? _invalidExceptionType;
    private INamedTypeSymbol? _underlyingType;
    private Conversions _conversions;
    private Customizations _customizations;
    private DeserializationStrictness _deserializationStrictness;
    private DebuggerAttributeGeneration _debuggerAttributes;
    private ComparisonGeneration _comparisonGeneration;
    private StringComparersGeneration _stringComparers;
    private CastOperator _fromPrimitiveCasting;
    private CastOperator _toPrimitiveCasting;
    private bool _disableStackTraceGenerationInDebug;
    private ParsableForStrings _parsableForStrings; 
    private ParsableForPrimitives _parsableForPrimitives; 
    private TryFromGeneration _tryFromGeneration; 
    private IsInitializedGeneration _isInitializedGeneration; 

    private BuildConfigurationFromAttributes(AttributeData att)
    {
        _matchingAttribute = att;
        _invalidExceptionType = null;
        _underlyingType = null;
        _conversions = Conversions.Default;
        _customizations = Customizations.None;
        _deserializationStrictness = DeserializationStrictness.Default;
        _debuggerAttributes = DebuggerAttributeGeneration.Default;
        _comparisonGeneration = ComparisonGeneration.Default;
        _parsableForStrings = ParsableForStrings.Unspecified;
        _parsableForPrimitives = ParsableForPrimitives.Unspecified;
        _stringComparers = StringComparersGeneration.Unspecified;
        _fromPrimitiveCasting = CastOperator.Unspecified;
        _toPrimitiveCasting = CastOperator.Unspecified;
        _disableStackTraceGenerationInDebug = false;
        _hasErroredAttributes = false;
        _tryFromGeneration = TryFromGeneration.Unspecified;
        _isInitializedGeneration = IsInitializedGeneration.Unspecified;
        
       _diagnostics = new List<Diagnostic>();
        
        ImmutableArray<TypedConstant> args = _matchingAttribute.ConstructorArguments;

        _hasErroredAttributes = args.Any(a => a.Kind == TypedConstantKind.Error);
    }

    public static VogenConfigurationBuildResult TryBuildFromValueObjectAttribute(AttributeData matchingAttribute) => 
        new BuildConfigurationFromAttributes(matchingAttribute).Build(false);

    public static VogenConfigurationBuildResult TryBuildFromVogenDefaultsAttribute(AttributeData matchingAttribute) => 
        new BuildConfigurationFromAttributes(matchingAttribute).Build(true);

    private VogenConfigurationBuildResult Build(bool argsAreFromVogenDefaultAttribute)
    {
        var isBaseGenericType = _matchingAttribute.AttributeClass!.BaseType!.IsGenericType;
        if (!_matchingAttribute.ConstructorArguments.IsEmpty || isBaseGenericType)
        {
            // make sure we don't have any errors
            ImmutableArray<TypedConstant> args = _matchingAttribute.ConstructorArguments;

            // find which constructor to use, it could be the generic attribute (> C# 11), or the non-generic.
            if (_matchingAttribute.AttributeClass!.IsGenericType || isBaseGenericType)
            {
                PopulateFromGenericValueObjectAttribute(_matchingAttribute, args);
            }
            else
            {
                PopulateFromNonGenericAttribute(args, argsAreFromVogenDefaultAttribute);
            }
        }

        if (_hasErroredAttributes)
        {
            // skip further generator execution and let compiler generate the errors
            return VogenConfigurationBuildResult.Null;
        }

        PopulateDiagnosticsWithAnyValidationIssues();

        return new(
            resultingConfiguration: new VogenConfiguration(
                _underlyingType,
                _invalidExceptionType,
                _conversions,
                _customizations,
                _deserializationStrictness,
                _debuggerAttributes,
                _comparisonGeneration,
                _stringComparers,
                _toPrimitiveCasting,
                _fromPrimitiveCasting,
                _disableStackTraceGenerationInDebug,
                _parsableForStrings,
                _parsableForPrimitives,
                _tryFromGeneration,
                _isInitializedGeneration),
            diagnostics: _diagnostics);
    }

    private void PopulateFromGenericValueObjectAttribute(AttributeData attributeData, ImmutableArray<TypedConstant> args)
    {
        INamedTypeSymbol? attrClassSymbol = attributeData.AttributeClass;

        if (attrClassSymbol is null)
        {
            return;
        }

        var isDerivedFromGenericAttribute =
            attrClassSymbol.BaseType!.FullName()!.StartsWith("Vogen.ValueObjectAttribute<");

        // Extracts the generic argument from the base type when the derived type isn't generic
        // e.g. MyCustomVoAttribute : ValueObjectAttribute<long>
        _underlyingType = isDerivedFromGenericAttribute && attrClassSymbol.TypeArguments.IsEmpty
            ? attrClassSymbol.BaseType!.TypeArguments[0] as INamedTypeSymbol
            : attrClassSymbol.TypeArguments[0] as INamedTypeSymbol;

        PopulateFromValueObjectAttributeArgs(args);
    }

    private void PopulateFromNonGenericAttribute(ImmutableArray<TypedConstant> args, bool argsAreFromVogenDefaultsAttribute)
    {
        _underlyingType = (INamedTypeSymbol?) args[0].Value;

        var argsNotIncludingUnderlyingType = args.Skip(1).ToImmutableArray();
        if (argsAreFromVogenDefaultsAttribute)
        {
            PopulateFromVogenDefaultsAttributeArgs(argsNotIncludingUnderlyingType);
        }
        else
        {
            PopulateFromValueObjectAttributeArgs(argsNotIncludingUnderlyingType);
        }
    }

    private void PopulateDiagnosticsWithAnyValidationIssues()
    {
        var syntax = _matchingAttribute.ApplicationSyntaxReference?.GetSyntax();
        if (syntax is null)
        {
            return;
        }

        var syntaxLocation = syntax.GetLocation();
        
        if (!_conversions.IsValidFlags())
        {
            _diagnostics.Add(DiagnosticsCatalogue.InvalidConversions(syntaxLocation));
        }

        if (!_customizations.IsValidFlags())
        {
            _diagnostics.Add(DiagnosticsCatalogue.InvalidCustomizations(syntaxLocation));
        }

        if (!_deserializationStrictness.IsValidFlags())
        {
            _diagnostics.Add(DiagnosticsCatalogue.InvalidDeserializationStrictness(syntaxLocation));
        }
    }

    // populates all args - it doesn't expect the underlying type argument as that is:
    // * not specified for the generic attribute, and
    // * stripped out (skipped) for the non-generic attribute
    // ReSharper disable once CognitiveComplexity
    private void PopulateFromVogenDefaultsAttributeArgs(ImmutableArray<TypedConstant> argsExcludingUnderlyingType)
    {
        if (argsExcludingUnderlyingType.Length > 12)
        {
            throw new InvalidOperationException("Too many arguments for the attribute.");
        }

        for (int i = argsExcludingUnderlyingType.Length - 1; i >= 0; i--)
        {
            var v = argsExcludingUnderlyingType[i].Value;

            if (v is null)
            {
                continue;
            }

            if (i == 11)
            {
                _isInitializedGeneration = (IsInitializedGeneration) v;
            }

            if (i == 10)
            {
                _tryFromGeneration = (TryFromGeneration) v;
            }

            if (i == 9)
            {
                _parsableForPrimitives = (ParsableForPrimitives) v;
            }

            if (i == 8)
            {
                _parsableForStrings = (ParsableForStrings) v;
            }

            if (i == 7)
            {
                _disableStackTraceGenerationInDebug = (bool) v;
            }

            if (i == 6)
            {
                _toPrimitiveCasting = (CastOperator) v;
            }

            if (i == 5)
            {
                _fromPrimitiveCasting = (CastOperator) v;
            }

            if (i == 4)
            {
                _debuggerAttributes = (DebuggerAttributeGeneration) v;
            }

            if (i == 3)
            {
                _deserializationStrictness = (DeserializationStrictness) v;
            }

            if (i == 2)
            {
                _customizations = (Customizations) v;
            }

            if (i == 1)
            {
                _invalidExceptionType = (INamedTypeSymbol?) v;

                BuildAnyIssuesWithTheException(_invalidExceptionType);
            }

            if (i == 0)
            {
                _conversions = (Conversions) v;
            }
        }
    }

    // ReSharper disable once CognitiveComplexity
    private void PopulateFromValueObjectAttributeArgs(ImmutableArray<TypedConstant> args)
    {
        if (args.Length > 13)
        {
            throw new InvalidOperationException("Too many arguments for the attribute.");
        }

        for (int i = args.Length - 1; i >= 0; i--)
        {
            var v = args[i].Value;

            if (v is null)
            {
                continue;
            }

            if (i == 12)
            {
                _isInitializedGeneration = (IsInitializedGeneration) v;
            }

            if (i == 11)
            {
                _tryFromGeneration = (TryFromGeneration) v;
            }

            if (i == 10)
            {
                _parsableForPrimitives = (ParsableForPrimitives) v;
            }

            if (i == 9)
            {
                _parsableForStrings = (ParsableForStrings) v;
            }

            if (i == 8)
            {
                _fromPrimitiveCasting = (CastOperator) v;
            }
            
            if (i == 7)
            {
                _toPrimitiveCasting = (CastOperator) v;
            }

            if (i == 6)
            {
                _stringComparers = (StringComparersGeneration) v;
            }

            if (i == 5)
            {
                _comparisonGeneration = (ComparisonGeneration) v;
            }

            if (i == 4)
            {
                _debuggerAttributes = (DebuggerAttributeGeneration) v;
            }

            if (i == 3)
            {
                _deserializationStrictness = (DeserializationStrictness) v;
            }

            if (i == 2)
            {
                _customizations = (Customizations) v;
            }

            if (i == 1)
            {
                _invalidExceptionType = (INamedTypeSymbol?) v;

                BuildAnyIssuesWithTheException(_invalidExceptionType);
            }

            if (i == 0)
            {
                _conversions = (Conversions) v;
            }
        }
    }

    private void BuildAnyIssuesWithTheException(INamedTypeSymbol? invalidExceptionType)
    {
        if (invalidExceptionType is null)
        {
            return;
        }

        if (!invalidExceptionType.ImplementsInterfaceOrBaseClass(typeof(Exception)))
        {
            _diagnostics.Add(DiagnosticsCatalogue.CustomExceptionMustDeriveFromException(invalidExceptionType));
        }

        var allConstructors = invalidExceptionType.Constructors.Where(c => c.DeclaredAccessibility == Accessibility.Public);

        var singleParameterConstructors = allConstructors.Where(c => c.Parameters.Length == 1);

        if (singleParameterConstructors.Any(c => c.Parameters.Single().Type.Name == "String"))
        {
            return;
        }

        _diagnostics.Add(DiagnosticsCatalogue.CustomExceptionMustHaveValidConstructor(invalidExceptionType));
    }
}