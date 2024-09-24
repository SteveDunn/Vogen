using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateComparableCode
{
    public static string GenerateIComparableHeaderIfNeeded(string precedingText, VoWorkItem item, TypeDeclarationSyntax tds)
    {
        if (item.Config.Comparison == ComparisonGeneration.Omit)
        {
            return string.Empty;
        }
        
        if (item.Config.Comparison == ComparisonGeneration.UseUnderlying && item.UnderlyingType.ImplementsInterfaceOrBaseClass(typeof(IComparable<>)))
        {
            return $"{precedingText} global::System.IComparable<{tds.Identifier}>, global::System.IComparable";
        }
    
        return string.Empty;
    }

    public static string GenerateIComparableImplementationIfNeeded(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        if (item.Config.Comparison == ComparisonGeneration.Omit)
        {
            return string.Empty;
        }

        INamedTypeSymbol primitiveSymbol = item.UnderlyingType;
        if (!primitiveSymbol.ImplementsInterfaceOrBaseClass(typeof(IComparable<>)))
        {
            return string.Empty;
        }

        if (item.Config.Comparison != ComparisonGeneration.UseUnderlying)
        {
            return string.Empty;
        }
    
        //string nullAnnotation = item.ShouldShowNullAnnotations ? "?" : string.Empty;
        string wrapperQ = item.Nullable.QuestionMarkForWrapper;

        var wrapper = tds.Identifier;

        string strongType = item.IsTheWrapperAReferenceType
            ? $$"""
                public int CompareTo({{wrapper}}{{wrapperQ}} other) 
                {
                    if(other is null) 
                      return 1;
                    
                    return Value.CompareTo(other.Value);
                }
                """
            : $"public int CompareTo({wrapper} other) => Value.CompareTo(other.Value);";

        
        return $$"""
                 {{strongType}}

                 public int CompareTo(object{{item.Nullable.QuestionMarkForOtherReferences}} other) 
                 {
                     if(other is null) 
                       return 1;
                     
                     if(other is {{wrapper}} x) 
                       return CompareTo(x);
                     
                     ThrowHelper.ThrowArgumentException("Cannot compare to object as it is not of type {{wrapper}}", nameof(other));
                     return 0;
                 }
                 """;
    }
}