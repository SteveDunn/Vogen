using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateEqualsAndHashCodes
{
    public static string GenerateEqualsForAClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var className = tds.Identifier;

        return $$"""
                  
                  public global::System.Boolean Equals({{className}} other)
                  {
                      if (ReferenceEquals(null, other))
                      {
                          return false;
                      }
                 
                      // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
                      // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
                      if(!_isInitialized || !other._isInitialized) return false;
                 
                      if (ReferenceEquals(this, other))
                      {
                          return true;
                      }
                 
                      return GetType() == other.GetType() && {{GenerateCallToDefaultComparerOrCustomStringComparer(item)}};
                  }
                 
                  {{GenerateEqualsForUnderlyingMethod(item)}}
                 
                  public override global::System.Boolean Equals(global::System.Object obj)
                  {
                      if (ReferenceEquals(null, obj))
                      {
                          return false;
                      }
                 
                      if (ReferenceEquals(this, obj))
                      {
                          return true;
                      }
                 
                      if (obj.GetType() != GetType())
                      {
                          return false;
                      }
                 
                      return Equals(({{className}}) obj);
                  }
                 """;
    }

    public static string GenerateEqualsForAStruct(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var structName = tds.Identifier;

        return $$"""

                 public readonly global::System.Boolean Equals({{structName}} other)
                 {
                     // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
                     // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
                     if(!_isInitialized || !other._isInitialized) return false;
                 
                     return {{GenerateCallToDefaultComparerOrCustomStringComparer(item)}};
                 }

                  {{GenerateEqualsForUnderlyingMethod(item)}}
                 
                 public readonly override global::System.Boolean Equals(global::System.Object obj)
                 {
                     return obj is {{structName}} && Equals(({{structName}}) obj);
                 }
                 """;
    }
    
    public static string GenerateGetHashCode(VoWorkItem item)
    {
        string itemUnderlyingType = item.UnderlyingTypeFullName;

        bool isString = typeof(string).IsAssignableFrom(itemUnderlyingType.GetType());

        var stringHashCode = isString && item.StringComparisonGeneration is not StringComparisonGeneration.Unspecified
            ? $"global::System.StringComparer.{GetComparerLiteral(item.StringComparisonGeneration)}.GetHashCode(Value)"
            : "Value.GetHashCode()";
        return $$"""
                         public override global::System.Int32 GetHashCode()
                         {
                             unchecked // Overflow is fine, just wrap
                             {
                                 global::System.Int32 hash = (global::System.Int32) 2166136261;
                                 hash = (hash * 16777619) ^ {{stringHashCode}};
                                 hash = (hash * 16777619) ^ GetType().GetHashCode();
                                 hash = (hash * 16777619) ^ global::System.Collections.Generic.EqualityComparer<{{itemUnderlyingType}}>.Default.GetHashCode();
                                 return hash;
                             }
                         }
                 """;
    }

    private static string GenerateEqualsForUnderlyingMethod(VoWorkItem item)
    {
        string itemUnderlyingType = item.UnderlyingTypeFullName;

        bool isString = typeof(string).IsAssignableFrom(itemUnderlyingType.GetType());

        return isString && item.StringComparisonGeneration is not StringComparisonGeneration.Unspecified
            ? GenerateEqualsForString(item, itemUnderlyingType)
            : $"public global::System.Boolean Equals({itemUnderlyingType} primitive) {{ return Value.Equals(primitive); }}";
    }

    private static string GenerateEqualsForString(VoWorkItem item, string itemUnderlyingType)
    {
        string comparerLiteral = GetComparerLiteral(item.StringComparisonGeneration);

        return
            $"public global::System.Boolean Equals({itemUnderlyingType} primitive) {{ return global::System.StringComparer.{comparerLiteral}.Equals(Value, primitive); }}";
    }

    private static string GetComparerLiteral(StringComparisonGeneration e) =>
        e switch
        {
            StringComparisonGeneration.CurrentCulture => "CurrentCulture",
            StringComparisonGeneration.CurrentCultureIgnoreCase => "CurrentCultureIgnoreCase",
            StringComparisonGeneration.InvariantCulture => "InvariantCulture",
            StringComparisonGeneration.InvariantCultureIgnoreCase => "InvariantCultureIgnoreCase",
            StringComparisonGeneration.Ordinal => "Ordinal",
            StringComparisonGeneration.OrdinalIgnoreCase => "OrdinalIgnoreCase",
            StringComparisonGeneration.Unspecified => throw new InvalidOperationException("Don't know how to get the StringComparer as it was unspecified"),
            _ => throw new InvalidOperationException("Don't know how to get the StringComparer for " + nameof(e))
        };

    private static string GenerateCallToDefaultComparerOrCustomStringComparer(VoWorkItem item)
    {
        string itemUnderlyingType = item.UnderlyingTypeFullName;

        bool isString = typeof(string).IsAssignableFrom(itemUnderlyingType.GetType());

        return isString && item.StringComparisonGeneration is not StringComparisonGeneration.Unspecified
            ? $"Equals(other.Value)"
            : $"global::System.Collections.Generic.EqualityComparer<{itemUnderlyingType}>.Default.Equals(Value, other.Value)";
    }
}
