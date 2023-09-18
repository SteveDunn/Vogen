using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateEquatableCode
{
    public static string GenerateForAClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var className = tds.Identifier;
        
        string itemUnderlyingType = item.UnderlyingTypeFullName;

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
                 
                  {{GenerateInstanceEqualsMethod(item)}}
                 
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

    private static string GenerateInstanceEqualsMethod(VoWorkItem item)
    {
        string itemUnderlyingType = item.UnderlyingTypeFullName;

        bool isString = typeof(string).IsAssignableFrom(itemUnderlyingType.GetType());

        return isString && item.StringComparisonGeneration is not StringComparisonGeneration.Unspecified
            ? $"public global::System.Boolean Equals({itemUnderlyingType} primitive) => Value.Equals(primitive, global::System.StringComparison.{item.StringComparisonGeneration});"
            : $"public global::System.Boolean Equals({itemUnderlyingType} primitive) => Value.Equals(primitive);";
    }

    private static string GenerateCallToDefaultComparerOrCustomStringComparer(VoWorkItem item)
    {
        string itemUnderlyingType = item.UnderlyingTypeFullName;

        bool isString = typeof(string).IsAssignableFrom(itemUnderlyingType.GetType());

        return isString && item.StringComparisonGeneration is not StringComparisonGeneration.Unspecified
            ? $"Equals(other.Value)"
            : $"global::System.Collections.Generic.EqualityComparer<{itemUnderlyingType}>.Default.Equals(Value, other.Value)";
    }

    public static string GenerateForAStruct(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var structName = tds.Identifier;

        var itemUnderlyingType = item.UnderlyingTypeFullName;

        return $$"""

                 public readonly global::System.Boolean Equals({{structName}} other)
                 {
                     // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
                     // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
                     if(!_isInitialized || !other._isInitialized) return false;
         
                     return global::System.Collections.Generic.EqualityComparer<{{itemUnderlyingType}}>.Default.Equals(Value, other.Value);
                 }
         
                 public readonly global::System.Boolean Equals({{itemUnderlyingType}} primitive) => Value.Equals(primitive);
         
                 public readonly override global::System.Boolean Equals(global::System.Object obj)
                 {
                     return obj is {{structName}} && Equals(({{structName}}) obj);
                 }
                 """;
    }
}