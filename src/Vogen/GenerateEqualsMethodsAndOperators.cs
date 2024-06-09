using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateEqualsMethodsAndOperators
{
    public static string GenerateEqualsMethodsForAClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var className = tds.Identifier;
        
        string virtualKeyword = item is { IsRecordClass: true, IsSealed: false } ? "virtual " : " ";

        var ret = item.UserProvidedOverloads.EqualsForWrapper.WasProvided ? string.Empty : 
$$"""

            public {{virtualKeyword}}global::System.Boolean Equals({{className}} other)
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

              return GetType() == other.GetType() && {{$"global::System.Collections.Generic.EqualityComparer<{item.UnderlyingTypeFullName}>.Default.Equals(Value, other.Value)"}};
            }
""";

        ret += 
$$"""

             public global::System.Boolean Equals({{className}} other, global::System.Collections.Generic.IEqualityComparer<{{className}}> comparer)
             {
                 return comparer.Equals(this, other);
             }

             {{GenerateEqualsMethodForPrimitive(item, isReadOnly: false)}}
""";

        if (!item.IsRecordClass)
        {
            ret += $$"""

                      public override global::System.Boolean Equals(global::System.Object obj)
                      {
                          return Equals(obj as {{className}});
                      }
                     """;
        }

        return ret;
    }

    public static string GenerateEqualsMethodsForAStruct(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var structName = tds.Identifier;

        var ret = item.UserProvidedOverloads.EqualsForWrapper.WasProvided ? string.Empty :
$$"""
            public readonly global::System.Boolean Equals({{structName}} other)
            {
              // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
              // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
              if(!_isInitialized || !other._isInitialized) return false;

              return {{$"global::System.Collections.Generic.EqualityComparer<{item.UnderlyingTypeFullName}>.Default.Equals(Value, other.Value)"}};
            }
  """;

        ret +=
$$"""

            public global::System.Boolean Equals({{structName}} other, global::System.Collections.Generic.IEqualityComparer<{{structName}}> comparer)
            {
              return comparer.Equals(this, other);
            }

            {{GenerateEqualsMethodForPrimitive(item, isReadOnly: true)}}
  """;

        if (!item.IsRecordStruct)
        {
            ret +=
$$"""

            public readonly override global::System.Boolean Equals(global::System.Object obj)
            {
              return obj is {{structName}} && Equals(({{structName}}) obj);
            }
  """;
        }

        return ret;
    }

    private static string GenerateEqualsMethodForPrimitive(VoWorkItem item, bool isReadOnly)
    {
        if (!item.Config.PrimitiveEqualityGeneration.HasFlag(PrimitiveEqualityGeneration.GenerateMethods))
        {
            return string.Empty;
        }

        string itemUnderlyingType = item.UnderlyingTypeFullName;

        bool isString = item.IsUnderlyingAString;

        string readonlyOrEmpty = isReadOnly ? " readonly" : string.Empty;

        string output = item.UserProvidedOverloads.EqualsForUnderlying.WasProvided ? string.Empty :
$$"""

            public{{readonlyOrEmpty}} global::System.Boolean Equals({{itemUnderlyingType}} primitive)
            {
              return Value.Equals(primitive);
            }

""";

        if(isString)
        {
            output += 
$$"""

            public{{readonlyOrEmpty}} global::System.Boolean Equals({{itemUnderlyingType}} primitive, global::System.StringComparer comparer) 
            {
                return comparer.Equals(Value, primitive);
            }
""";
        }

        return output;
    }

    public static string GenerateEqualsOperatorsForPrimitivesIfNeeded(string itemUnderlyingType, SyntaxToken typeName, VoWorkItem item)
    {
        if (item.Config.PrimitiveEqualityGeneration.HasFlag(PrimitiveEqualityGeneration.GenerateOperators))
            return $"""
                    
                            public static global::System.Boolean operator ==({typeName} left, {itemUnderlyingType} right) => Equals(left.Value, right);
                            public static global::System.Boolean operator !=({typeName} left, {itemUnderlyingType} right) => !Equals(left.Value, right);
                    
                            public static global::System.Boolean operator ==({itemUnderlyingType} left, {typeName} right) => Equals(left, right.Value);
                            public static global::System.Boolean operator !=({itemUnderlyingType} left, {typeName} right) => !Equals(left, right.Value);
                    """;

        return string.Empty;
    }

    public static string GenerateInterfaceIfNeeded(string prefix, string itemUnderlyingType, VoWorkItem item)
    {
        if (!item.Config.PrimitiveEqualityGeneration.HasFlag(PrimitiveEqualityGeneration.GenerateMethods))
        {
            return string.Empty;
        }
        
        return $"{prefix}global::System.IEquatable<{itemUnderlyingType}> ";
    }
}
