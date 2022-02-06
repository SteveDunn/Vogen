using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal static class Templates
{
    private static readonly Assembly _thisAssembly = typeof(Templates).Assembly;

    private static readonly ImmutableHashSet<string> _existingResources =
        _thisAssembly.GetManifestResourceNames().ToImmutableHashSet();

    private static string? LoadEmbeddedResource(string resourceName)
    {
        if (!_existingResources.Contains(resourceName))
        {
            return null;
        }
        
        var resourceStream = _thisAssembly.GetManifestResourceStream(resourceName)!;

        using var reader = new StreamReader(resourceStream, Encoding.UTF8);

        return reader.ReadToEnd();
    }

    private static void ThrowNoResource(string resourceName) => 
        throw new MissingResourceException($"Could not find embedded resource {resourceName}. Available names: {string.Join(", ", _existingResources)}");

    public static string? TryGetForSpecificType(INamedTypeSymbol? underlyingType, string restOfTemplateName)
    {
        var underlyingTypeName = TypeResolver.ResolveTemplateNameFromTypeName(underlyingType);

        return LoadEmbeddedResource($"Vogen.Templates.{underlyingTypeName}.{underlyingTypeName}_{restOfTemplateName}.cs");
    }

    static class TypeResolver
    {
        /// <summary>
        /// Resolves a named type symbol to a template name.
        /// Named typed symbols for primitives have the 'SpecialType' set. This can be in the form of 'Int16' for Short,
        /// or 'Int32' for Int.
        /// If the Special Type isn't set, we just the 'Name' property of the symbol.
        /// </summary>
        /// <param name="nts"></param>
        /// <returns></returns>
        public static string? ResolveTemplateNameFromTypeName(INamedTypeSymbol? nts)
        {
            if (nts == null) return null;
            if (nts.SpecialType != SpecialType.None)
            {
                var s = nts.SpecialType.ToString();
                var underscore = s.IndexOf("_", StringComparison.OrdinalIgnoreCase);
                if (underscore >= s.Length - 1)
                {
                    return s;
                }

                var newString = s.Substring(underscore + 1);
                if (newString == "Int64") return "Long";
                if (newString == "Int32") return "Int";
                if (newString == "Int16") return "Short";

                return newString;
            }

            return nts.Name;
        }
    }

    public static string GetForAnyType(string restOfTemplateName)
    {
        string resourceName = $"Vogen.Templates.AnyOtherType.AnyOtherType_{restOfTemplateName}.cs";

        string? template = LoadEmbeddedResource(resourceName);

        return template ?? throw new MissingResourceException(
            $"Could not find embedded resource {resourceName}. Available names: {string.Join(", ", _existingResources)}");
    }
}