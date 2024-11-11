using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Vogen.Generators.Conversions;

namespace Vogen;

internal class GenerateCodeForConversionMarkers
{
    public static void Generate(SourceProductionContext context, Compilation compilation, ImmutableArray<ConversionMarkerClassDefinition> conversionMarkerClasses)
    {
        if (!compilation.IsAtLeastCSharpVersion(LanguageVersion.CSharp12))
        {
            return;
        }
        
        foreach (ConversionMarkerClassDefinition? eachMarkerClass in conversionMarkerClasses)
        {
            var matchingMarkers = eachMarkerClass.AttributeDefinitions.Where(a => a.Marker?.Kind == ConverterMarkerKind.EFCore);
            
            WriteEncompassingExtensionMethod(
                eachMarkerClass.MarkerClassSymbol, 
                matchingMarkers, context);
            
            foreach (ConversionMarkerAttributeDefinition? eachAttributeInMarkerClass in matchingMarkers)
            {
                WriteEachIfNeeded(context, eachAttributeInMarkerClass);
            }
        }
    }

    private static void WriteEachIfNeeded(SourceProductionContext context, ConversionMarkerAttributeDefinition specResult)
    {
        ConverterMarker? spec = specResult.Marker;
        
        if (spec is null)
        {
            return;
        }

        var body = GenerateEfCoreTypes.GenerateOuter(spec.UnderlyingType, spec.VoSymbol.IsValueType, spec.VoSymbol);
        var extensionMethod = GenerateEfCoreTypes.GenerateOuterExtensionMethod(spec);

        var fullNamespace = spec.SourceType.FullNamespace();

        var isPublic = spec.SourceType.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        var ns = string.IsNullOrEmpty(fullNamespace) ? string.Empty : $"namespace {fullNamespace};";
        
        
        string sb =
$$"""
#if NET8_0_OR_GREATER

{{GeneratedCodeSegments.Preamble}}

{{ns}}
    
{{accessor}} partial class {{spec.SourceType.Name}}
{
    {{body}}
}

{{extensionMethod}}

#endif
""";
        
        SourceText sourceText = SourceText.From(sb, Encoding.UTF8);

        var unsanitized = $"{spec.SourceType.ToDisplayString()}_{spec.VoSymbol.ToDisplayString()}.g.cs";
        string filename = Util.SanitizeToALegalFilename(unsanitized);

        Util.TryWriteUsingUniqueFilename(filename, context, sourceText);
    }
    
    private static void WriteEncompassingExtensionMethod(
        INamedTypeSymbol? classDefinitionMarkerClassSymbol, 
        IEnumerable<ConversionMarkerAttributeDefinition> conversionMarkerAttributeDefinitions,
        SourceProductionContext context)
    {
        INamedTypeSymbol? markerSymbol = classDefinitionMarkerClassSymbol;
        
        if (markerSymbol is null)
        {
            return;
        }
        
        var fullNamespace = markerSymbol.FullNamespace();

        var isPublic = markerSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        var ns = string.IsNullOrEmpty(fullNamespace) ? string.Empty : $"namespace {fullNamespace};";

        string allCalls = GenerateBody();

        string source =
            $$"""
              #if NET8_0_OR_GREATER

              {{GeneratedCodeSegments.Preamble}}

              {{ns}}
                  
                {{accessor}} static class {{markerSymbol.Name}}__Ext
                {
                    {{accessor}} static global::Microsoft.EntityFrameworkCore.ModelConfigurationBuilder RegisterAllIn{{markerSymbol.Name}}(this global::Microsoft.EntityFrameworkCore.ModelConfigurationBuilder configurationBuilder)
                    {
                      {{allCalls}}
        
                      return configurationBuilder; 
                    }
                }

              #endif
              """;

        SourceText sourceText = SourceText.From(source, Encoding.UTF8);

        string filename = Util.SanitizeToALegalFilename($"{markerSymbol.ToDisplayString()}.g.cs");

        Util.TryWriteUsingUniqueFilename(filename, context, sourceText);
        
        return;

        string GenerateBody()
        {
            StringBuilder sb = new StringBuilder();

            foreach (ConversionMarkerAttributeDefinition eachSpec in conversionMarkerAttributeDefinitions)
            {
                if (eachSpec.Marker is null)
                {
                    continue;
                }

                var voSymbol = eachSpec.Marker.VoSymbol;
                sb.AppendLine($"configurationBuilder.Properties<{voSymbol.FullName()}>().HaveConversion<{markerSymbol.FullName()}.{voSymbol.Name}EfCoreValueConverter, {markerSymbol.FullName()}.{voSymbol.Name}EfCoreValueComparer>();");
            }

            return sb.ToString();
        }
    }
    
}