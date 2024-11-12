using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Vogen.Generators.Conversions;

namespace Vogen;

internal class GenerateCodeEfCoreMarkers
{
    public static void Generate(SourceProductionContext context, Compilation compilation, ImmutableArray<MarkerClassDefinition> markerClasses)
    {
        if (!compilation.IsAtLeastCSharpVersion(LanguageVersion.CSharp12))
        {
            return;
        }
        
        foreach (MarkerClassDefinition? eachMarkerClass in markerClasses)
        {
            var matchingMarkers = eachMarkerClass.AttributeDefinitions.Where(a => a.Marker?.Kind == ConversionMarkerKind.EFCore);
            
            StoreExtensionMethodToRegisterAllInMarkerClass(eachMarkerClass.MarkerClassSymbol, matchingMarkers, context);
            
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

        var fullNamespace = markerClass.MarkerClassSymbol.FullNamespace();

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

        string filename = Util.GetLegalMarkerClassFilename(markerClass.MarkerClassSymbol, markerClass.VoSymbol, markerClass.Kind);

        Util.TryWriteUsingUniqueFilename(filename, context, sourceText);
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

        var filename = Util.GetLegalMarkerClassFilename(markerSymbol, ConversionMarkerKind.EFCore);

        //string filename = Util.SanitizeToALegalFilename($"{markerSymbol.ToDisplayString()}.g.cs");

        Util.TryWriteUsingUniqueFilename(filename, context, sourceText);
        
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
                sb.AppendLine($"configurationBuilder.Properties<{voSymbol.FullName()}>().HaveConversion<{markerSymbol.FullName()}.{voSymbol.Name}EfCoreValueConverter, {markerSymbol.FullName()}.{voSymbol.Name}EfCoreValueComparer>();");
            }

            return sb.ToString();
        }
    }
    
}