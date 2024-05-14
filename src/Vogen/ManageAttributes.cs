using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

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
    /// <param name="ctx"></param>
    /// <returns></returns>
    public static VogenConfigurationBuildResult GetDefaultConfigFromGlobalAttribute(GeneratorAttributeSyntaxContext ctx)
    {
        var assemblyAttributes = ctx.Attributes;
        
        if (assemblyAttributes.IsDefaultOrEmpty)
        {
            return VogenConfigurationBuildResult.Null;
        }

        AttributeData attr = assemblyAttributes.ElementAt(0);

        return BuildConfigurationFromAttributes.TryBuildFromVogenDefaultsAttribute(attr);
    }

    /// <summary>
    /// Gets global default configuration from any global (assembly) attribute.
    /// This is used by the analyzer.
    /// If none are specified, then the default configuration is used.
    /// If some are specified, then they are validated.
    /// If anything is invalid, a compilation error is raised.
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static VogenConfigurationBuildResult GetDefaultConfigFromGlobalAttribute(Compilation compilation)
    {
        ImmutableArray<AttributeData> assemblyAttributes = compilation.Assembly.GetAttributes();
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

        if (matchingAttribute is null)
        {
            return VogenConfigurationBuildResult.Null;
        }

        VogenConfigurationBuildResult globalConfig = BuildConfigurationFromAttributes.TryBuildFromVogenDefaultsAttribute(matchingAttribute);

        return globalConfig;
    }
}