using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

internal class GenerateEfCoreTypeConversions : IGenerateConversion
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        return string.Empty;
    }

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!IsOurs(item.Config.Conversions))
        {
            return string.Empty;
        }

        string code =
            Templates.TryGetForSpecificType(item.UnderlyingType, "EfCoreValueConverter") ??
            Templates.GetForAnyType("EfCoreValueConverter");

        if (item.IsTheWrapperAValueType)
        {
            code += """
                    
                            public class EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
                            {
                                public EfCoreValueComparer() : base(
                                    (left, right) => DoCompare(left, right), 
                                    instance => instance._isInitialized ? instance.GetHashCode() : 0) 
                                { 
                                }
                                
                                static bool DoCompare(VOTYPE left, VOTYPE right)
                                {
                                    // if neither are initialized, then they're equal
                                    if(!left._isInitialized && !right._isInitialized) return true;
                                    
                                    return left._isInitialized && right._isInitialized && left._value.Equals(right._value);
                                }
                            }
                    """;
        }
        else
        {
            code += """
                    
                            public class EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
                            {
                                public EfCoreValueComparer() : base(
                                    (left, right) => DoCompare(left, right), 
                                    instance => instance._isInitialized ? instance._value.GetHashCode() : 0) 
                                { 
                                }
                                    
                                static bool DoCompare(VOTYPE left, VOTYPE right)
                                {
                                    // if both null, then they're equal
                                    if (left is null) return right is null;
                                    
                                    // if only right is null, then they're not equal
                                    if (right is null) return false;
                                    
                                    // if they're both the same reference, then they're equal
                                    if (ReferenceEquals(left, right)) return true;
                                    
                                    // if neither are initialized, then they're equal
                                    if(!left._isInitialized && !right._isInitialized) return true;
                                    
                                    return left._isInitialized && right._isInitialized && left._value.Equals(right._value);            
                                }                
                            }
                    """;
        }

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);
        
        return code;
    }

    private static bool IsOurs(Vogen.Conversions conversions) => conversions.HasFlag(Vogen.Conversions.EfCoreValueConverter);
}