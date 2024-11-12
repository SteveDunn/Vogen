using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal class GenerateCodeForMessagePackMarkers
{
    public static void GenerateForMarkerClasses(
        SourceProductionContext context, 
        Compilation compilation, 
        ImmutableArray<MarkerClassDefinition> conversionMarkerClasses)
    {
        foreach (MarkerClassDefinition? eachMarkerClass in conversionMarkerClasses)
        {
            GenerateCodeForMessagePack.GenerateForAMarkerClass(context, compilation, eachMarkerClass);
        }
    }
}