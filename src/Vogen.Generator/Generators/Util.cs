using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generator.generators;

public static class Util
{
    public static string GenerateAnyInstances(TypeDeclarationSyntax classDeclarationSyntax, ValueObjectWorkItem item,
        List<string> log)
    {
        if (item.InstanceProperties.Count == 0)
        {
            log.Add("No InstanceProperties on WorkItem - doing no instance properties");
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();
        
        foreach (var each in item.InstanceProperties)
        {
            sb.AppendLine(GenerateAnyInstances_internal(each,classDeclarationSyntax, item, log));
        }

        return sb.ToString();
    }
    
    public static string GenerateValidation(ValueObjectWorkItem workItem)
    {
        if (workItem.ValidateMethod != null)
            return @$"var validation = {workItem.TypeToAugment.Identifier}.Validate(value);
            if (validation != Vogen.SharedTypes.Validation.Ok)
            {{
                throw new Vogen.SharedTypes.ValueObjectValidationException(validation.ErrorMessage);
            }}
";
        return string.Empty;
    }
    

    private static string GenerateAnyInstances_internal(
        InstanceProperties instanceProperties,
        TypeDeclarationSyntax classDeclarationSyntax, 
        ValueObjectWorkItem item,
        List<string> log)
    {
        if (item.InstanceProperties.Count == 0)
        {
            log.Add("No InstanceProperties on WorkItem - doing no instance properties");
            return string.Empty;
        }

        var instanceValue = BuildInstanceValue(item, instanceProperties.Value, log);
        log.Add($"InstanceProperties found on WorkItem: name: '{instanceProperties.Name}', value: '{instanceValue}'");

        return $@"
// instance...

public static {classDeclarationSyntax.Identifier} {instanceProperties.Name} = new {classDeclarationSyntax.Identifier}({instanceValue});";
    }

    private static string BuildInstanceValue(ValueObjectWorkItem item, object instancePropertiesValue, List<string> log)
    {
        log.Add($"underlying type is '{item.UnderlyingType?.FullName()}'");

        if (item.UnderlyingType?.FullName() == typeof(String).FullName)
        {
            return $@"""{instancePropertiesValue}""";
        }

        return instancePropertiesValue.ToString();
    }

}