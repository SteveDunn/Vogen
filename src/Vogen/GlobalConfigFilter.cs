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
    /// <param name="diags"></param>
    /// <returns></returns>
    public static VogenConfiguration? GetDefaultConfigFromGlobalAttribute(
        ImmutableArray<AttributeSyntax> defaults,
        Compilation compilation,
        DiagnosticCollection diags)
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

        return BuildConfigurationFromAttribute(matchingAttribute, diags);
    }
    
    public static VogenConfiguration? BuildConfigurationFromAttribute(AttributeData matchingAttribute, 
        DiagnosticCollection diagnostics)
    {
        INamedTypeSymbol? invalidExceptionType = null;
        INamedTypeSymbol? underlyingType = null;
        Conversions conversions = Conversions.Default;
        
        bool hasMisconfiguredInput = false;

        if (!matchingAttribute.ConstructorArguments.IsEmpty)
        {
            // make sure we don't have any errors
            ImmutableArray<TypedConstant> args = matchingAttribute.ConstructorArguments;

            foreach (TypedConstant arg in args)
            {
                if (arg.Kind == TypedConstantKind.Error)
                {
                    // have an error, so don't try and do any generation
                    hasMisconfiguredInput = true;
                }
            }

            switch (args.Length)
            {
                case 3:
                    invalidExceptionType = (INamedTypeSymbol?)args[2].Value;
                    if(invalidExceptionType != null && !invalidExceptionType.ImplementsInterfaceOrBaseClass(typeof(System.Exception)))
                    {
                        diagnostics.AddCustomExceptionMustDeriveFromException(invalidExceptionType);
                    }
                    goto case 2;
                case 2:
                    if (args[1].Value != null)
                    {
                        conversions = (Conversions) args[1].Value!;
                    }

                    goto case 1;
                case 1:
                    underlyingType = (INamedTypeSymbol?)args[0].Value;
                    break;
            }
        }
        
        if (!matchingAttribute.NamedArguments.IsEmpty)
        {
            foreach (KeyValuePair<string, TypedConstant> arg in matchingAttribute.NamedArguments)
            {
                TypedConstant typedConstant = arg.Value;
                if (typedConstant.Kind == TypedConstantKind.Error)
                {
                    hasMisconfiguredInput = true;
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
                    }
                }
            }
        }

        if (hasMisconfiguredInput)
        {
            // skip further generator execution and let compiler generate the errors
            return null;
        }

        SyntaxNode? syntax = null;
        //if (conversions.HasValue && !conversions.Value.IsValidFlags())
        if (!conversions.IsValidFlags())
        {
            syntax = matchingAttribute.ApplicationSyntaxReference?.GetSyntax();
            if (syntax is not null)
            {
                diagnostics.AddInvalidConversions(syntax.GetLocation());
                //diagnostics(InvalidConversionDiagnostic.Create(syntax));
            }
        }

        return new VogenConfiguration(underlyingType, invalidExceptionType, conversions);
    }

    private static string? GetQualifiedTypeName(ISymbol? symbol)
    {
        if (symbol == null)
        {
            return null;
        }
        
        return symbol.ContainingNamespace
               + "." + symbol.Name
               + ", " + symbol.ContainingAssembly;
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