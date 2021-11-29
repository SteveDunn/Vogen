using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generator.Diagnostics;

class ValueObjectReceiver : ISyntaxContextReceiver
{
    public List<string> Log { get; } = new();

    public List<ValueObjectWorkItem> WorkItems { get; } = new();

    public DiagnosticCollection DiagnosticMessages { get; } = new();
    
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        try
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                // Debugger.Launch();
            }
#endif
            Diagnose(context);

            if (context.Node is not TypeDeclarationSyntax typeDeclarationSyntax)
            {
                return;
            }

            var voClass = (INamedTypeSymbol) context.SemanticModel.GetDeclaredSymbol(context.Node)!;


            string fullNamespace = voClass.FullNamespace();
            Log.Add("++ full namespace is " + fullNamespace);
            Log.Add("++ full name is " + voClass.FullName());

            var attributes = voClass.GetAttributes();

            if (attributes.Length == 0)
            {
                Log.Add($"no attributes on {voClass}");

                return;
            }

            AttributeData? voAttribute =
                attributes.SingleOrDefault(a =>
                    a.AttributeClass?.ToString() == "Vogen.SharedTypes.ValueObjectAttribute");

            if (voAttribute is null)
            {
                Log.Add(
                    $"attribute class not Vogen.SharedTypes.ValueObjectAttribute - is '{attributes.SingleOrDefault()?.AttributeClass}'");

                return;
            }

            var containingType = context.SemanticModel.GetDeclaredSymbol(context.Node)!.ContainingType;
            if (containingType != null)
            {
                DiagnosticMessages.AddTypeCannotBeNested(voClass, containingType);
            }


            var instanceProperties = TryBuildInstanceProperties(attributes);

            Log.Add($"   Augmenting class: {typeDeclarationSyntax.Identifier}");

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

            var underlyingType = (INamedTypeSymbol?) voAttribute.ConstructorArguments[0].Value;

            if (underlyingType is null)
            {
                DiagnosticMessages.AddMustSpecifyUnderlyingType(voClass);
                return;
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
                FullNamespace = fullNamespace
            });


            // ClassToAugment = classDeclarationSyntax;
            // Log.Add($"   Attribute: {att.AttributeClass!.Name} Full Name: {att.AttributeClass.FullNamespace()}");
            //
            // UnderlyingType = (INamedTypeSymbol?)att.ConstructorArguments[0].Value;
        }
        catch (Exception ex)
        {
            Log.Add("Error parsing syntax: " + ex);
            //throw;
        }
    }

    private IEnumerable<InstanceProperties> TryBuildInstanceProperties(ImmutableArray<AttributeData> attributes)
    {
        var atts =
            attributes.Where(a => a.AttributeClass?.ToString() == "Vogen.SharedTypes.InstanceAttribute");

        foreach (var att in atts)
        {
            // var att =
            //     attributes.SingleOrDefault(a => a.AttributeClass?.ToString() == "Vogen.SharedTypes.InstanceAttribute");

            if (att == null)
            {
                Log.Add($"no instance attribute");

                continue;
            }

            var name = (string?) att.ConstructorArguments[0].Value;

            if (name is null)
            {
                Log.Add($"name symbol for InstanceAttribute is null");
                continue;
            }

            var value = att.ConstructorArguments[1].Value;

            if (value is null)
            {
                Log.Add($"value symbol for InstanceAttribute is null");
                continue;
            }

            Log.Add($"instance attribute found - Name: '{name}', Value: {value}");

            yield return new InstanceProperties(name, value);
        }

    }

    private void Diagnose(GeneratorSyntaxContext context)
    {
        if (context.Node is ClassDeclarationSyntax)
        {
            var testClass = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!;
            Log.Add($"Found a class named {testClass.Name}");
            var attributes = testClass.GetAttributes();
            Log.Add($"    Found {attributes.Length} attributes");
            foreach (AttributeData att in attributes)
            {
                Log.Add($"   Class to augment: {testClass!.Name} Full Name: {testClass.FullNamespace()}");
                Log.Add($"   Attribute: {att.AttributeClass!.Name} Full Name: {att.AttributeClass.FullNamespace()}");
                foreach (var arg in att.ConstructorArguments)
                {
                    Log.Add(
                        $"    ....Argument: Type='{arg.Type}' Value_Type='{arg.Value?.GetType().FullName}' Value='{arg.Value}'");

                    if (arg.Value is INamedTypeSymbol namedArgType)
                    {
                        Log.Add($"    ........Found a INamedTypeSymbol named '{namedArgType}'");
                        var members = namedArgType.GetMembers();
                        foreach (var member in members)
                        {
                            if (member is IPropertySymbol property)
                                Log.Add(
                                    $"    ...........Property: {property.Name} CanRead:{property.GetMethod != null} CanWrite:{property.SetMethod != null}");
                        }
                    }
                }
            }
        }
    }
}