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
        List<string> log,
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

        if (voAttribute.ConstructorArguments.Length == 0)
        {
            diagnostics.AddMustSpecifyUnderlyingType(voClass);
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

        var underlyingType = (INamedTypeSymbol?) voAttribute.ConstructorArguments[0].Value;

        if (underlyingType is null)
        {
            diagnostics.AddMustSpecifyUnderlyingType(voClass);
            return null;
        }

        var containingType = target.ContainingType;// context.SemanticModel.GetDeclaredSymbol(context.Node)!.ContainingType;
        if (containingType != null)
        {
            diagnostics.AddTypeCannotBeNested(voClass, containingType);
        }

        var instanceProperties = TryBuildInstanceProperties(attributes, voClass, log, diagnostics);

        MethodDeclarationSyntax? validateMethod = null;

        // add any validator methods it finds
        foreach (var memberDeclarationSyntax in tds.Members)
        {
            if (memberDeclarationSyntax is MethodDeclarationSyntax mds)
            {
                object? value = mds.Identifier.Value;

                log.Add($"    Found method named {value}");

                if (value?.ToString() == "Validate")
                {
                    if (!(mds.DescendantTokens().Any(t => t.IsKind(SyntaxKind.StaticKeyword))))
                    {
                        diagnostics.AddValidationMustBeStatic(mds);
                    }

                    TypeSyntax returnTypeSyntax = mds.ReturnType;

                    if (returnTypeSyntax.ToString() != "Validation")
                    {
                        diagnostics.AddValidationMustReturnValidationType(mds);
                        log.Add($"    Validate return type is {returnTypeSyntax}");

                    }

                    log.Add($"    Added and will call {value}");

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
            ValidateMethod = validateMethod,
            FullNamespace = voClass.FullNamespace()
        };
    }

    private static IEnumerable<InstanceProperties> TryBuildInstanceProperties(
        ImmutableArray<AttributeData> attributes,
        INamedTypeSymbol voClass,
        List<string> log,
        DiagnosticCollection diagnostics)
    {
        var matchingAttributes =
            attributes.Where(a => a.AttributeClass?.ToString() is "Vogen.InstanceAttribute");

        foreach (AttributeData? eachAttribute in matchingAttributes)
        {
            if (eachAttribute == null)
            {
                log.Add($"no instance attribute");

                continue;
            }

            ImmutableArray<TypedConstant> constructorArguments = eachAttribute.ConstructorArguments;

            if (constructorArguments.Length == 0)
            {
                log.Add($"no constructor args!");
                continue;
            }

            var name = (string?) constructorArguments[0].Value;

            if (name is null)
            {
                log.Add($"name symbol for InstanceAttribute is null");
                diagnostics.AddInstanceMethodCannotHaveNullArgumentName(voClass);
                //  continue;
            }

            var value = constructorArguments[1].Value;

            if (value is null)
            {
                log.Add($"value symbol for InstanceAttribute is null");
                diagnostics.AddInstanceMethodCannotHaveNullArgumentValue(voClass);
            }

            if (name is null || value is null)
            {
                continue;
            }

            log.Add($"instance attribute found - Name: '{name}', Value: {value}");

            yield return new InstanceProperties(name, value);
        }

    }


}