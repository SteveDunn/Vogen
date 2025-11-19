using System.Collections.Generic;
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
        string inAppendage)
    {
        OpenApiVersionBeingUsed v = IsOpenApi2xReferenced(knownSymbols) ? OpenApiVersionBeingUsed.TwoPlus :
            IsOpenApi1xReferenced(knownSymbols) ? OpenApiVersionBeingUsed.One : OpenApiVersionBeingUsed.None; 
        
        if (v is OpenApiVersionBeingUsed.None)
        {
            return;
        }

        var sb = new StringBuilder();

        sb.AppendLine(GeneratedCodeSegments.Preamble);
        sb.AppendLine();
        sb.AppendLine("public static class VogenOpenApiExtensions");
        sb.AppendLine("{");
        sb
            .Append(_indent)
            .Append("public static global::Microsoft.AspNetCore.OpenApi.OpenApiOptions MapVogenTypes")
            .Append(inAppendage)
            .AppendLine("(this global::Microsoft.AspNetCore.OpenApi.OpenApiOptions options)");

        sb.Append(_indent).AppendLine("{");

        sb
            .Append(_indent, 2)
            .AppendLine("options.AddSchemaTransformer((schema, context, cancellationToken) =>");

        sb
            .Append(_indent, 2)
            .AppendLine("{");

        MapWorkItemsForOpenApi(workItems, sb, v);

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

        context.AddSource("OpenApiSchemaExtensions_g.cs", sb.ToString());
    }

    private static string MapWorkItemsForOpenApi(List<VoWorkItem> workItems, StringBuilder sb, OpenApiVersionBeingUsed v)
    {
        MapWorkItemsForOpenApi(workItems, sb, false, v);

        var valueTypes = workItems.Where(i => i.IsTheWrapperAValueType);
        MapWorkItemsForOpenApi(valueTypes, sb, true, v);

        return sb.ToString();
    }

    private static void MapWorkItemsForOpenApi(IEnumerable<VoWorkItem> workItems, StringBuilder sb, bool nullable, OpenApiVersionBeingUsed v)
    {
        foreach (VoWorkItem workItem in workItems)
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

    private static bool IsOpenApi1xReferenced(VogenKnownSymbols vogenKnownSymbols) => vogenKnownSymbols.OpenApiOptions is not null;
    private static bool IsOpenApi2xReferenced(VogenKnownSymbols vogenKnownSymbols) => vogenKnownSymbols.JsonSchemaType is not null;

    enum OpenApiVersionBeingUsed
    {
        None,
        One,
        TwoPlus
    }
}