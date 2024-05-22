using Microsoft.CodeAnalysis;

namespace Vogen;

internal class WriteSwashbuckleSchemaFilter
{
    public static void WriteIfNeeded(
        VogenConfiguration? globalConfig,
        SourceProductionContext context,
        Compilation compilation)
    {
        if (compilation.GetTypeByMetadataName("Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter") is null)
        {
            return;
        }
        
        var generation = globalConfig?.SwashbuckleSchemaFilterGeneration ??
                         VogenConfiguration.DefaultInstance.SwashbuckleSchemaFilterGeneration;

        if (generation == SwashbuckleSchemaFilterGeneration.Omit)
        {
            return;
        }

        string s2 =
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

        context.AddSource("SwashbuckleSchemaFilter_g.cs", s2);
    }
}