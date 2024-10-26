using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateCodeForStringComparers
{
    public static string GenerateIfNeeded(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        if (!item.IsUnderlyingAString)
        {
            return string.Empty;
        }

        if(item.Config.StringComparers != StringComparersGeneration.Generate)
        {
            return string.Empty;
        }

        return $$"""
                 #nullable disable
                 
                 public static class Comparers
                 {
                     private class __StringEqualityComparer : global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}>
                     {
                         readonly global::System.StringComparer _comparer;
                     
                         public __StringEqualityComparer(global::System.StringComparer comparer) 
                         {
                             _comparer = comparer;
                        }
                     
                         public bool Equals({{tds.Identifier}} x, {{tds.Identifier}} y) 
                         { 
                            return _comparer.Equals(x._value, y._value);
                        }
                     
                         public int GetHashCode({{tds.Identifier}} obj) 
                         {
                            return _comparer.GetHashCode();
                         }
                     }
                     
                     public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> Ordinal =>
                            new __StringEqualityComparer(global::System.StringComparer.Ordinal); 
                 
                     public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> OrdinalIgnoreCase =>
                            new __StringEqualityComparer(global::System.StringComparer.OrdinalIgnoreCase); 
                 
                     public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> CurrentCulture =>
                            new __StringEqualityComparer(global::System.StringComparer.CurrentCulture); 
                 
                     public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> CurrentCultureIgnoreCase =>
                            new __StringEqualityComparer(global::System.StringComparer.CurrentCultureIgnoreCase); 
                 
                     public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> InvariantCulture =>
                            new __StringEqualityComparer(global::System.StringComparer.InvariantCulture); 
                 
                     public static global::System.Collections.Generic.IEqualityComparer<{{tds.Identifier}}> InvariantCultureIgnoreCase =>
                            new __StringEqualityComparer(global::System.StringComparer.InvariantCultureIgnoreCase);
                            
                    #nullable restore
                 } 
                 """;
    }
}