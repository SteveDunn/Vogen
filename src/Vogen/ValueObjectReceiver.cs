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

class ValueObjectReceiver : ISyntaxContextReceiver
{
    public List<string> Log { get; } = new();

    public List<ValueObjectWorkItem> WorkItems { get; } = new();

    public DiagnosticCollection DiagnosticMessages { get; } = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        try
        {
            if (context.Node is not TypeDeclarationSyntax typeDeclarationSyntax)
            {
                return;
            }

            var voClass = (INamedTypeSymbol) context.SemanticModel.GetDeclaredSymbol(context.Node)!;

            var attributes = voClass.GetAttributes();

            if (attributes.Length == 0)
            {
                Log.Add($"no attributes on {voClass}");

                return;
            }

            AttributeData? voAttribute =
                attributes.SingleOrDefault(a =>
                {
                    var fullName = a.AttributeClass?.FullName();
                    Log.Add($"== attribute fullname is {fullName}");
                    return fullName is "Vogen.ValueObjectAttribute";
                });

            if (voAttribute is null)
            {
                return;
            }
            
            if (voAttribute.ConstructorArguments.Length == 0)
            {
                DiagnosticMessages.AddMustSpecifyUnderlyingType(voClass);
                return;
            }
            
            var underlyingType = (INamedTypeSymbol?) voAttribute.ConstructorArguments[0].Value;
            
            if (underlyingType is null)
            {
                DiagnosticMessages.AddMustSpecifyUnderlyingType(voClass);
                return;
            }

            var containingType = context.SemanticModel.GetDeclaredSymbol(context.Node)!.ContainingType;
            if (containingType != null)
            {
                DiagnosticMessages.AddTypeCannotBeNested(voClass, containingType);
            }

            var instanceProperties = TryBuildInstanceProperties(attributes, voClass);

            MethodDeclarationSyntax? validateMethod = null;

            // add any validator methods it finds
            foreach (var memberDeclarationSyntax in typeDeclarationSyntax.Members)
            {
                if (memberDeclarationSyntax is MethodDeclarationSyntax mds)
                {
                    if (!(mds.DescendantTokens().Any(t => t.IsKind(SyntaxKind.StaticKeyword))))
                    {
                        DiagnosticMessages.AddValidationMustBeStatic(mds);
                    }

                    object? value = mds.Identifier.Value;

                    Log.Add($"    Found method named {value}");

                    if (value?.ToString() == "Validate")
                    {
                        TypeSyntax returnTypeSyntax = mds.ReturnType;
                        if (returnTypeSyntax.ToString() != "Validation")
                        {
                            DiagnosticMessages.AddValidationMustReturnValidationType(mds);
                            Log.Add($"    Validate return type is {returnTypeSyntax}");

                        }

                        Log.Add($"    Added and will call {value}");

                        validateMethod = mds;
                    }
                }
            }

            if (SymbolEqualityComparer.Default.Equals(voClass, underlyingType))
            {
                DiagnosticMessages.AddUnderlyingTypeMustNotBeSameAsValueObjectType(voClass);
            }

            if (underlyingType.ImplementsInterfaceOrBaseClass(typeof(ICollection)))
            {
                DiagnosticMessages.AddUnderlyingTypeCannotBeCollection(voClass, underlyingType);
            }
            
            bool isValueType = underlyingType.IsValueType;

            WorkItems.Add(new ValueObjectWorkItem
            {
                InstanceProperties = instanceProperties.ToList(),
                TypeToAugment = typeDeclarationSyntax,
                IsValueType = isValueType,
                UnderlyingType = underlyingType,
                ValidateMethod = validateMethod,
                FullNamespace = voClass.FullNamespace()
            });
        }
        catch (Exception ex) when (LogException(ex))
        {
        }
    }

    private bool LogException(Exception ex)
    {
        Log.Add("Error parsing syntax: " + ex);
        return false;
    }

    private IEnumerable<InstanceProperties> TryBuildInstanceProperties(
        ImmutableArray<AttributeData> attributes,
        INamedTypeSymbol voClass)
    {
        var matchingAttributes =
            attributes.Where(a => a.AttributeClass?.ToString() is "Vogen.InstanceAttribute");

        foreach (AttributeData? eachAttribute in matchingAttributes)
        {
            if (eachAttribute == null)
            {
                Log.Add($"no instance attribute");

                continue;
            }

            ImmutableArray<TypedConstant> constructorArguments = eachAttribute.ConstructorArguments;

            if (constructorArguments.Length == 0)
            {
                Log.Add($"no constructor args!");
                continue;
            }

            var name = (string?) constructorArguments[0].Value;

            if (name is null)
            {
                Log.Add($"name symbol for InstanceAttribute is null");
                DiagnosticMessages.AddInstanceMethodCannotHaveNullArgumentName(voClass);
              //  continue;
            }

            var value = constructorArguments[1].Value;

            if (value is null)
            {
                Log.Add($"value symbol for InstanceAttribute is null");
                DiagnosticMessages.AddInstanceMethodCannotHaveNullArgumentValue(voClass!);
            }

            if (name is null || value is null)
            {
                continue;
            }

            Log.Add($"instance attribute found - Name: '{name}', Value: {value}");

            yield return new InstanceProperties(name, value);
        }

    }
}