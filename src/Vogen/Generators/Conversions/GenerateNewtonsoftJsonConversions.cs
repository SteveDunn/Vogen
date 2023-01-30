using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

internal class GenerateNewtonsoftJsonConversions : IGenerateConversion
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!IsOurs(item.Conversions))
        {
            return string.Empty;
        }

        return $@"[global::Newtonsoft.Json.JsonConverter(typeof({item.VoTypeName}NewtonsoftJsonConverter))]";
    }

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!IsOurs(item.Conversions))
        {
            return string.Empty;
        }

        string? code =
            Templates.TryGetForSpecificType(item.UnderlyingType, "NewtonsoftJsonConverter");
        if (code is null)
        {
            code = item.UnderlyingType.IsValueType ? Templates.GetForAnyType("NewtonsoftJsonConverterValueType") : Templates.GetForAnyType("NewtonsoftJsonConverterReferenceType");
        }

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);
        
        return code;
    }

    private static bool IsOurs(Vogen.Conversions conversions) => conversions.HasFlag(Vogen.Conversions.NewtonsoftJson);
}