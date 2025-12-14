using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

internal class GenerateSystemTextJsonConversions : IGenerateConversion
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item, VogenKnownSymbols knownSymbols)
    {
        if (knownSymbols.StjSerializer is null)
        {
            return string.Empty;
        }
        
        if (!item.Config.Conversions.HasFlag(Vogen.Conversions.SystemTextJson))
        {
            return string.Empty;
        }

        return $@"[global::System.Text.Json.Serialization.JsonConverter(typeof({item.VoTypeName}SystemTextJsonConverter))]";
    }

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item, VogenKnownSymbols knownSymbols)
    {
        if (knownSymbols.StjSerializer is null)
        {
            return string.Empty;
        }

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

        code = code.Replace("DESERIALIZEJSONMETHOD", GenerateDeserializeJsonMethod(item));

        code = code.Replace("VOTYPE", item.VoTypeName);
        code = code.Replace("VOUNDERLYINGTYPE", item.UnderlyingTypeFullNameWithGlobalAlias);

        return $"""
                #nullable disable
                {code}
                #nullable restore
                """;
    }

    private static string GenerateDeserializeJsonMethod(VoWorkItem item) => $$"""
          #if NETCOREAPP3_0_OR_GREATER
          [global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
          #endif
          private static void ThrowJsonExceptionWhenValidationFails(Vogen.Validation validation)
          {
              var e = ThrowHelper.CreateValidationException(validation);
              throw new global::System.Text.Json.JsonException(null, e);
          }
          {{GenerateThrowExceptionHelperIfNeeded(item)}}

          private static VOTYPE DeserializeJson(VOUNDERLYINGTYPE value)
          {
              {{GenerateNullCheckAndThrowJsonExceptionIfNeeded(item)}}
              {{Util.GenerateCallToNormalizeMethodIfNeeded(item)}}
              {{Util.GenerateChecksForKnownInstancesIfRequired(item)}}
              
              {{GenerateCodeForCallingValidation.CallWhenDeserializingAndCheckStrictnessFlag(item, "ThrowJsonExceptionWhenValidationFails")}}

              return new VOTYPE(value);
          }
          """
        .Replace("\n", "\n            ");

    private static string GenerateThrowExceptionHelperIfNeeded(VoWorkItem voWorkItem) =>
        voWorkItem.IsTheUnderlyingAValueType ? string.Empty
            : """
                
                private static void ThrowJsonExceptionWhenNull(VOUNDERLYINGTYPE value)
                {
                    if (value == null)
                    {
                        var e = ThrowHelper.CreateCannotBeNullException();
                        throw new global::System.Text.Json.JsonException(null, e);
                    }
                }
                """;

    private static string GenerateNullCheckAndThrowJsonExceptionIfNeeded(VoWorkItem voWorkItem) =>
        voWorkItem.IsTheUnderlyingAValueType ? string.Empty
            : $$"""
                ThrowJsonExceptionWhenNull(value);
                """;

    private static string ResolveTemplate(VoWorkItem item) =>
        Templates.TryGetForSpecificType(item.UnderlyingType, "SystemTextJsonConverter") ??
        Templates.GetForAnyType("SystemTextJsonConverter");
}