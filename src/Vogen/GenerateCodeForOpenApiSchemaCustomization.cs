using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Vogen.Types;

namespace Vogen;

internal class GenerateCodeForOpenApiSchemaCustomization
{
    private const char _indent = '\t'; // tab-level

    public static void WriteIfNeeded(VogenConfiguration? globalConfig,
        SourceProductionContext context,
        List<VoWorkItem> workItems,
        VogenKnownSymbols knownSymbols,
        ImmutableArray<MarkerClassDefinition> markerClasses,
        Compilation compilation)
    {
        GenerateCodeForAspNetCoreOpenApiSchema.WriteOpenApiSpecForMarkers(context, workItems, knownSymbols, markerClasses);

        var c = globalConfig?.OpenApiSchemaCustomizations ??
                VogenConfiguration.DefaultInstance.OpenApiSchemaCustomizations;

        var projectName = ProjectName.FromAssemblyName(compilation.Assembly.Name);

        var className = string.IsNullOrEmpty(projectName) ? string.Empty : $"MapVogenTypesIn{projectName}";

        if (c.HasFlag(OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter))
        {
            WriteSchemaFilter(context, knownSymbols, className);
        }

        if (c.HasFlag(OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod))
        {
            WriteSwashbuckleExtensionMethodMapping(context, workItems, knownSymbols, className);
        }

        if (c.HasFlag(OpenApiSchemaCustomizations.GenerateOpenApiMappingExtensionMethod))
        {
            GenerateCodeForAspNetCoreOpenApiSchema
                .WriteOpenApiExtensionMethodMapping(context, workItems, knownSymbols, className);
        }
    }

    private static void WriteSchemaFilter(SourceProductionContext context, VogenKnownSymbols knownSymbols, string inAppendage)
    {
        if (!OpenApiSchemaUtils.IsSwashbuckleReferenced(knownSymbols))
        {
            return;
        }

        var openApiVersion = OpenApiSchemaUtils.DetermineOpenApiVersionBeingUsed(knownSymbols);
        var openApiSchemaReference = openApiVersion switch
        {
            OpenApiVersionBeingUsed.One => "global::Microsoft.OpenApi.Models.OpenApiSchema",
            OpenApiVersionBeingUsed.TwoPlus => "global::Microsoft.OpenApi.IOpenApiSchema",
            _ => throw new ArgumentOutOfRangeException(nameof(openApiVersion), "Unknown or unimplemented schema version")
        };
        
        
        string source =
            $$"""

              {{GeneratedCodeSegments.Preamble}}

              using System.Reflection;

              {{Util.GenerateCoverageExcludeAndGeneratedCodeAttributes()}}
              public class VogenSchemaFilter{{inAppendage}} : global::Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter
              {                                
                  private const BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance;

                  public void Apply({{openApiSchemaReference}} schema, global::Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext context)
                  {
                      if (context.Type.GetCustomAttribute<Vogen.ValueObjectAttribute>() is not { } attribute)
                          return;

                      var type = attribute.GetType();
                      if (!type.IsGenericType || type.GenericTypeArguments.Length != 1)
                      {
                          return;
                      }

                      var schemaValueObject = context.SchemaGenerator.GenerateSchema(
                          type.GenericTypeArguments[0], 
                          context.SchemaRepository, 
                          context.MemberInfo, context.ParameterInfo);
                      
                      TryCopyPublicProperties(schemaValueObject, schema);
                  }

                  private static void TryCopyPublicProperties<T>(T oldObject, T newObject) where T : class
                  {
                      if (ReferenceEquals(oldObject, newObject))
                      {
                          return;
                      }

                      var type = typeof(T);
                      
                      var propertyList = type.GetProperties(_flags);
                      
                      if (propertyList.Length <= 0)
                      {
                          return;
                      }

                      foreach (var newObjProp in propertyList)
                      {
                          var oldProp = type.GetProperty(newObjProp.Name, _flags)!;
                          
                          if (!oldProp.CanRead || !newObjProp.CanWrite)
                          {
                              continue;
                          }

                          var value = oldProp.GetValue(oldObject);
                          newObjProp.SetValue(newObject, value);
                      }
                  }
              }
              """;

        context.AddSource("SwashbuckleSchemaFilter_g.cs", Util.FormatSource(source));
    }

    private static void WriteSwashbuckleExtensionMethodMapping(SourceProductionContext context,
        List<VoWorkItem> workItems,
        VogenKnownSymbols knownSymbols,
        string className)
    {
        var openApiVersion = OpenApiSchemaUtils.DetermineOpenApiVersionBeingUsed(knownSymbols);
        
        if (!OpenApiSchemaUtils.IsSwashbuckleReferenced(knownSymbols) || openApiVersion == OpenApiVersionBeingUsed.None)
        {
            return;
        }

        string source =
            $$"""

              {{GeneratedCodeSegments.Preamble}}

              public static class VogenSwashbuckleExtensions
              {
                  public static global::Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions {{className}}(this global::Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions o)
                  {
              {{MapWorkItems(workItems, openApiVersion)}}

                      return o;
                  }
              }
              """;

        context.AddSource("SwashbuckleSchemaExtensions_g.cs", source);
    }

