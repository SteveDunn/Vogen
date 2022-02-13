using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    /// <param name="reportDiagnostic"></param>
    /// <returns></returns>
    public static VogenConfiguration? GetDefaultConfigFromGlobalAttribute(
        ImmutableArray<AttributeSyntax> defaults,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic)
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

        INamedTypeSymbol? defaultsAttribute = compilation.GetTypeByMetadataName("Vogen.VogenDefaultsAttribute");
        if (defaultsAttribute is null)
        {
            // The attribute isn't part of the compilation for some reason...
            return null;
        }

        foreach (AttributeData attribute in assemblyAttributes)
        {
            if (!defaultsAttribute.Equals(attribute.AttributeClass, SymbolEqualityComparer.Default))
            {
                continue;
            }

            Type invalidExceptionType = typeof(ValueObjectValidationException);
            Conversions conversions = Conversions.Default;
            bool hasMisconfiguredInput = false;

            if (!attribute.ConstructorArguments.IsEmpty)
            {
                // make sure we don't have any errors
                ImmutableArray<TypedConstant> args = attribute.ConstructorArguments;

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
                    case 2:
                        conversions = (Conversions) (args[1].Value ?? Conversions.Default);
                        goto case 1;
                    case 1:
                        invalidExceptionType = (Type) (args[0].Value ?? typeof(ValueObjectValidationException));
                        break;
                }
            }

            if (!attribute.NamedArguments.IsEmpty)
            {
                foreach (KeyValuePair<string, TypedConstant> arg in attribute.NamedArguments)
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
                            case "invalidExceptionType":
                                invalidExceptionType = (Type) typedConstant.Value!;
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
                break;
            }

            SyntaxNode? syntax = null;
            if (!conversions.IsValidFlags())
            {
                syntax = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                {
                    reportDiagnostic(InvalidConversionDiagnostic.Create(syntax));
                }
            }

            return new VogenConfiguration(invalidExceptionType, conversions);
        }

        return null;
    }

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