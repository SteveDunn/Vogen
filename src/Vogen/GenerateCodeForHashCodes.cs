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

        return
            $$"""
              
                        public override global::System.Int32 GetHashCode()
                        {
                          unchecked // Overflow is fine, just wrap
                          {
                              global::System.Int32 hash = (global::System.Int32) 2166136261;
                              hash = (hash * 16777619) ^ GetType().GetHashCode();
                              hash = (hash * 16777619) ^ global::System.Collections.Generic.EqualityComparer<{{itemUnderlyingType}}>.Default.GetHashCode(Value);
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

        return
            $$"""
              
                        public readonly override global::System.Int32 GetHashCode()
                        {
                          return global::System.Collections.Generic.EqualityComparer<{{itemUnderlyingType}}>.Default.GetHashCode(Value);
                        }
              """;
    }
    
}