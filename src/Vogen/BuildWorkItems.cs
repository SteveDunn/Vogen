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
    public static VoWorkItem? TryBuild(
        VoTarget target,
        SourceProductionContext context,
        DiagnosticCollection diagnostics)
    {
        var tds = target.TypeToAugment;

        var voClass = target.SymbolForType;

        var attributes = voClass.GetAttributes();

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
                diagnostics.AddCannotHaveUserConstructors(eachConstructor);
            }
        }

        ImmutableArray<TypedConstant> args = voAttribute.ConstructorArguments;

        Conversions conversions = Conversions.None;

        foreach (TypedConstant arg in args)
        {
            if (arg.Kind == TypedConstantKind.Error)
            {
                break;
            }
        }

        if (args.Length == 0)
        {
            diagnostics.AddMustSpecifyUnderlyingType(voClass);
            return null;
        }

        var underlyingType = (INamedTypeSymbol?) args[0].Value;

        if (underlyingType is null)
        {
            diagnostics.AddMustSpecifyUnderlyingType(voClass);
            return null;
        }

        if (args.Length == 2 && args[1].Value is not null)
        {
            conversions = (Conversions) args[1].Value!;
        }

        var containingType = target.ContainingType;// context.SemanticModel.GetDeclaredSymbol(context.Node)!.ContainingType;
        if (containingType != null)
        {
            diagnostics.AddTypeCannotBeNested(voClass, containingType);
        }

        var instanceProperties = TryBuildInstanceProperties(attributes, voClass, diagnostics);

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
                        diagnostics.AddValidationMustBeStatic(mds);
                    }

                    TypeSyntax returnTypeSyntax = mds.ReturnType;

                    if (returnTypeSyntax.ToString() != "Validation")
                    {
                        diagnostics.AddValidationMustReturnValidationType(mds);
                    }

                    validateMethod = mds;
                }
            }
        }

        if (SymbolEqualityComparer.Default.Equals(voClass, underlyingType))
        {
            diagnostics.AddUnderlyingTypeMustNotBeSameAsValueObjectType(voClass);
        }

        if (underlyingType.ImplementsInterfaceOrBaseClass(typeof(ICollection)))
        {
            diagnostics.AddUnderlyingTypeCannotBeCollection(voClass, underlyingType);
        }

        bool isValueType = underlyingType.IsValueType;

        return new VoWorkItem
        {
            InstanceProperties = instanceProperties.ToList(),
            TypeToAugment = tds,
            IsValueType = isValueType,
            UnderlyingType = underlyingType,
            Conversions = conversions,
            ValidateMethod = validateMethod,
            FullNamespace = voClass.FullNamespace()
        };
    }

    private static IEnumerable<InstanceProperties> TryBuildInstanceProperties(
        ImmutableArray<AttributeData> attributes,
        INamedTypeSymbol voClass,
        DiagnosticCollection diagnostics)
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
                diagnostics.AddInstanceMethodCannotHaveNullArgumentName(voClass);
                //  continue;
            }

            var value = constructorArguments[1].Value;

            if (value is null)
            {
                diagnostics.AddInstanceMethodCannotHaveNullArgumentValue(voClass);
            }

            if (name is null || value is null)
            {
                continue;
            }

            yield return new InstanceProperties(name, value);
        }

    }
}