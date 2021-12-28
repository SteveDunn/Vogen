using System;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class Util
{
    public static string GenerateAnyInstances(TypeDeclarationSyntax classDeclarationSyntax, VoWorkItem item)
    {
        if (item.InstanceProperties.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();
        
        foreach (var each in item.InstanceProperties)
        {
            sb.AppendLine(GenerateAnyInstances_internal(each,classDeclarationSyntax, item));
        }

        return sb.ToString();
    }
    
    public static string GenerateValidation(VoWorkItem workItem)
    {
        if (workItem.ValidateMethod != null)
            return @$"var validation = {workItem.TypeToAugment.Identifier}.Validate(value);
            if (validation != Vogen.Validation.Ok)
            {{
                throw new Vogen.ValueObjectValidationException(validation.ErrorMessage);
            }}
";
        return string.Empty;
    }
    

    private static string GenerateAnyInstances_internal(
        InstanceProperties instanceProperties,
        TypeDeclarationSyntax classDeclarationSyntax, 
        VoWorkItem item)
    {
        if (item.InstanceProperties.Count == 0)
        {
            return string.Empty;
        }

        var instanceValue = BuildInstanceValue(item, instanceProperties.Value);

        return $@"
// instance...

public static {classDeclarationSyntax.Identifier} {instanceProperties.Name} = new {classDeclarationSyntax.Identifier}({instanceValue});";
    }

    private static string BuildInstanceValue(VoWorkItem item, object instancePropertiesValue)
    {
        if (item.UnderlyingType?.FullName() == typeof(String).FullName)
        {
            return $@"""{instancePropertiesValue}""";
        }

        if (item.UnderlyingType?.FullName() == typeof(decimal).FullName)
        {
            return $@"{instancePropertiesValue}m";
        }

        if (item.UnderlyingType?.FullName() == typeof(float).FullName)
        {
            return $@"{instancePropertiesValue}f";
        }

        if (item.UnderlyingType?.FullName() == typeof(double).FullName)
        {
            return $@"{instancePropertiesValue}d";
        }

        return instancePropertiesValue.ToString();
    }

    public static string GenerateModifiersFor(TypeDeclarationSyntax tds) => string.Join(" ", tds.Modifiers);

    public static string WriteStartNamespace(string @namespace)
    {
        if (string.IsNullOrEmpty(@namespace))
        {
            return string.Empty;
        }

        return @$"namespace {@namespace}
{{
";
    }

    public static string WriteCloseNamespace(string @namespace)
    {
        if (string.IsNullOrEmpty(@namespace))
        {
            return string.Empty;
        }

        return @$"}}";
    }
}