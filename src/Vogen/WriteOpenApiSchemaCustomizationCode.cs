using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal class WriteOpenApiSchemaCustomizationCode
{
    public static void WriteIfNeeded(VogenConfiguration? globalConfig,
        SourceProductionContext context,
        List<VoWorkItem> workItems,
        VogenKnownSymbols knownSymbols)
    {
        var c = globalConfig?.OpenApiSchemaCustomizations ?? VogenConfiguration.DefaultInstance.OpenApiSchemaCustomizations;

        if (c.HasFlag(OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter))
        {
            WriteSchemaFilter(context, knownSymbols);
        }

        if (c.HasFlag(OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod))
        {
            WriteExtensionMethodMapping(context, workItems, knownSymbols);
        }
    }

    private static void WriteSchemaFilter(SourceProductionContext context, VogenKnownSymbols knownSymbols)
    {
        if (!IsSwashbuckleReferenced(knownSymbols))
        {
            return;
        }
        
        string source =
            $$"""

              {{GeneratedCodeSegments.Preamble}}

              using System.Reflection;
              
              public class VogenSchemaFilter : global::Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter
              {                                
                  private const BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance;

                  public void Apply(global::Microsoft.OpenApi.Models.OpenApiSchema schema, global::Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext context)
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

    private static void WriteExtensionMethodMapping(SourceProductionContext context,
        List<VoWorkItem> workItems,
        VogenKnownSymbols knownSymbols)
    {
        if (!IsSwashbuckleReferenced(knownSymbols))
        {
            return;
        }
        
        string source =
            $$"""

              {{GeneratedCodeSegments.Preamble}}

              public static class VogenSwashbuckleExtensions
              {
                  public static global::Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions MapVogenTypes(this global::Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions o)
                  {
                      {{MapWorkItems(workItems)}}
              
                      return o;
                  }
              }
              """;

        context.AddSource("SwashbuckleSchemaExtensions_g.cs", source);
    }

    private static bool IsSwashbuckleReferenced(VogenKnownSymbols vogenKnownSymbols) => vogenKnownSymbols.SwaggerISchemaFilter is not null;

    private static string MapWorkItems(List<VoWorkItem> workItems)
    {
        var workItemCode = new StringBuilder();
        foreach (var workItem in workItems)
        {
            var fqn = string.IsNullOrEmpty(workItem.FullNamespace)
                ? $"{workItem.VoTypeName}"
                : $"{workItem.FullNamespace}.{workItem.VoTypeName}";

            TypeAndFormat typeAndPossibleFormat = MapUnderlyingTypeToJsonSchema(workItem);
            string typeText = $"Type = \"{typeAndPossibleFormat.Type}\"";
            string formatText = typeAndPossibleFormat.Format.Length == 0 ? "" : $", Format = \"{typeAndPossibleFormat.Format}\"";
            
            workItemCode.AppendLine(
                $$"""global::Microsoft.Extensions.DependencyInjection.SwaggerGenOptionsExtensions.MapType<{{fqn}}>(o, () => new global::Microsoft.OpenApi.Models.OpenApiSchema { {{typeText}}{{formatText}} });""");
        }

        return workItemCode.ToString();
    }

    record struct TypeAndFormat(string Type, string Format = "");

    private static TypeAndFormat MapUnderlyingTypeToJsonSchema(VoWorkItem workItem)
    {
        var primitiveType = workItem.UnderlyingTypeFullName;
        
        TypeAndFormat jsonType = primitiveType switch
        {
            "System.Int32" => new("integer"),
            "System.Single" => new("number"),
            "System.Decimal" =>new( "number"),
            "System.Double" => new("number"),
            "System.String" => new("string"),
            "System.Boolean" =>new( "boolean"),
            "System.Guid" =>new( "string", "uuid"),
            _ => new(TryMapComplexPrimitive(workItem))
        };

        return jsonType;
    }

    private static string TryMapComplexPrimitive(VoWorkItem workItem)
    {
        if (workItem.ParsingInformation.IParsableIsAvailable)
        {
            return "string";
        }

        return "object";
    }
}