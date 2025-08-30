using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

internal class GenerateNewtonsoftJsonConversions : IGenerateConversion
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!IsOurs(item.Config.Conversions))
        {
            return string.Empty;
        }

        return $@"[global::Newtonsoft.Json.JsonConverter(typeof({item.VoTypeName}NewtonsoftJsonConverter))]";
    }

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!IsOurs(item.Config.Conversions))
        {
            return string.Empty;
        }

        var allowUninitialized = item.Config.Customizations.HasFlag(Customizations.AllowSerializingUninitializedNewtonSoft);

        string? code = Templates.TryGetForSpecificType(item.UnderlyingType, "NewtonsoftJsonConverter");
        
        if (code is null)
        {
            code = item.UnderlyingType.IsValueType
                ? Templates.GetForAnyType("NewtonsoftJsonConverterValueType")
                : Templates.GetForAnyType("NewtonsoftJsonConverterReferenceType");
        }

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullName);

        if (allowUninitialized)
        {
            code = code.Replace("serializer.Serialize(writer, id.Value);", @"
                if (id.IsInitialized())
                {
                    serializer.Serialize(writer, id.Value);
                }
                else
                {
                    writer.WriteNull();
                }");
        }

        return $"""
                #nullable disable
                
                {code}
                
                #nullable restore
                """;

    }

    private static bool IsOurs(Vogen.Conversions conversions) => conversions.HasFlag(Vogen.Conversions.NewtonsoftJson);
}