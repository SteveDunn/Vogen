using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace Vogen;

internal static class BuildWorkItems
{
    public static VoWorkItem? TryBuild(VoTarget target,
        SourceProductionContext context,
        VogenConfiguration? globalConfig,
        Compilation compilation)
    {
        TypeDeclarationSyntax voTypeSyntax = target.VoSyntaxInformation;

        INamedTypeSymbol voSymbolInformation = target.VoSymbolInformation;

        if (target.DataForAttributes.Length == 0) return null;

        ImmutableArray<AttributeData> allAttributes = voSymbolInformation.GetAttributes();

        if (target.DataForAttributes.Length != 1)
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.DuplicateTypesFound(voTypeSyntax.GetLocation(), voSymbolInformation.Name));
            return null;
        }
        
        if (!voTypeSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.TypeShouldBePartial(voTypeSyntax.GetLocation(), voSymbolInformation.Name));
            return null;
        }
        
        if (voSymbolInformation.IsAbstract)
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.TypeCannotBeAbstract(voSymbolInformation));
        }

        if (ReportErrorsForAnyUserConstructors(context, voSymbolInformation))
        {
            return null;
        }

        AttributeData voAttribute = target.DataForAttributes[0];

        // Build the configuration but only log issues as diagnostics if they would cause additional compilation errors,
        // such as incorrect exceptions, or invalid customizations. For other issues, there are separate analyzers.
        var localBuildResult = BuildConfigurationFromAttributes.TryBuildFromValueObjectAttribute(voAttribute);
        foreach (var diagnostic in localBuildResult.Diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }

        VogenConfiguration? localConfig = localBuildResult.ResultingConfiguration;
        
        if (localConfig is null)
        {
            return null;
        }

        var config = VogenConfiguration.Combine(
            localConfig,
            globalConfig,
            funcForDefaultUnderlyingType: () => compilation.GetSpecialType(SpecialType.System_Int32));

        ReportErrorIfNestedType(context, voSymbolInformation, target.NestingInfo);

        INamedTypeSymbol underlyingType = config.UnderlyingType ?? throw new InvalidOperationException("No underlying type");
        
        IEnumerable<InstanceProperties> instanceProperties =
            TryBuildInstanceProperties(allAttributes, voSymbolInformation, context, underlyingType).ToList();

        UserProvidedOverloads userProvidedOverloads =
            DiscoverUserProvidedOverloads.Discover(voSymbolInformation, underlyingType);

        ThrowIfToStringOverrideOnRecordIsUnsealed(target, context, userProvidedOverloads.ToStringInfo);

        MethodDeclarationSyntax? validateMethod = null;
        MethodDeclarationSyntax? normalizeInputMethod = null;

        // add any validator or normalize methods it finds
        foreach (var memberDeclarationSyntax in voTypeSyntax.Members)
        {
            if (memberDeclarationSyntax is MethodDeclarationSyntax mds)
            {
                string? methodName = mds.Identifier.Value?.ToString();

                if (TryHandleValidateMethod(methodName, mds, context, compilation))
                {
                    validateMethod = mds;
                }

                if (TryHandleNormalizeMethod(methodName, mds, context, config, target))
                {
                    normalizeInputMethod = mds;
                }
            }
        }

        ReportErrorIfVoTypeIsSameAsUnderlyingType(context, voSymbolInformation, config);

        ReportErrorIfUnderlyingTypeIsCollection(context, config, voSymbolInformation);

        bool isWrapperAValueType = voTypeSyntax switch
        {
            ClassDeclarationSyntax => false,
            StructDeclarationSyntax => true,
            RecordDeclarationSyntax rds when rds.IsKind(SyntaxKind.RecordDeclaration) => false,
            RecordDeclarationSyntax rds when rds.IsKind(SyntaxKind.RecordStructDeclaration) => true,
            _ => false
        };
        
        return new VoWorkItem
        {
            InstanceProperties = instanceProperties.ToList(),
            TypeToAugment = voTypeSyntax,
            
            IsTheUnderlyingAValueType = IsUnderlyingAValueType(config),
            IsTheWrapperAValueType = isWrapperAValueType,
            
            ParsingInformation = BuildParsingInformation(compilation, underlyingType),
            
            UserProvidedOverloads = userProvidedOverloads,
            
            UnderlyingType = underlyingType,
            Conversions = config.Conversions,
            DeserializationStrictness = config.DeserializationStrictness,
            DebuggerAttributes = config.DebuggerAttributes,
            Customizations = config.Customizations,
            TypeForValidationExceptions = config.ValidationExceptionType,
            ComparisonGeneration = config.Comparison,
            StringComparersGeneration = config.StringComparers,
            ParsableForStrings = config.ParsableForStrings,
            TryFromGeneration = config.TryFromGeneration,
            IsInitializedGeneration = config.IsInitializedGeneration,
            ParsableForPrimitives = config.ParsableForPrimitives,
            ToPrimitiveCastOperator = config.ToPrimitiveCasting,
            FromPrimitiveCastOperator = config.FromPrimitiveCasting,
            DisableStackTraceRecordingInDebug = config.DisableStackTraceRecordingInDebug,
            ValidateMethod = validateMethod,
            NormalizeInputMethod = normalizeInputMethod,
            FullNamespace = voSymbolInformation.FullNamespace(),
            IsSealed = voSymbolInformation.IsSealed,
            AccessibilityKeyword = voSymbolInformation.IsInternal() ? "internal" : "public",
            
            WrapperType = voSymbolInformation
        };
    }

    private static ParsingInformation BuildParsingInformation(Compilation compilation, INamedTypeSymbol underlyingType)
    {
        ParsingInformation parsingInformation = new()
        {
            UnderlyingIsAString = underlyingType.SpecialType == SpecialType.System_String,
            IParsableIsAvailable = compilation.GetTypeByMetadataName("System.IParsable`1") is not null,
            IFormatProviderType = compilation.GetTypeByMetadataName("System.IFormatProvider"),
            UnderlyingDerivesFromISpanParsable = DoesPubliclyImplementGenericInterface(underlyingType, compilation.GetTypeByMetadataName("System.ISpanParsable`1")),
            UnderlyingDerivesFromIParsable = DoesPubliclyImplementGenericInterface(underlyingType, compilation.GetTypeByMetadataName("System.IParsable`1")),
            UnderlyingDerivesFromIUtf8SpanParsable = DoesPubliclyImplementGenericInterface(underlyingType, compilation.GetTypeByMetadataName("System.IUtf8SpanParsable`1")),
            
            TryParseMethodsOnThePrimitive = MethodDiscovery.FindTryParseMethodsOnThePrimitive(underlyingType).ToList(),
            ParseMethodsOnThePrimitive = MethodDiscovery.FindParseMethodsOnThePrimitive(underlyingType).ToList(),
        };
        
        return parsingInformation;
    }

    private static bool DoesPubliclyImplementGenericInterface(INamedTypeSymbol underlyingType, INamedTypeSymbol? openGeneric)
    {
        INamedTypeSymbol? closedGeneric = openGeneric?.Construct(underlyingType);
        return MethodDiscovery.DoesPrimitivePubliclyImplementThisInterface(underlyingType, closedGeneric);
    }

    private static void ThrowIfToStringOverrideOnRecordIsUnsealed(VoTarget target,
        SourceProductionContext context,
        UserProvidedToString info)
    {
        if (info is { WasSupplied: true, Method: not null, IsRecordClass: true, IsSealed: false })
        {
            context.ReportDiagnostic(
                DiagnosticsCatalogue.RecordToStringOverloadShouldBeSealed(
                    info.Method.Locations[0],
                    target.VoSymbolInformation.Name));
        }
    }

    private static bool IsUnderlyingAValueType(VogenConfiguration config)
    {
        bool isValueType = true;
        if (config.UnderlyingType is not null)
        {
            isValueType = config.UnderlyingType.IsValueType;
        }

        return isValueType;
    }

    private static void ReportErrorIfUnderlyingTypeIsCollection(SourceProductionContext context,
        VogenConfiguration config,
        INamedTypeSymbol voSymbolInformation)
    {
        if (config.UnderlyingType.ImplementsInterfaceOrBaseClass(typeof(ICollection)))
        {
            context.ReportDiagnostic(
                DiagnosticsCatalogue.UnderlyingTypeCannotBeCollection(voSymbolInformation, config.UnderlyingType!));
        }
    }

    private static void ReportErrorIfVoTypeIsSameAsUnderlyingType(SourceProductionContext context,
        INamedTypeSymbol voSymbolInformation, VogenConfiguration config)
    {
        if (SymbolEqualityComparer.Default.Equals(voSymbolInformation, config.UnderlyingType))
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.UnderlyingTypeMustNotBeSameAsValueObjectType(voSymbolInformation));
        }
    }

    private static void ReportErrorIfNestedType(
        SourceProductionContext context,
        INamedTypeSymbol voSymbolInformation,
        NestingInfo nestingInfo)
    {
        if(nestingInfo.IsNested)
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.TypeCannotBeNested(voSymbolInformation, nestingInfo.ContainingType));
        }
    }

    private static bool ReportErrorsForAnyUserConstructors(SourceProductionContext context, INamedTypeSymbol voSymbolInformation)
    {
        bool reported = false;

        ImmutableArray<IMethodSymbol> allConstructors = voSymbolInformation.Constructors;

        foreach (IMethodSymbol? eachConstructor in allConstructors)
        {
            if (eachConstructor.IsImplicitlyDeclared) continue;

            context.ReportDiagnostic(DiagnosticsCatalogue.CannotHaveUserConstructors(eachConstructor));
            reported = true;
        }

        return reported;
    }


    private static bool TryHandleNormalizeMethod(
        string? methodName, 
        MethodDeclarationSyntax mds,
        SourceProductionContext context, 
        VogenConfiguration config, 
        VoTarget target)
    {
        if (StringComparer.OrdinalIgnoreCase.Compare(methodName, "normalizeinput") != 0)
        {
            return false;
        }

        if (!IsMethodStatic(mds))
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.NormalizeInputMethodMustBeStatic(mds));
            return false;
        }

        if (mds.ParameterList.Parameters.Count != 1)
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.NormalizeInputMethodTakeOneParameterOfUnderlyingType(mds));
            return false;
        }

        if (!AreSameType(mds.ParameterList.Parameters[0].Type, config.UnderlyingType, target.SemanticModel))
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.NormalizeInputMethodTakeOneParameterOfUnderlyingType(mds));
            return false;
        }

        if (!AreSameType(mds.ReturnType, config.UnderlyingType, target.SemanticModel))
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.NormalizeInputMethodMustReturnUnderlyingType(mds));
            return false;
        }

        return true;
    }

    private static bool TryHandleValidateMethod(string? methodName,
        MethodDeclarationSyntax mds,
        SourceProductionContext context,
        Compilation compilation)
    {
        if (StringComparer.OrdinalIgnoreCase.Compare(methodName, "validate") != 0)
        {
            return false;
        }

        if (!IsMethodStatic(mds))
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.ValidationMustBeStatic(mds));
            return false;
        }

        SemanticModel model = compilation.GetSemanticModel(mds.SyntaxTree);

        IMethodSymbol? method = model.GetDeclaredSymbol(mds);
        if (method is null)
        {
            return false;
        }
        
        if(method.ReturnType is INamedTypeSymbol s && s.FullName() != "Vogen.Validation")
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.ValidationMustReturnValidationType(mds));
            return false;
        }

        return true;
    }

    private static bool AreSameType(
        TypeSyntax? typeSyntax,
        INamedTypeSymbol? expectedType,
        SemanticModel targetSemanticModel)
    {
        INamedTypeSymbol? fptSymbol = targetSemanticModel.GetSymbolInfo(typeSyntax!).Symbol as INamedTypeSymbol;
        return SymbolEqualityComparer.Default.Equals(fptSymbol, expectedType);
    }

    private static bool IsMethodStatic(MethodDeclarationSyntax mds) => mds.DescendantTokens().Any(t => t.IsKind(SyntaxKind.StaticKeyword));

    private static IEnumerable<InstanceProperties> TryBuildInstanceProperties(
        ImmutableArray<AttributeData> attributes,
        INamedTypeSymbol voClass,
        SourceProductionContext context, 
        INamedTypeSymbol underlyingType)
    {
        IEnumerable<AttributeData> matchingAttributes =
            attributes.Where(a => a.AttributeClass?.FullName() is "Vogen.InstanceAttribute");

        var props = BuildInstanceProperties.Build(matchingAttributes, context, voClass, underlyingType);
        
        return props.Where(a => a is not null)!;
    }
}