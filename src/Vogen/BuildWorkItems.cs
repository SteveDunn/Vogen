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
        LanguageVersion languageVersion,
        VogenKnownSymbols vogenKnownSymbols,
        Compilation compilation)
    {
        TypeDeclarationSyntax voTypeSyntax = target.VoSyntaxInformation;
        
        bool showNullAnnotations = ShouldShowNullAnnotations(compilation, target);

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

        var config = CombineConfigurations.CombineAndResolveAnythingUnspecified(
            localConfig,
            globalConfig,
            funcForDefaultUnderlyingType: () => vogenKnownSymbols.Int32);

        ReportErrorIfNestedType(context, voSymbolInformation, target.NestingInfo);

        if (config.UnderlyingType is null)
        {
            return null;
        }

        INamedTypeSymbol underlyingType = config.UnderlyingType;
        
        IEnumerable<InstanceProperties> instanceProperties =
            TryBuildInstanceProperties(allAttributes, voSymbolInformation, context, underlyingType).ToList();

        UserProvidedOverloads userProvidedOverloads =
            DiscoverUserProvidedOverloads.Discover(voSymbolInformation, underlyingType);

        ThrowIfAnyToStringOverrideOnRecordIsUnsealed(target, context, userProvidedOverloads.ToStringOverloads);

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

        var isTheUnderlyingAValueType = IsUnderlyingAValueType(config);
        return new VoWorkItem
        {
            Nullable = new(showNullAnnotations, !isWrapperAValueType, !isTheUnderlyingAValueType),
            LanguageVersion = languageVersion,
            InstanceProperties = instanceProperties.ToList(),
            TypeToAugment = voTypeSyntax,
            
            IsTheUnderlyingAValueType = isTheUnderlyingAValueType,
            IsTheWrapperAValueType = isWrapperAValueType,
            
            ParsingInformation = BuildParsingInformation(underlyingType, vogenKnownSymbols),
            ToStringInformation = BuildToStringInformation(underlyingType, vogenKnownSymbols),
            Config = config,
            
            UserProvidedOverloads = userProvidedOverloads,
            
            UnderlyingType = underlyingType,
            ValidateMethod = validateMethod,
            NormalizeInputMethod = normalizeInputMethod,
            FullNamespace = voSymbolInformation.FullNamespace(),
            IsSealed = voSymbolInformation.IsSealed,
            AccessibilityKeyword = voSymbolInformation.IsInternal() ? "internal" : "public",
            DefaultValidationExceptionSymbol = vogenKnownSymbols.ValueObjectValidationException,
            
            WrapperType = voSymbolInformation,
        };
    }

    private static bool ShouldShowNullAnnotations(Compilation compilation, VoTarget target)
    {
        SemanticModel sm = compilation.GetSemanticModel(target.VoSyntaxInformation.SyntaxTree);
        NullableContext nullableContext = sm.GetNullableContext(target.VoSyntaxInformation.GetLocation().SourceSpan.Start);
        
        if (nullableContext.HasFlag(NullableContext.AnnotationsContextInherited))
        {
            return (compilation.Options as CSharpCompilationOptions)!.NullableContextOptions.HasFlag(NullableContextOptions.Enable);
        }

        return nullableContext.HasFlag(NullableContext.AnnotationsEnabled);
    }

    private static ParsingInformation BuildParsingInformation(INamedTypeSymbol underlyingType, VogenKnownSymbols vogenKnownSymbols)
    {
        ParsingInformation parsingInformation = new()
        {
            UnderlyingIsAString = underlyingType.SpecialType == SpecialType.System_String,
            IParsableIsAvailable = vogenKnownSymbols.IParsableOfT is not null,
            IFormatProviderType = vogenKnownSymbols.IFormatProvider,
            UnderlyingDerivesFromISpanParsable = DoesPubliclyImplementGenericInterface(underlyingType, vogenKnownSymbols.ISpanParsableOfT),
            UnderlyingDerivesFromIParsable = DoesPubliclyImplementGenericInterface(underlyingType, vogenKnownSymbols.IParsableOfT),
            UnderlyingDerivesFromIUtf8SpanParsable = DoesPubliclyImplementGenericInterface(underlyingType, vogenKnownSymbols.IUtf8SpanParsableOfT),
            
            TryParseMethodsOnThePrimitive = MethodDiscovery.FindTryParseMethodsOnThePrimitive(underlyingType).ToList(),
            ParseMethodsOnThePrimitive = MethodDiscovery.FindParseMethodsOnThePrimitive(underlyingType).ToList(),
        };
        
        return parsingInformation;
    }

    private static ToStringInformation BuildToStringInformation(INamedTypeSymbol underlyingType, VogenKnownSymbols vogenKnownSymbols)
    {
        ToStringInformation info = new()
        {
            UnderlyingIsAString = underlyingType.SpecialType == SpecialType.System_String,
            ToStringMethodsOnThePrimitive = MethodDiscovery.FindToStringMethodsOnThePrimitive(underlyingType).ToList(),
        };
        
        return info;
    }

    private static bool DoesPubliclyImplementGenericInterface(INamedTypeSymbol underlyingType, INamedTypeSymbol? openGeneric)
    {
        INamedTypeSymbol? closedGeneric = openGeneric?.Construct(underlyingType);
        return MethodDiscovery.DoesPrimitivePubliclyImplementThisInterface(underlyingType, closedGeneric);
    }

    private static void ThrowIfAnyToStringOverrideOnRecordIsUnsealed(VoTarget target,
        SourceProductionContext context,
        UserProvidedToStringMethods infos)
    {
        foreach (IMethodSymbol info in infos)
        {
            if(target.VoSymbolInformation.IsRecordClass() && !info.IsSealed)
            {
                context.ReportDiagnostic(
                    DiagnosticsCatalogue.RecordToStringOverloadShouldBeSealed(
                        info.Locations[0],
                        target.VoSymbolInformation.Name));
            }
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