using Analyzer.Utilities.Extensions;

namespace Vogen;

public static class GenerateCodeForHashCodes
{
    public static string GenerateGetHashCodeForAClass(VoWorkItem item)
    {
        if (item.UserProvidedOverloads.HashCodeInfo.WasProvided)
        {
            return string.Empty;
        }
        
        string itemUnderlyingType = item.UnderlyingTypeFullNameWithGlobalAlias;

        // When the underlying type is a nullable value type (e.g. ushort?), EqualityComparer<T>.Default.GetHashCode
        // has [DisallowNull] on its parameter in .NET 10+, causing CS8607. Use null-safe approach instead.
        // Wrap the expression in parentheses to ensure correct precedence with the ^ operator.
        string hashCodeExpression = item.UnderlyingType.IsNullableValueType()
            ? "(Value is null ? 0 : Value.GetHashCode())"
            : $"global::System.Collections.Generic.EqualityComparer<{itemUnderlyingType}>.Default.GetHashCode(Value)";

        return
            $$"""
              
                        public override global::System.Int32 GetHashCode()
                        {
                          unchecked // Overflow is fine, just wrap
                          {
                              global::System.Int32 hash = (global::System.Int32) 2166136261;
                              hash = (hash * 16777619) ^ GetType().GetHashCode();
                              hash = (hash * 16777619) ^ {{hashCodeExpression}};
                              return hash;
                          }
                        }
              """;
    }
    
    public static string GenerateForAStruct(VoWorkItem item)
    {
        if (item.UserProvidedOverloads.HashCodeInfo.WasProvided)
        {
            return string.Empty;
        }

        string itemUnderlyingType = item.UnderlyingTypeFullNameWithGlobalAlias;

        // When the underlying type is a nullable value type (e.g. ushort?), EqualityComparer<T>.Default.GetHashCode
        // has [DisallowNull] on its parameter in .NET 10+, causing CS8607. Use null-safe approach instead.
        string hashCodeExpression = item.UnderlyingType.IsNullableValueType()
            ? "Value is null ? 0 : Value.GetHashCode()"
            : $"global::System.Collections.Generic.EqualityComparer<{itemUnderlyingType}>.Default.GetHashCode(Value)";

        return
            $$"""
              
                        public readonly override global::System.Int32 GetHashCode()
                        {
                          return {{hashCodeExpression}};
                        }
              """;
    }
    
}