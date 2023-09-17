using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateEquatableCode
{
    public static string GenerateForAClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var className = tds.Identifier;

        var itemUnderlyingType = item.UnderlyingTypeFullName;

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
                 
                      return GetType() == other.GetType() && global::System.Collections.Generic.EqualityComparer<{{itemUnderlyingType}}>.Default.Equals(Value, other.Value);
                  }
                 
                  public global::System.Boolean Equals({{itemUnderlyingType}} primitive) => Value.Equals(primitive);
                 
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