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
            if (context.Node is TypeDeclarationSyntax typeDeclarationSyntax)
            {
                HandleType(context, typeDeclarationSyntax);

                return;
            }

            if (context.Node is LiteralExpressionSyntax literalExpressionSyntax)
            {
                HandleDefaultLiteralExpression(context, literalExpressionSyntax);
                return;
            }

            if (context.Node is DefaultExpressionSyntax defaultExpressionSyntax)
            {
                HandleDefaultExpression(context, defaultExpressionSyntax);
                return;
            }
        }
        catch (Exception ex) when (LogException(ex))
        {
        }
    }

    private void HandleDefaultLiteralExpression(GeneratorSyntaxContext context, LiteralExpressionSyntax literalExpressionSyntax)
    {
        if (literalExpressionSyntax.Kind() != SyntaxKind.DefaultLiteralExpression)
        {
            return;
        }

        var ancestor = literalExpressionSyntax.Ancestors(false)
            .FirstOrDefault(a => a.IsKind(SyntaxKind.VariableDeclaration));

        if (ancestor is not VariableDeclarationSyntax variableDeclarationSyntax)
        {
            return;
        }

        if (!IsOneOfOurValueObjects(context, variableDeclarationSyntax.Type, out string name))
        {
            return;
        }

        DiagnosticMessages.AddUsingDefaultProhibited(literalExpressionSyntax.GetLocation(), name);
    }

    private void HandleDefaultExpression(GeneratorSyntaxContext context, DefaultExpressionSyntax defaultExpressionSyntax)
    {
        TypeSyntax? typeSyntax = defaultExpressionSyntax?.Type;

        if (typeSyntax == null)
        {
            return;
        }

        if (!IsOneOfOurValueObjects(context, typeSyntax, out string name))
        {
            return;
        }

        DiagnosticMessages.AddUsingDefaultProhibited(typeSyntax.GetLocation(), name);
    }

    private bool IsOneOfOurValueObjects(GeneratorSyntaxContext context, TypeSyntax typeSyntax, out string name)
    {
        name = string.Empty;
        
        SymbolInfo typeSymbolInfo = context.SemanticModel.GetSymbolInfo(typeSyntax);

        var voClass = typeSymbolInfo.Symbol;

        if (voClass == null)
        {
            return false;
        }

        var attributes = voClass.GetAttributes();

        if (attributes.Length == 0)
        {
            return false;
        }

        AttributeData? voAttribute =
            attributes.SingleOrDefault(a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");

        if (voAttribute is null)
        {
            return false;
        }

        name = voClass.Name;

        return true;
    }

    private void HandleType(GeneratorSyntaxContext context, TypeDeclarationSyntax typeDeclarationSyntax)
    {
        var voClass = (INamedTypeSymbol) context.SemanticModel.GetDeclaredSymbol(context.Node)!;

        var attributes = voClass.GetAttributes();

        if (attributes.Length == 0)
        {
            return;
        }

        AttributeData? voAttribute = attributes.SingleOrDefault(
            a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");

        if (voAttribute is null)
        {
            return;
        }

        if (voAttribute.ConstructorArguments.Length == 0)
        {
            DiagnosticMessages.AddMustSpecifyUnderlyingType(voClass);
            return;
        }

        foreach (var eachConstructor in voClass.Constructors)
        {
            // no need to check for default constructor as it's already defined
            // and the user will see: error CS0111: Type 'Foo' already defines a member called 'Foo' with the same parameter type
            if (eachConstructor.Parameters.Length > 0)
            {
                DiagnosticMessages.AddCannotHaveUserConstructors(eachConstructor);
            }
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
                object? value = mds.Identifier.Value;

                Log.Add($"    Found method named {value}");

                if (value?.ToString() == "Validate")
                {
                    if (!(mds.DescendantTokens().Any(t => t.IsKind(SyntaxKind.StaticKeyword))))
                    {
                        DiagnosticMessages.AddValidationMustBeStatic(mds);
                    }

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