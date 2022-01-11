using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

class GenerateTypeConverterConversions : IGenerateConversion
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!item.Conversions.HasFlag(Vogen.Conversions.TypeConverter))
        {
            return string.Empty;
        }

        return $@"[System.ComponentModel.TypeConverter(typeof({item.VoTypeName}TypeConverter))]";
    }

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!item.Conversions.HasFlag(Vogen.Conversions.TypeConverter))
        {
            return string.Empty;
        }

        string code = Templates.TryGetForSpecificType(item.UnderlyingType, "TypeConverter") ??
                       Templates.GetForAnyType("TypeConverter");

        code = code.Replace("VOTYPE", item.VoTypeName);
        return code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);
    }

}