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
        if (!IsOurs(item.Conversions))
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
                                    public EfCoreValueComparer() : base((left, right) => (!left._isInitialized && !right._isInitialized) || (left._isInitialized && right._isInitialized && left.Equals(right)), instance => instance._isInitialized ? instance._value.GetHashCode() : 0) { }
                                }
                    """;
        }
        else
        {
            code += """
                    
                                public class EfCoreValueComparer : global::Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<VOTYPE>
                                {
                                    public EfCoreValueComparer() : base((left, right) => (left == (VOTYPE)null && right == (VOTYPE)null) || (!left._isInitialized && !right._isInitialized) || (left._isInitialized && right._isInitialized && left.Equals(right)), instance => instance._isInitialized ? instance._value.GetHashCode() : 0) { }
                                }
                    """;
        }

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);
        
        return code;
    }

    private static bool IsOurs(Vogen.Conversions conversions) => conversions.HasFlag(Vogen.Conversions.EfCoreValueConverter);
}