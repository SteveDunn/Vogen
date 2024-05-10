using System.Collections.Immutable;
using System.Linq;
using System.Threading;
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
    /// <param name="ctx"></param>
    /// <returns></returns>
    public static VogenConfigurationBuildResult GetDefaultConfigFromGlobalAttribute(
        GeneratorAttributeSyntaxContext ctx)
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


    /// <summary>
    /// Tries to get the syntax element for any matching attribute that might exist in the provided context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>The syntax of the attribute if it matches the global defaults attribute, otherwise null.</returns>
    public static AttributeSyntax? TryGetAssemblyLevelDefaultsAttribute(GeneratorAttributeSyntaxContext context)
    {
        ImmutableArray<AttributeData> assemblyAttributes = context.TargetSymbol.GetAttributes();

        if (assemblyAttributes.IsDefaultOrEmpty)
        {
            return null;
        }
        
        foreach (AttributeData? attribute in assemblyAttributes)
        {
            var attrClass = attribute.AttributeClass;
            
            if (!(attrClass?.Name is "VogenDefaultsAttribute" or "VogenDefaults" &&
                  attrClass.ToDisplayString() == "Vogen.VogenDefaultsAttribute"))
            {
                continue;
            }
            
            SyntaxNode? syntax = attribute.ApplicationSyntaxReference?.GetSyntax();

            return syntax as AttributeSyntax;
        }

        return null;
    }

    /// <summary>
    /// Tries to get the syntax element for any matching attribute that might exist in the provided context.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The syntax of the attribute if it matches the global defaults attribute, otherwise null.</returns>
    public static AttributeSyntax? TryGetAssemblyLevelDefaultsAttribute2(GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var attributes = context.Attributes;
        if (attributes.IsDefaultOrEmpty)
        {
            return null;
        }
        
        AttributeData a = attributes.ElementAt(0);
        
        var n = a.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
        return n;
        // ImmutableArray<AttributeData> assemblyAttributes = context.TargetSymbol.GetAttributes();
        //
        // if (assemblyAttributes.IsDefaultOrEmpty)
        // {
        //     return null;
        // }
        //
        // foreach (AttributeData? attribute in assemblyAttributes)
        // {
        //     var attrClass = attribute.AttributeClass;
        //     
        //     if (!(attrClass?.Name is "VogenDefaultsAttribute" or "VogenDefaults" &&
        //           attrClass.ToDisplayString() == "Vogen.VogenDefaultsAttribute"))
        //     {
        //         continue;
        //     }
        //     
        //     SyntaxNode? syntax = attribute.ApplicationSyntaxReference?.GetSyntax();
        //
        //     return syntax as AttributeSyntax;
        // }

        //return null;
    }
}