using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Vogen.Diagnostics;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace Vogen;

internal static class BuildConfigurationFromAttributes
{
    public static VogenConfigurationBuildResult TryBuild(AttributeData matchingAttribute)
    {
        VogenConfigurationBuildResult buildResult = new VogenConfigurationBuildResult();

        INamedTypeSymbol? invalidExceptionType = null;
        INamedTypeSymbol? underlyingType = null;
        Conversions conversions = Conversions.Default;
        Customizations customizations = Customizations.None;
        DeserializationStrictness deserializationStrictness = DeserializationStrictness.Default;
        DebuggerAttributeGeneration debuggerAttributes = DebuggerAttributeGeneration.Default;
        ComparisonGeneration comparisonGeneration = ComparisonGeneration.Default;
        StringComparisonGeneration stringComparison = StringComparisonGeneration.Unspecified;

        bool hasErroredAttributes = false;

        var isBaseGenericType = matchingAttribute.AttributeClass!.BaseType!.IsGenericType;
        if (!matchingAttribute.ConstructorArguments.IsEmpty || isBaseGenericType)
        {
            // make sure we don't have any errors
            ImmutableArray<TypedConstant> args = matchingAttribute.ConstructorArguments;

            foreach (TypedConstant arg in args)
            {
                if (arg.Kind == TypedConstantKind.Error)
                {
                    hasErroredAttributes = true;
                }
            }

            // find which constructor to use, it could be the generic attribute (> C# 11), or the non-generic.
            if (matchingAttribute.AttributeClass!.IsGenericType || isBaseGenericType)
            {
                populateFromGenericAttribute(matchingAttribute, args);
            }
            else
            {
                populateFromNonGenericAttribute(args);
            }
        }

        if (hasErroredAttributes)
        {
            // skip further generator execution and let compiler generate the errors
            return VogenConfigurationBuildResult.Null;
        }

        populateDiagnosticsWithAnyValidationIssues();

        buildResult.ResultingConfiguration = new VogenConfiguration(
            underlyingType,
            invalidExceptionType,
            conversions,
            customizations,
            deserializationStrictness,
            debuggerAttributes,
            comparisonGeneration,
            stringComparison);

        return buildResult;

        void populateFromGenericAttribute(AttributeData attributeData, ImmutableArray<TypedConstant> args)
        {
            INamedTypeSymbol? attrClassSymbol = attributeData.AttributeClass;
            
            if (attrClassSymbol is null) return;
            
            var isDerivedFromGenericAttribute =
                attrClassSymbol.BaseType!.FullName()!.StartsWith("Vogen.ValueObjectAttribute<");

            // Extracts the generic argument from the base type when the derived type isn't generic
            // e.g. MyCustomVoAttribute : ValueObjectAttribute<long>
            underlyingType = isDerivedFromGenericAttribute && attrClassSymbol.TypeArguments.IsEmpty
                ? attrClassSymbol.BaseType!.TypeArguments[0] as INamedTypeSymbol
                : attrClassSymbol.TypeArguments[0] as INamedTypeSymbol;
            
            // there's one less argument because there's no underlying type arguments as it's specified in the generic
            // declaration
            
            populate(args);
        }

        void populateFromNonGenericAttribute(ImmutableArray<TypedConstant> args)
        {
            underlyingType = (INamedTypeSymbol?) args[0].Value;
            
            var skipped = args.Skip(1);
            populate(skipped.ToImmutableArray());
        }

        void populateDiagnosticsWithAnyValidationIssues()
        {
            if (!conversions.IsValidFlags())
            {
                var syntax = matchingAttribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                {
                    buildResult.AddDiagnostic(DiagnosticsCatalogue.InvalidConversions(syntax.GetLocation()));
                }
            }

            if (!customizations.IsValidFlags())
            {
                var syntax = matchingAttribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                {
                    buildResult.AddDiagnostic(DiagnosticsCatalogue.InvalidCustomizations(syntax.GetLocation()));
                }
            }

            if (!deserializationStrictness.IsValidFlags())
            {
                var syntax = matchingAttribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                {
                    buildResult.AddDiagnostic(DiagnosticsCatalogue.InvalidDeserializationStrictness(syntax.GetLocation()));
                }
            }
        }

        // populates all args - it doesn't expect the underlying type argument as that is:
        // * not specified for the generic attribute, and
        // * stripped out (skipped) for the non-generic attribute
        void populate(ImmutableArray<TypedConstant> args)
        {
            if (args.Length > 7)
            {
                throw new InvalidOperationException("Too many arguments for the attribute.");
            }
            
            for (int i = args.Length - 1; i >= 0; i--)
            {
                var v = args[i].Value;
                
                if (v is null)
                    continue;
                
                if (i == 6)
                    stringComparison = (StringComparisonGeneration) v;
                
                if (i == 5)
                    comparisonGeneration = (ComparisonGeneration) v;
            
                if (i == 4)
                    debuggerAttributes = (DebuggerAttributeGeneration) v;
             
                if (i == 3)
                    deserializationStrictness = (DeserializationStrictness) v;
                
                if (i == 2)
                    customizations = (Customizations) v;
                
                if (i == 1)
                {
                    invalidExceptionType = (INamedTypeSymbol?) v;

                    BuildAnyIssuesWithTheException(invalidExceptionType, buildResult);
                }

                if (i == 0)
                    conversions = (Conversions) v;
            }
        }
    }

    private static void BuildAnyIssuesWithTheException(
        INamedTypeSymbol? invalidExceptionType,
        VogenConfigurationBuildResult buildResult)
    {
        if (invalidExceptionType == null)
        {
            return;
        }

        if (!invalidExceptionType.ImplementsInterfaceOrBaseClass(typeof(Exception)))
        {
            buildResult.AddDiagnostic(DiagnosticsCatalogue.CustomExceptionMustDeriveFromException(invalidExceptionType));
        }

        var allConstructors = invalidExceptionType.Constructors.Where(c => c.DeclaredAccessibility == Accessibility.Public);

        var singleParameterConstructors = allConstructors.Where(c => c.Parameters.Length == 1);

        if (singleParameterConstructors.Any(c => c.Parameters.Single().Type.Name == "String"))
        {
            return;
        }

        buildResult.AddDiagnostic(DiagnosticsCatalogue.CustomExceptionMustHaveValidConstructor(invalidExceptionType));
    }
}