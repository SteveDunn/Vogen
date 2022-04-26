using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

internal class GenerateSystemTextJsonConversions : IGenerateConversion
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!item.Conversions.HasFlag(Vogen.Conversions.SystemTextJson))
        {
            return string.Empty;
        }

        return $@"[global::System.Text.Json.Serialization.JsonConverter(typeof({item.VoTypeName}SystemTextJsonConverter))]";
    }

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!item.Conversions.HasFlag(Vogen.Conversions.SystemTextJson))
        {
            return string.Empty;
        }

        string code =
            Templates.TryGetForSpecificType(item.UnderlyingType, "SystemTextJsonConverter") ??
            Templates.GetForAnyType("SystemTextJsonConverter");

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);
        
        return code;
    }
}