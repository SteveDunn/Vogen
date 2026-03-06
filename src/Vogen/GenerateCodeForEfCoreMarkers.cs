using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Vogen.Generators.Conversions;

namespace Vogen;

internal static class GenerateCodeForEfCoreMarkers
{
    public static void Generate(SourceProductionContext context, Compilation compilation, MarkersCollection mcc)
    {
        if (!compilation.IsAtLeastCSharp12())
        {
            return;
        }

        ImmutableArray<MarkerAndAttributes> xxx = mcc.GetByKind(ConversionMarkerKind.EFCore);

        foreach (var eachMarkerClass in xxx)
        {
            var matchingMarkers = eachMarkerClass.Attributes;

            StoreExtensionMethodToRegisterAllInMarkerClass(eachMarkerClass.Symbol, matchingMarkers, context);
            
            foreach (MarkerAttributeDefinition? eachAttributeInMarkerClass in matchingMarkers)
            {
                WriteEachIfNeeded(context, eachAttributeInMarkerClass.Marker);
            }
        }
    }

    private static void WriteEachIfNeeded(SourceProductionContext context, ConversionMarker? markerClass)
    {
        if (markerClass is null)
        {
            return;
        }

        var body = GenerateEfCoreTypes.GenerateBodyForAMarkerClass(markerClass);
        var extensionMethod = GenerateEfCoreTypes.GenerateMarkerExtensionMethod(markerClass);

        var fullNamespace = markerClass.MarkerClassSymbol.FullUnalisaedNamespace();

        var isPublic = markerClass.MarkerClassSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        var ns = string.IsNullOrEmpty(fullNamespace) ? string.Empty : $"namespace {fullNamespace};";
        
        string sb =
$$"""
#if NET8_0_OR_GREATER

{{GeneratedCodeSegments.Preamble}}

{{ns}}
    
{{accessor}} partial class {{markerClass.MarkerClassSymbol.Name}}
{
    {{body}}
}

{{extensionMethod}}

#endif
""";
        
        SourceText sourceText = SourceText.From(sb, Encoding.UTF8);

        string filename = Util.GetLegalFilenameForMarkerClass(markerClass.MarkerClassSymbol, markerClass.VoSymbol, markerClass.Kind);

        Util.AddSourceToContext(filename, context, sourceText);
    }
    
    private static void StoreExtensionMethodToRegisterAllInMarkerClass(
        INamedTypeSymbol? markerSymbol, 
        IEnumerable<MarkerAttributeDefinition> markerAttributes,
        SourceProductionContext context)
    {
        if (markerSymbol is null)
        {
            return;
        }
        
        var fullNamespace = markerSymbol.FullUnalisaedNamespace();

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

        var filename = Util.GetLegalFilenameForMarkerClass(markerSymbol, ConversionMarkerKind.EFCore);

        Util.AddSourceToContext(filename, context, sourceText);
        
        return;

        string GenerateBody()
        {
            StringBuilder sb = new StringBuilder();

            foreach (MarkerAttributeDefinition eachSpec in markerAttributes)
            {
                if (eachSpec.Marker is null)
                {
                    continue;
                }

                var voSymbol = eachSpec.Marker.VoSymbol;
                sb.AppendLine($"configurationBuilder.Properties<{voSymbol.EscapedFullName()}>().HaveConversion<{markerSymbol.EscapedFullName()}.{voSymbol.Name}EfCoreValueConverter, {markerSymbol.EscapedFullName()}.{voSymbol.Name}EfCoreValueComparer>();");
            }

            return sb.ToString();
        }
    }
}