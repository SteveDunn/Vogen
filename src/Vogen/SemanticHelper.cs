using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal static class SemanticHelper
{
    /// <summary>
    /// Returns the full name of a symbol, including the namespace.
    /// The returned value is escaped in case in case there are any
    /// keywords present. 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string EscapedFullName(this ISymbol symbol)
    {
        var prefix = FullNamespace(symbol);
        var suffix = "";

        if (symbol is INamedTypeSymbol nts)
        {
            if (nts.Arity > 0)
            {
                suffix = $"<{string.Join(", ", nts.TypeArguments.Select(a => GetEscapedFullNameForTypeArgument(a)))}>";
            }
        }
        
        if (prefix != string.Empty)
        {
            return $"{prefix}.{Util.EscapeKeywordsIfRequired(symbol.Name)}{suffix}";
        }

        return Util.EscapeKeywordsIfRequired(symbol.Name) + suffix;

        static string? GetEscapedFullNameForTypeArgument(ITypeSymbol a)
        {
            if (!a.CanBeReferencedByName)
            {
                return null;
            }
            if(a is not ITypeSymbol nts)
            {
                return null;
            }
            return EscapedFullName(nts);
        }
    }

    public static string FullNamespace(this ISymbol symbol)
    {
        var parts = new Stack<string>();
        INamespaceSymbol? iterator = symbol as INamespaceSymbol ?? symbol.ContainingNamespace;
        
        while (iterator is not null)
        {
            if (!string.IsNullOrEmpty(iterator.Name))
            {
                parts.Push(Util.EscapeKeywordsIfRequired(iterator.Name));
            }

            iterator = iterator.ContainingNamespace;
        }

        return string.Join(".", parts);
    }

    public static bool HasDefaultConstructor(this INamedTypeSymbol symbol)
    {
        return symbol.Constructors.Any(c => c.Parameters.Count() == 0);
    }

    public static IEnumerable<IPropertySymbol> ReadWriteScalarProperties(this INamedTypeSymbol symbol)
    {
        return symbol.GetMembers().OfType<IPropertySymbol>()
            .Where(p => p.CanRead() && p.CanWrite() && !p.HasParameters());
    }

    public static IEnumerable<IPropertySymbol> ReadableScalarProperties(this INamedTypeSymbol symbol)
    {
        return symbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.CanRead() && !p.HasParameters());
    }

    public static IEnumerable<IPropertySymbol> WritableScalarProperties(this INamedTypeSymbol symbol)
    {
        return symbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.CanWrite() && !p.HasParameters());
    }

    public static bool CanRead(this IPropertySymbol symbol) => symbol.GetMethod is not null;

    public static bool CanWrite(this IPropertySymbol symbol) => symbol.SetMethod is not null;

    public static bool HasParameters(this IPropertySymbol symbol) => symbol.Parameters.Any();

    public static bool ImplementsInterfaceOrBaseClass(this INamedTypeSymbol? typeSymbol, Type typeToCheck) =>
        ImplementsInterfaceOrBaseClass(typeSymbol, typeToCheck, true);
    public static bool ImplementsInterfaceOrBaseClassDirectly(this INamedTypeSymbol? typeSymbol, Type typeToCheck) =>
        ImplementsInterfaceOrBaseClass(typeSymbol, typeToCheck, false);

    private static bool ImplementsInterfaceOrBaseClass(this INamedTypeSymbol? typeSymbol, Type typeToCheck, bool recurse)
    {
        if (typeSymbol is null)
        {
            return false;
        }

        if (typeSymbol.MetadataName == typeToCheck.Name)
        {
            return true;
        }

        if (typeSymbol.BaseType?.MetadataName == typeToCheck.Name)
        {
            return true;
        }

        foreach (INamedTypeSymbol? @interface in typeSymbol.AllInterfaces)
        {
            if (@interface.MetadataName == typeToCheck.Name)
            {
                return true;
            }
        }

        if (recurse && typeSymbol.BaseType is not null)
        {
            return ImplementsInterfaceOrBaseClass(typeSymbol.BaseType, typeToCheck);
        }

        return false;
    }
}
