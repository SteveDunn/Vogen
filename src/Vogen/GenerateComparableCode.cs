using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateComparableCode
{
    public static string GenerateIComparableHeaderIfNeeded(string precedingText, VoWorkItem item, TypeDeclarationSyntax tds)
    {
        if (item.ComparisonGeneration == ComparisonGeneration.Omit)
        {
            return string.Empty;
        }
        
        if (item.ComparisonGeneration == ComparisonGeneration.UseUnderlying && item.UnderlyingType.ImplementsInterfaceOrBaseClass(typeof(IComparable<>)))
        {
            return $"{precedingText} global::System.IComparable<{tds.Identifier}>, global::System.IComparable";
        }
    
        return string.Empty;
    }

    public static string GenerateIComparableImplementationIfNeeded(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        if (item.ComparisonGeneration == ComparisonGeneration.Omit)
        {
            return string.Empty;
        }

        INamedTypeSymbol? primitiveSymbol = item.UnderlyingType;
        if (!primitiveSymbol.ImplementsInterfaceOrBaseClass(typeof(IComparable<>)))
        {
            return string.Empty;
        }

        if (item.ComparisonGeneration != ComparisonGeneration.UseUnderlying)
        {
            return string.Empty;
        }
    
        var primitive = tds.Identifier;
        var s = $$"""
                  public int CompareTo({{primitive}} other) => Value.CompareTo(other.Value);
                          public int CompareTo(object other) {
                              if(other == null) return 1;
                              if(other is {{primitive}} x) return CompareTo(x);
                              throw new global::System.ArgumentException("Cannot compare to object as it is not of type {{primitive}}", nameof(other));
                          }
                  """;
    
        return s;
    }
}