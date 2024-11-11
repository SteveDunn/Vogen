using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Vogen;

internal class GenerateCodeForMessagePackMarkers
{
    public static void GenerateForMarkerClasses(
        SourceProductionContext context, 
        Compilation compilation, 
        ImmutableArray<ConversionMarkerClassDefinition> conversionMarkerClasses)
    {
        if (!compilation.IsAtLeastCSharpVersion(LanguageVersion.CSharp12))
        {
            return;
        }
        
        foreach (ConversionMarkerClassDefinition? eachMarkerClass in conversionMarkerClasses)
        {
            var matchingMarkers = eachMarkerClass.AttributeDefinitions.Where(a => a.Marker?.Kind == ConverterMarkerKind.MessagePack);

            var ps = matchingMarkers.Select(ConvertToParams).ToList();

            GenerateCodeForMessagePack.Generate(context, compilation, ps);
        }
    }

    private static MessagePackGeneratorParams ConvertToParams(ConversionMarkerAttributeDefinition spec) =>
        new(
            spec.Marker!.SourceType.FullNamespace(),
            spec.Marker!.VoSymbol,
            spec.Marker!.UnderlyingType,
            "public");
}