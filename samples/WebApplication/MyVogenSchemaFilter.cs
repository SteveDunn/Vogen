using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;

public class MyVogenSchemaFilter : ISchemaFilter
{                                
    private const BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance;

    public void Apply(Microsoft.OpenApi.Models.OpenApiSchema schema, SchemaFilterContext context)
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
            context.MemberInfo, 
            context.ParameterInfo);
        schemaValueObject.Description = $"This is the description for {context.Type}";
        
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