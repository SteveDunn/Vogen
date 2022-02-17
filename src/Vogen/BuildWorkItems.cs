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
        VogenConfiguration? globalConfig)
    {
        var tds = target.TypeToAugment;

        var voClass = target.SymbolForType;

        ImmutableArray<AttributeData> attributes = voClass.GetAttributes();

        if (attributes.Length == 0)
        {
            return null;
        }

        AttributeData? voAttribute = attributes.SingleOrDefault(
            a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");

        if (voAttribute is null)
        {
            return null;
        }

        foreach (var eachConstructor in voClass.Constructors)
        {
            // no need to check for default constructor as it's already defined
            // and the user will see: error CS0111: Type 'Foo' already defines a member called 'Foo' with the same parameter type
            if (eachConstructor.Parameters.Length > 0)
            {
                context.ReportDiagnostic(DiagnosticItems.CannotHaveUserConstructors(eachConstructor));
            }
        }

        ImmutableArray<TypedConstant> args = voAttribute.ConstructorArguments;

        // build the configuration but log any diagnostics (we have a separate analyzer that does that)
        var localConfig = GlobalConfigFilter.BuildConfigurationFromAttribute(voAttribute, context);
        
        if (localConfig == null)
        {
            return null;
        }

        var config = VogenConfiguration.Combine(localConfig.Value, globalConfig);

        foreach (TypedConstant arg in args)
        {
            if (arg.Kind == TypedConstantKind.Error)
            {
                break;
            }
        }

        var containingType = target.ContainingType;// context.SemanticModel.GetDeclaredSymbol(context.Node)!.ContainingType;
        if (containingType != null)
        {
            context.ReportDiagnostic(DiagnosticItems.TypeCannotBeNested(voClass, containingType));
        }

        var instanceProperties = TryBuildInstanceProperties(attributes, voClass, context);

        MethodDeclarationSyntax? validateMethod = null;

        // add any validator methods it finds
        foreach (var memberDeclarationSyntax in tds.Members)
        {
            if (memberDeclarationSyntax is MethodDeclarationSyntax mds)
            {
                object? value = mds.Identifier.Value;

                if (StringComparer.OrdinalIgnoreCase.Compare(value?.ToString(), "validate") == 0)
                {
                    if (!(mds.DescendantTokens().Any(t => t.IsKind(SyntaxKind.StaticKeyword))))
                    {
                        context.ReportDiagnostic(DiagnosticItems.ValidationMustBeStatic(mds));
                    }

                    TypeSyntax returnTypeSyntax = mds.ReturnType;

                    if (returnTypeSyntax.ToString() != "Validation")
                    {
                        context.ReportDiagnostic(DiagnosticItems.ValidationMustReturnValidationType(mds));
                    }

                    validateMethod = mds;
                }
            }
        }

        if (SymbolEqualityComparer.Default.Equals(voClass, config.UnderlyingType))
        {
            context.ReportDiagnostic(DiagnosticItems.UnderlyingTypeMustNotBeSameAsValueObjectType(voClass));
        }

        if (config.UnderlyingType.ImplementsInterfaceOrBaseClass(typeof(ICollection)))
        {
            context.ReportDiagnostic(DiagnosticItems.UnderlyingTypeCannotBeCollection(voClass, config.UnderlyingType!));
        }

        bool isValueType = true;
        if (config.UnderlyingType != null)
        {
            isValueType = config.UnderlyingType.IsValueType;
        }

        return new VoWorkItem
        {
            InstanceProperties = instanceProperties.ToList(),
            TypeToAugment = tds,
            IsValueType = isValueType,
            UnderlyingType = config.UnderlyingType,
            Conversions = config.Conversions, //?? throw new InvalidOperationException("Must have Conversions"),
            TypeForValidationExceptions = config.ValidationExceptionType,
            ValidateMethod = validateMethod,
            FullNamespace = voClass.FullNamespace()
        };
    }

    private static IEnumerable<InstanceProperties> TryBuildInstanceProperties(
        ImmutableArray<AttributeData> attributes,
        INamedTypeSymbol voClass,
        SourceProductionContext context)
    {
        var matchingAttributes =
            attributes.Where(a => a.AttributeClass?.ToString() is "Vogen.InstanceAttribute");

        foreach (AttributeData? eachAttribute in matchingAttributes)
        {
            if (eachAttribute == null)
            {
                continue;
            }

            ImmutableArray<TypedConstant> constructorArguments = eachAttribute.ConstructorArguments;

            if (constructorArguments.Length == 0)
            {
                continue;
            }

            var name = (string?) constructorArguments[0].Value;

            if (name is null)
            {
                context.ReportDiagnostic(DiagnosticItems.InstanceMethodCannotHaveNullArgumentName(voClass));
                //  continue;
            }

            var value = constructorArguments[1].Value;

            if (value is null)
            {
                context.ReportDiagnostic(DiagnosticItems.InstanceMethodCannotHaveNullArgumentValue(voClass));
            }

            if (name is null || value is null)
            {
                continue;
            }

            yield return new InstanceProperties(name, value);
        }

    }
}