using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class Util
{
    public static string GenerateAnyInstances(TypeDeclarationSyntax classDeclarationSyntax, VoWorkItem item,
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
        VoWorkItem item,
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

    private static string BuildInstanceValue(VoWorkItem item, object instancePropertiesValue, List<string> log)
    {
        log.Add($"underlying type is '{item.UnderlyingType?.FullName()}'");

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
}