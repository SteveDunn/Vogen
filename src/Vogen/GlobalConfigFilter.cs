using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;

namespace Vogen;

internal static class GlobalConfigFilter
{
    /// <summary>
    /// This is stage 1 in the pipeline - the 'quick filter'.  We find out is it a type declaration and does it have any attributes? - don't allocate anything
    /// here as this is called a **lot** (every time the editor is changed, i.e. key-presses).
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static bool IsTarget(SyntaxNode node) =>
        node is AttributeListSyntax attributeList
        && attributeList.Target is not null
        && attributeList.Target.Identifier.IsKind(SyntaxKind.AssemblyKeyword);

    /// <summary>
    /// Gets global default configuration from any global (assembly) attribute.
    /// If none are specified, then the default configuration is used.
    /// If some are specified, then they are validated.
    /// If anything is invalid, a compilation error is raised.
    /// </summary>
    /// <param name="defaults"></param>
    /// <param name="compilation"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static VogenConfiguration? GetDefaultConfigFromGlobalAttribute(
        ImmutableArray<AttributeSyntax> defaults,
        Compilation compilation,
        SourceProductionContext context)
    {
        if (defaults.IsDefaultOrEmpty)
        {
            // No global defaults
            return null;
        }

        var assemblyAttributes = compilation.Assembly.GetAttributes();
        if (assemblyAttributes.IsDefaultOrEmpty)
        {
            return null;
        }

        INamedTypeSymbol? allThatMatchByName = compilation.GetTypeByMetadataName("Vogen.VogenDefaultsAttribute");
        if (allThatMatchByName is null)
        {
            return null;
        }

        AttributeData? matchingAttribute = assemblyAttributes.SingleOrDefault(aa =>
            allThatMatchByName.Equals(aa.AttributeClass, SymbolEqualityComparer.Default));

        if (matchingAttribute == null)
        {
            return null;
        }

        return BuildConfigurationFromAttribute(matchingAttribute, context);
    }
    
    public static VogenConfiguration? BuildConfigurationFromAttribute(
        AttributeData matchingAttribute, 
        SourceProductionContext context)
    {
        INamedTypeSymbol? invalidExceptionType = null;
        INamedTypeSymbol? underlyingType = null;
        Conversions conversions = Conversions.Default;
        Customizations customizations= Customizations.None;
        DeserializationStrictness deserializationStrictness = DeserializationStrictness.Default;
        
        bool hasErroredAttributes = false;

        if (!matchingAttribute.ConstructorArguments.IsEmpty)
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
            deserializationStrictness = matchingAttribute.AttributeClass!.IsGenericType
                ? PopulateFromGenericAttribute(matchingAttribute, args)
                : PopulateFromNonGenericAttribute(args);
        }
        
        if (!matchingAttribute.NamedArguments.IsEmpty)
        {
            foreach (KeyValuePair<string, TypedConstant> arg in matchingAttribute.NamedArguments)
            {
                TypedConstant typedConstant = arg.Value;
                if (typedConstant.Kind == TypedConstantKind.Error)
                {
                    hasErroredAttributes = true;
                }
                else
                {
                    switch (arg.Key)
                    {
                        case "underlyingType":
                            underlyingType = (INamedTypeSymbol?) typedConstant.Value!;
                            break;
                        case "invalidExceptionType":
                            invalidExceptionType = (INamedTypeSymbol?) typedConstant.Value!;
                            break;
                        case "conversions":
                            conversions = (Conversions) (typedConstant.Value ?? Conversions.Default);
                            break;
                        case "customizations":
                            customizations = (Customizations) (typedConstant.Value ?? Customizations.None);
                            break;
                        case "deserializationStrictness":
                            deserializationStrictness = (DeserializationStrictness) (typedConstant.Value ?? Customizations.None);
                            break;
                    }
                }
            }
        }

        if (hasErroredAttributes)
        {
            // skip further generator execution and let compiler generate the errors
            return null;
        }

        if (!conversions.IsValidFlags())
        {
            var syntax = matchingAttribute.ApplicationSyntaxReference?.GetSyntax();
            if (syntax is not null)
            {
                context.ReportDiagnostic(DiagnosticsCatalogue.InvalidConversions(syntax.GetLocation()));
            }
        }

        if (!customizations.IsValidFlags())
        {
            var syntax = matchingAttribute.ApplicationSyntaxReference?.GetSyntax();
            if (syntax is not null)
            {
                context.ReportDiagnostic(DiagnosticsCatalogue.InvalidCustomizations(syntax.GetLocation()));
            }
        }

        if (!deserializationStrictness.IsValidFlags())
        {
            var syntax = matchingAttribute.ApplicationSyntaxReference?.GetSyntax();
            if (syntax is not null)
            {
                context.ReportDiagnostic(DiagnosticsCatalogue.InvalidDeserializationStrictness(syntax.GetLocation()));
            }
        }

        return new VogenConfiguration(underlyingType, invalidExceptionType, conversions, customizations, deserializationStrictness);

        DeserializationStrictness PopulateFromGenericAttribute(
            AttributeData attributeData,
            ImmutableArray<TypedConstant> args)
        {
            var type = attributeData.AttributeClass!.TypeArguments[0] as INamedTypeSymbol;
            switch (args.Length)
            {
                case 4:
                    if (args[3].Value != null)
                    {
                        deserializationStrictness = (DeserializationStrictness) args[3].Value!;
                    }

                    goto case 3;
                case 3:
                    if (args[2].Value != null)
                    {
                        customizations = (Customizations) args[2].Value!;
                    }

                    goto case 2;
                case 2:
                    invalidExceptionType = (INamedTypeSymbol?) args[1].Value;

                    ReportAnyIssuesWithTheException(invalidExceptionType, context);
                    goto case 1;

                case 1:
                    if (args[0].Value != null)
                    {
                        conversions = (Conversions) args[0].Value!;
                    }

                    break;
            }

            underlyingType = type;

            return deserializationStrictness;
        }

        DeserializationStrictness PopulateFromNonGenericAttribute(ImmutableArray<TypedConstant> args)
        {
            switch (args.Length)
            {
                case 5:
                    if (args[4].Value != null)
                    {
                        deserializationStrictness = (DeserializationStrictness) args[4].Value!;
                    }

                    goto case 4;
                case 4:
                    if (args[3].Value != null)
                    {
                        customizations = (Customizations) args[3].Value!;
                    }

                    goto case 3;
                case 3:
                    invalidExceptionType = (INamedTypeSymbol?) args[2].Value;

                    ReportAnyIssuesWithTheException(invalidExceptionType, context);
                    goto case 2;

                case 2:
                    if (args[1].Value != null)
                    {
                        conversions = (Conversions) args[1].Value!;
                    }

                    goto case 1;
                case 1:
                    underlyingType = (INamedTypeSymbol?) args[0].Value;
                    break;
            }

            return deserializationStrictness;
        }
    }

    private static void ReportAnyIssuesWithTheException(INamedTypeSymbol? invalidExceptionType, SourceProductionContext context)
    {
        if (invalidExceptionType == null)
        {
            return;
        }
        
        if (!invalidExceptionType.ImplementsInterfaceOrBaseClass(typeof(Exception)))
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.CustomExceptionMustDeriveFromException(invalidExceptionType));
        }

        var allConstructors = invalidExceptionType.Constructors.Where(c=>c.DeclaredAccessibility == Accessibility.Public);
        
        var singleParameterConstructors = allConstructors.Where(c => c.Parameters.Length == 1);
        
        if (singleParameterConstructors.Any(c => c.Parameters.Single().Type.Name == "String"))
        {
            return;
        }

        context.ReportDiagnostic(DiagnosticsCatalogue.CustomExceptionMustHaveValidConstructor(invalidExceptionType));
    }

    /// <summary>
    /// Tries to get the syntax element for any matching attribute that might exist in the provided context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static AttributeSyntax? GetAssemblyLevelAttributeForConfiguration(GeneratorSyntaxContext context)
    {
        // we know the node is a AttributeListSyntax thanks to IsSyntaxTargetForGeneration
        var attributeListSyntax = (AttributeListSyntax) context.Node;

        foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
        {
            if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
            {
                continue;
            }

            INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
            string fullName = attributeContainingTypeSymbol.ToDisplayString();

            if (fullName == "Vogen.VogenDefaultsAttribute")
            {
                return attributeSyntax;
            }
        }

        return null;
    }
}