using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateEqualsMethodsAndOperators
{
    public static string GenerateEqualsMethodsForAClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        string wrapperQ = item.Nullable.QuestionMarkForWrapper;

        var className = tds.Identifier;
        
        string virtualKeyword = item is { IsRecordClass: true, IsSealed: false } ? "virtual " : " ";

        var ret = item.UserProvidedOverloads.EqualsForWrapper.WasProvided ? string.Empty : 
$$"""

            public {{virtualKeyword}}global::System.Boolean Equals({{className}}{{wrapperQ}} other)
            {
              if (ReferenceEquals(null, other))
              {
                  return false;
              }

              // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
              // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
              if(!IsInitialized() || !other.IsInitialized()) return false;

              if (ReferenceEquals(this, other))
              {
                  return true;
              }

              return GetType() == other.GetType() && {{$"global::System.Collections.Generic.EqualityComparer<{item.UnderlyingTypeFullName}>.Default.Equals(Value, other.Value)"}};
            }
""";

        ret += 
$$"""

             public global::System.Boolean Equals({{className}}{{wrapperQ}} other, global::System.Collections.Generic.IEqualityComparer<{{className}}> comparer)
             {
                 return comparer.Equals(this, other);
             }

             {{GenerateEqualsMethodForPrimitive(item, isReadOnly: false)}}
""";

        if (!item.IsRecordClass)
        {
            ret += $$"""

                      public override global::System.Boolean Equals(global::System.Object{{item.Nullable.QuestionMarkForOtherReferences}} obj)
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
              if(!IsInitialized() || !other.IsInitialized()) return false;

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

            public readonly override global::System.Boolean Equals(global::System.Object{{item.Nullable.QuestionMarkForOtherReferences}} obj)
            {
              return obj is {{structName}} && Equals(({{structName}}) obj);
            }
  """;
        }

        return ret;
    }

    private static string GenerateEqualsMethodForPrimitive(VoWorkItem item, bool isReadOnly)
    {
        string underlyingQ = item.Nullable.QuestionMarkForUnderlying;

        if (!item.Config.PrimitiveEqualityGeneration.HasFlag(PrimitiveEqualityGeneration.GenerateMethods))
        {
            return string.Empty;
        }

        string itemUnderlyingType = item.UnderlyingTypeFullName;

        bool isString = item.IsUnderlyingAString;

        string readonlyOrEmpty = isReadOnly ? " readonly" : string.Empty;

        string output = item.UserProvidedOverloads.EqualsForUnderlying.WasProvided ? string.Empty :
$$"""

            public{{readonlyOrEmpty}} global::System.Boolean Equals({{itemUnderlyingType}}{{underlyingQ}} primitive)
            {
              return Value.Equals(primitive);
            }

""";

        if(isString)
        {
            output += 
$$"""

            public{{readonlyOrEmpty}} global::System.Boolean Equals({{itemUnderlyingType}}{{underlyingQ}} primitive, global::System.StringComparer comparer) 
            {
                return comparer.Equals(Value, primitive);
            }
""";
        }

        return output;
    }

    public static string GenerateEqualsOperatorsForPrimitivesIfNeeded(string itemUnderlyingType, SyntaxToken typeName, VoWorkItem item)
    {
        if (!item.Config.PrimitiveEqualityGeneration.HasFlag(PrimitiveEqualityGeneration.GenerateOperators))
        {
            return string.Empty;
        }

        string w = item.Nullable.QuestionMarkForWrapper;
        string u = item.Nullable.QuestionMarkForUnderlying;
        
        string wDefaultToFalse = item.Nullable.IsEnabled && item.IsTheWrapperAReferenceType ? "?? false" : string.Empty;

        return $"""
                
                    public static global::System.Boolean operator ==({typeName}{w} left, {itemUnderlyingType}{u} right) => left{w}.Value.Equals(right) {wDefaultToFalse};
                    public static global::System.Boolean operator ==({itemUnderlyingType}{u} left, {typeName}{w} right) => right{w}.Value.Equals(left) {wDefaultToFalse};

                    public static global::System.Boolean operator !=({itemUnderlyingType}{u} left, {typeName}{w} right) => !(left == right);
                    public static global::System.Boolean operator !=({typeName}{w} left, {itemUnderlyingType}{u} right) => !(left == right);
                """;
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
