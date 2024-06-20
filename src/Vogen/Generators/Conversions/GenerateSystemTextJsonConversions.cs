using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

internal class GenerateSystemTextJsonConversions : IGenerateConversion
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!item.Config.Conversions.HasFlag(Vogen.Conversions.SystemTextJson))
        {
            return string.Empty;
        }

        return $@"[global::System.Text.Json.Serialization.JsonConverter(typeof({item.VoTypeName}SystemTextJsonConverter))]";
    }

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!item.Config.Conversions.HasFlag(Vogen.Conversions.SystemTextJson))
        {
            return string.Empty;
        }

        string code = ResolveTemplate(item);

        bool allowNullReferences = true;

        if (item.IsTheWrapperAReferenceType)
        {
            // it's a class
            if (item.Config.DeserializationStrictness.HasFlag(DeserializationStrictness.DisallowNulls))
            {
                allowNullReferences = false;
            }
            
        }
        
        code = allowNullReferences ? CodeSections.CutSection(code, "__HANDLE_NULL__") : CodeSections.KeepSection(code, "__HANDLE_NULL__");

        if (code.Contains("__NORMAL__"))
        {
            (string keep, string cut) keepCut =
                item.Config.Customizations.HasFlag(Customizations.TreatNumberAsStringInSystemTextJson)
                    ? ("__STRING__", "__NORMAL__")
                    : ("__NORMAL__", "__STRING__");

            code = CodeSections.CutSection(code, keepCut.cut);
            code = CodeSections.KeepSection(code, keepCut.keep);
        }

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);

        return code;
    }

    private static string ResolveTemplate(VoWorkItem item) =>
        Templates.TryGetForSpecificType(item.UnderlyingType, "SystemTextJsonConverter") ??
        Templates.GetForAnyType("SystemTextJsonConverter");
}