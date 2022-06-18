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

        string code = ResolveTemplate(item);
        if (code.Contains("__NORMAL__"))
        {
            (string keep, string cut) keepCut =
                item.Customizations.HasFlag(Customizations.TreatNumberAsStringInSystemTextJson)
                    ? ("__STRING__", "__NORMAL__") : ("__NORMAL__", "__STRING__");

            code = CodeSections.CutSection(code, keepCut.cut);
            code = CodeSections.KeepSection(code, keepCut.keep);
        }

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);
        
        return code;
    }

    private string ResolveTemplate(VoWorkItem item) =>
        Templates.TryGetForSpecificType(item.UnderlyingType, "SystemTextJsonConverter") ??
        Templates.GetForAnyType("SystemTextJsonConverter");
}