    private static string MapWorkItems(List<VoWorkItem> workItems, OpenApiVersionBeingUsed openApiVersion)
    {
        var sb = new StringBuilder();

        var items = workItems.Select(workItem => new Item
        {
            IParsableIsAvailable = workItem.ParsingInformation.IParsableIsAvailable,
            UnderlyingTypeFullName = workItem.UnderlyingTypeFullName,
            VoTypeName = workItem.VoTypeName,
            FullAliasedNamespace = workItem.FullAliasedNamespace,
            IsTheWrapperAValueType = workItem.IsTheWrapperAValueType
        }).ToArray();

        // map everything an non-nullable
        MapWorkItems(items, sb, false, openApiVersion);

        // map value types again as nullable, see https://github.com/SteveDunn/Vogen/issues/693
        MapWorkItems(items.Where(i => i.IsTheWrapperAValueType), sb, true, openApiVersion);

        return sb.ToString();
    }

    private static void MapWorkItems(IEnumerable<Item> workItems, StringBuilder sb, bool nullable,
        OpenApiVersionBeingUsed openApiVersion)
    {
        foreach (var workItem in workItems)
        {
            sb.Append(_indent, 2);
            string voTypeName = workItem.VoTypeName;

            var fqn = string.IsNullOrEmpty(workItem.FullAliasedNamespace)
                ? $"{voTypeName}"
                : $"{workItem.FullAliasedNamespace}.{voTypeName}";

            if (nullable)
            {
                fqn = $"global::System.Nullable<{fqn}>";
            }

            TypeAndFormat typeAndPossibleFormat = MapUnderlyingTypeToJsonSchema(workItem);
            string formatText = typeAndPossibleFormat.Format.Length == 0 ? "" : $", Format = \"{typeAndPossibleFormat.Format}\"";

            switch (openApiVersion)
            {
                case OpenApiVersionBeingUsed.One:
                    {
                        string typeText = $"Type = \"{typeAndPossibleFormat.Type}\"";
                        string nullableText = $", Nullable = {nullable.ToString().ToLower()}";

                        sb.AppendLine(
                            $$"""global::Microsoft.Extensions.DependencyInjection.SwaggerGenOptionsExtensions.MapType<{{fqn}}>(o, () => new global::Microsoft.OpenApi.Models.OpenApiSchema { {{typeText}}{{formatText}}{{nullableText}} });""");
                        break;
                    }
                case OpenApiVersionBeingUsed.TwoPlus:
                    {
                        string typeText = $"Type = global::Microsoft.OpenApi.JsonSchemaType.{typeAndPossibleFormat.JsonSchemaType}";
                        if (nullable)
                        {
                            typeText += " | global::Microsoft.OpenApi.JsonSchemaType.Null";
                        }

                        sb.AppendLine(
                            $$"""global::Microsoft.Extensions.DependencyInjection.SwaggerGenOptionsExtensions.MapType<{{fqn}}>(o, () => new global::Microsoft.OpenApi.OpenApiSchema { {{typeText}}{{formatText}} });""");
                        break;
                    }
            }
        }
    }

    public class Item
    {
        public required string UnderlyingTypeFullName { get; init; }
        public required bool IParsableIsAvailable { get; init; }
        public required string VoTypeName { get; init; }
        public required string FullAliasedNamespace { get; init; }
        public required bool IsTheWrapperAValueType { get; init; }
    }

    internal record struct TypeAndFormat(string Type, string JsonSchemaType, string Format);

    // see https://spec.openapis.org/oas/v3.0.0.html#data-types
    internal static TypeAndFormat MapUnderlyingTypeToJsonSchema(Item workItem)
    {
        var primitiveType = workItem.UnderlyingTypeFullName;

        TypeAndFormat jsonType = primitiveType switch
        {
            "System.Int32" => new("integer", "Number", "int32"),
            "System.Int64" => new("integer", "Number", "int64"),
            "System.Int16" => new("number", "Number", ""),
            "System.Single" => new("number", "Number", ""),
            "System.Decimal" => new("number", "Number", "double"),
            "System.Double" => new("number", "Number", "double"),
            "System.String" => new("string", "String", ""),
            "System.Boolean" => new("boolean", "Boolean", ""),
            "System.DateOnly" => new("string", "String", "date"),
            "System.DateTime" => new("string", "String", "date-time"),
            "System.DateTimeOffset" => new("string", "String", "date-time"),
            "System.Guid" => new("string", "String", "uuid"),
            "System.Byte" => new("string", "String", "byte"),
            _ => TryMapComplexPrimitive(workItem.IParsableIsAvailable)
        };

        return jsonType;
    }

    private static TypeAndFormat TryMapComplexPrimitive(bool iParsableIsAvailable)
    {
        if (iParsableIsAvailable)
        {
            return new("string", "String", "");
        }

        return new("object", "Object", "");
    }
}