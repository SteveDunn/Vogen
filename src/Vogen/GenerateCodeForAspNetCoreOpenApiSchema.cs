using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using static Vogen.GenerateCodeForOpenApiSchemaCustomization;

namespace Vogen;

internal static class GenerateCodeForAspNetCoreOpenApiSchema
{
    private const char _indent = '\t'; // tab-level

    internal static void WriteOpenApiExtensionMethodMapping(
        SourceProductionContext context,
        List<VoWorkItem> workItems,
        VogenKnownSymbols knownSymbols,
        string className)
    {
        var items = workItems.Select(eachItem =>
            new Item
            {
                VoTypeName = eachItem.VoTypeName,
                IsTheWrapperAValueType = eachItem.IsTheWrapperAValueType,
                FullAliasedNamespace = eachItem.FullAliasedNamespace,
                IParsableIsAvailable = knownSymbols.IParsableOfT is not null,
                UnderlyingTypeFullName = eachItem.UnderlyingType.EscapedFullName()
            }).ToList();
        
        var openApiVersion = OpenApiSchemaUtils.DetermineOpenApiVersionBeingUsed(knownSymbols);

        if (!OpenApiSchemaUtils.IsOpenApiOptionsReferenced(knownSymbols) || openApiVersion is OpenApiVersionBeingUsed.None)
        {
            return;
        }

        WriteOpenApiExtensionMethodMapping(context, items, knownSymbols, className, openApiVersion);
    }

    private static void WriteOpenApiExtensionMethodMapping(
        SourceProductionContext context,
        List<Item> workItems,
        VogenKnownSymbols knownSymbols,
        string className, 
        OpenApiVersionBeingUsed openApiVersion)
    {
        var sb = new StringBuilder();

        sb.AppendLine(GeneratedCodeSegments.Preamble);
        sb.AppendLine();
        sb.AppendLine("public static partial class VogenOpenApiExtensions");
        sb.AppendLine("{");
        sb
            .Append(_indent)
            .Append($"public static global::Microsoft.AspNetCore.OpenApi.OpenApiOptions {className}")
            .AppendLine("(this global::Microsoft.AspNetCore.OpenApi.OpenApiOptions options)");

        sb.Append(_indent).AppendLine("{");

        sb
            .Append(_indent, 2)
            .AppendLine("options.AddSchemaTransformer((schema, context, cancellationToken) =>");

        sb
            .Append(_indent, 2)
            .AppendLine("{");

        MapWorkItemsForOpenApi(workItems, sb, openApiVersion);

        sb
            .Append(_indent, 3)
            .AppendLine("return global::System.Threading.Tasks.Task.CompletedTask;");

        sb
            .Append(_indent, 2)
            .AppendLine("});");

        sb.AppendLine();
        sb.Append(_indent, 2).AppendLine("return options;");
        sb.Append(_indent).AppendLine("}");
        sb.AppendLine("}");

        context.AddSource($"_{className}_g.cs", sb.ToString());
    }

    private static void MapWorkItemsForOpenApi(List<Item> workItems, StringBuilder sb, OpenApiVersionBeingUsed v)
    {
        MapWorkItemsForOpenApi(workItems, sb, false, v);

        var valueTypes = workItems.Where(i => i.IsTheWrapperAValueType);
        MapWorkItemsForOpenApi(valueTypes, sb, true, v);
    }

    private static void MapWorkItemsForOpenApi(IEnumerable<Item> workItems, StringBuilder sb, bool nullable, OpenApiVersionBeingUsed v)
    {
        foreach (var workItem in workItems)
        {
            string voTypeName = workItem.VoTypeName;
            string ns = workItem.FullAliasedNamespace;

            string fqn = string.IsNullOrEmpty(ns) ? voTypeName : $"{ns}.{voTypeName}";
            string typeExpression = nullable
                ? $"global::System.Nullable<{fqn}>"
                : $"{(string.IsNullOrEmpty(ns) ? "" : ns + ".")}{voTypeName}";

            TypeAndFormat typeAndPossibleFormat = MapUnderlyingTypeToJsonSchema(workItem);

            sb.Append(_indent, 3).AppendLine($"if (context.JsonTypeInfo.Type == typeof({typeExpression}))");
            sb.Append(_indent, 3).AppendLine("{");

            if (v is OpenApiVersionBeingUsed.One)
            {
                sb.Append(_indent, 4).AppendLine($"schema.Type = \"{typeAndPossibleFormat.Type}\";");
                if (nullable)
                {
                    sb.Append(_indent, 4).AppendLine("schema.Nullable = true;");
                }
            }

            if (v is OpenApiVersionBeingUsed.TwoPlus)
            {
                string t = $"Microsoft.OpenApi.JsonSchemaType.{typeAndPossibleFormat.JsonSchemaType}";
                if (nullable)
                {
                    t += " | Microsoft.OpenApi.JsonSchemaType.Null";
                }
                
                sb.Append(_indent, 4).AppendLine($"schema.Type = {t};");
            }

            if (!string.IsNullOrEmpty(typeAndPossibleFormat.Format))
            {
                sb.Append(_indent, 4).AppendLine($"schema.Format = \"{typeAndPossibleFormat.Format}\";");
            }

            sb.Append(_indent, 3).AppendLine($"}}");
            sb.AppendLine();
        }
    }

    public static void WriteOpenApiSpecForMarkers(SourceProductionContext context,
        List<VoWorkItem> workItems,
        VogenKnownSymbols knownSymbols,
        ImmutableArray<MarkerClassDefinition> markerClasses)
    {
        foreach (MarkerClassDefinition eachMarkerClass in markerClasses)
        {
            var matchingMarkers = eachMarkerClass.AttributeDefinitions.Where(a => a.Marker?.Kind == ConversionMarkerKind.OpenApi).ToList();

            if (matchingMarkers.Count == 0)
            {
                continue;
            }

            var items = eachMarkerClass.AttributeDefinitions.Select(ad => ad.Marker).Where(m => m is not null).Select(eachItem =>
                new Item
                {
                    IsTheWrapperAValueType = eachItem!.VoSymbol.IsValueType,
                    FullAliasedNamespace = eachItem.VoSymbol.FullAliasedNamespace(),
                    IParsableIsAvailable = knownSymbols.IParsableOfT is not null,
                    UnderlyingTypeFullName = eachItem.UnderlyingTypeSymbol.EscapedFullName(),
                    VoTypeName = eachItem.VoSymbol.Name
                }).ToList();

            WriteOpenApiExtensionMethodMapping(
                context,
                items,
                knownSymbols,
                $"MapVogenTypesIn{eachMarkerClass.MarkerClassSymbol.Name}",
                OpenApiVersionBeingUsed.TwoPlus);
        }
    }
}