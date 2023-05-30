using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;

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
        var localBuildResult = ManageAttributes.TryBuildConfigurationFromAttribute(voAttribute);
        foreach (var diagnostic in localBuildResult.Diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }

        VogenConfiguration? localConfig = localBuildResult.ResultingConfiguration;
        
        if (localConfig == null)
        {
            return null;
        }

        var config = VogenConfiguration.Combine(localConfig.Value, globalConfig, () => compilation.GetSpecialType(SpecialType.System_Int32));

        ReportErrorIfNestedType(target, context, voSymbolInformation);

        IEnumerable<InstanceProperties> instanceProperties =
            TryBuildInstanceProperties(allAttributes, voSymbolInformation, context, config.UnderlyingType).ToList();

        var toStringInfo = HasToStringOverload(voSymbolInformation);

        ThrowIfToStringOverrideOnRecordIsUnsealed(target, context, toStringInfo);

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

        var isValueType = IsUnderlyingAValueType(config);

        return new VoWorkItem
        {
            InstanceProperties = instanceProperties.ToList(),
            TypeToAugment = voTypeSyntax,
            IsValueType = isValueType,
            HasToString = toStringInfo.HasToString,
            UnderlyingType = config.UnderlyingType ?? throw new InvalidOperationException("No underlying type"),
            Conversions = config.Conversions,
            DeserializationStrictness = config.DeserializationStrictness,
            DebuggerAttributes = config.DebuggerAttributes,
            Customizations = config.Customizations,
            TypeForValidationExceptions = config.ValidationExceptionType,
            ValidateMethod = validateMethod,
            NormalizeInputMethod = normalizeInputMethod,
            FullNamespace = voSymbolInformation.FullNamespace()
        };
    }

    private static void ThrowIfToStringOverrideOnRecordIsUnsealed(VoTarget target,
        SourceProductionContext context,
        ToStringInfo info)
    {
        if (info is { HasToString: true, Method: not null, IsRecordClass: true, IsSealed: false })
        {
            context.ReportDiagnostic(
                DiagnosticsCatalogue.RecordToStringOverloadShouldBeSealed(
                    info.Method.Locations[0],
                    target.VoSymbolInformation.Name));
        }
    }

    private record struct ToStringInfo(bool HasToString, bool IsRecordClass, bool IsSealed, IMethodSymbol? Method);

    private static ToStringInfo HasToStringOverload(ITypeSymbol typeSymbol)
    {
        while (true)
        {
            var toStringMethods = typeSymbol.GetMembers("ToString").OfType<IMethodSymbol>();

            foreach (IMethodSymbol eachMethod in toStringMethods)
            {
                // we could have "public virtual new string ToString() => "xxx" 
                if (!eachMethod.IsOverride && !eachMethod.IsVirtual)
                {
                    continue;
                }

                // can't change access rights
                if (eachMethod.DeclaredAccessibility != Accessibility.Public && eachMethod.DeclaredAccessibility != Accessibility.Protected)
                {
                    continue;
                }

                if (eachMethod.Parameters.Length != 0)
                {
                    continue;
                }

                // records always have an implicitly declared ToString method. In C# 10, the user can differentiate this
                // by making the method sealed.
                if (typeSymbol.IsRecord && eachMethod.IsImplicitlyDeclared)
                {
                    continue;
                }

                // In C# 10, the user can differentiate a ToString overload by making the method sealed.
                // We report back if it's sealed or not so that we can emit an error if it's not sealed.
                // The error stops another compilation error; if unsealed, the generator generates a duplicate ToString() method.
                return new ToStringInfo(
                    HasToString: true,
                    IsRecordClass: typeSymbol is { IsRecord: true, IsReferenceType: true },
                    IsSealed: eachMethod.IsSealed,
                    eachMethod);
            }

            INamedTypeSymbol? baseType = typeSymbol.BaseType;

            if (baseType is null)
            {
                return new ToStringInfo(false, false, false, null);
            }
            
            if (baseType.SpecialType == SpecialType.System_Object || baseType.SpecialType == SpecialType.System_ValueType)
            {
                return new ToStringInfo(false, false, false, null);
            }

            typeSymbol = baseType;
        }
    }

    private static bool IsUnderlyingAValueType(VogenConfiguration config)
    {
        bool isValueType = true;
        if (config.UnderlyingType != null)
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

    private static void ReportErrorIfNestedType(VoTarget target, SourceProductionContext context,
        INamedTypeSymbol voSymbolInformation)
    {
        INamedTypeSymbol? containingType = target.ContainingType;
        if (containingType != null)
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.TypeCannotBeNested(voSymbolInformation, containingType));
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
        INamedTypeSymbol? underlyingType)
    {
        IEnumerable<AttributeData> matchingAttributes =
            attributes.Where(a => a.AttributeClass?.FullName() is "Vogen.InstanceAttribute");

        var props = BuildInstanceProperties.Build(matchingAttributes, context, voClass, underlyingType);
        
        return props.Where(a => a is not null)!;
    }
}