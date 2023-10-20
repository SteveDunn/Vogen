using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace Vogen;

internal static class ManageAttributes
{
    /// <summary>
    /// Gets global default configuration from any global (assembly) attribute.
    /// If none are specified, then the default configuration is used.
    /// If some are specified, then they are validated.
    /// If anything is invalid, a compilation error is raised.
    /// </summary>
    /// <param name="defaults"></param>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static VogenConfigurationBuildResult GetDefaultConfigFromGlobalAttribute(
        ImmutableArray<AttributeSyntax> defaults,
        Compilation compilation)
    {
        if (defaults.IsDefaultOrEmpty)
        {
            // No global defaults
            return VogenConfigurationBuildResult.Null;
        }

        return GetDefaultConfigFromGlobalAttribute(compilation);
    }

    /// <summary>
    /// Gets global default configuration from any global (assembly) attribute.
    /// If none are specified, then the default configuration is used.
    /// If some are specified, then they are validated.
    /// If anything is invalid, a compilation error is raised.
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static VogenConfigurationBuildResult GetDefaultConfigFromGlobalAttribute(
        Compilation compilation)
    {
        var assemblyAttributes = compilation.Assembly.GetAttributes();
        if (assemblyAttributes.IsDefaultOrEmpty)
        {
            return VogenConfigurationBuildResult.Null;
        }

        INamedTypeSymbol? allThatMatchByName = compilation.GetTypeByMetadataName("Vogen.VogenDefaultsAttribute");
        if (allThatMatchByName is null)
        {
            return VogenConfigurationBuildResult.Null;
        }

        AttributeData? matchingAttribute = assemblyAttributes.SingleOrDefault(aa =>
            allThatMatchByName.Equals(aa.AttributeClass, SymbolEqualityComparer.Default));

        if (matchingAttribute == null)
        {
            return VogenConfigurationBuildResult.Null;
        }

        VogenConfigurationBuildResult globalConfig = BuildConfigurationFromAttributes.TryBuildFromVogenDefaultsAttribute(matchingAttribute);

        return globalConfig;
    }


    /// <summary>
    /// Tries to get the syntax element for any matching attribute that might exist in the provided context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>The syntax of the attribute if it matches the global defaults attribute, otherwise null.</returns>
    public static AttributeSyntax? TryGetAssemblyLevelDefaultsAttribute(GeneratorSyntaxContext context)
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