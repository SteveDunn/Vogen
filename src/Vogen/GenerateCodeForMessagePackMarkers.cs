using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal class GenerateCodeForMessagePackMarkers
{
    public static void GenerateForMarkerClasses(SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<MarkerClassDefinition> conversionMarkerClasses,
        VogenKnownSymbols vogenKnownSymbols)
    {
        foreach (MarkerClassDefinition? eachMarkerClass in conversionMarkerClasses)
        {
            GenerateCodeForMessagePack.GenerateForAMarkerClass(context, eachMarkerClass);
        }
    }
}