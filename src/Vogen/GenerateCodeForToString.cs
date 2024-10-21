namespace Vogen;

public static class GenerateCodeForToString
{
    public static string GenerateNonReadOnly(VoWorkItem item) => GenerateToString(item, false);
    
    public static string GenerateReadOnly(VoWorkItem item) => GenerateToString(item, true);

    private static string GenerateToString(VoWorkItem item, bool isReadOnly)
    {
        if (item.UserProvidedOverloads.ToStringInfo.WasSupplied)
        {
            return string.Empty;
        }
        
        string ro = isReadOnly ? " readonly" : string.Empty;
        
        // we don't consider nullability here as *our* ToString never returns null, and we are free to 'narrow'
        // the signature (going from nullable to non-nullable): https://github.com/dotnet/coreclr/pull/23466#discussion_r269822099
        
        return $"""
                /// <summary>Returns the string representation of the underlying <see cref="{Util.EscapeTypeNameForTripleSlashComment(item.UnderlyingType)}" />.</summary>
                public{ro} override global::System.String ToString() => IsInitialized() ? Value.ToString() ?? "" : "[UNINITIALIZED]";
                """;
    }
}