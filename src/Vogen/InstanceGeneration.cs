﻿using System;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class InstanceGeneration
{
    public static string GenerateAnyInstances(TypeDeclarationSyntax classDeclarationSyntax, VoWorkItem item)
    {
        if (item.InstanceProperties.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();

        foreach (InstanceProperties each in item.InstanceProperties)
        {
            sb.AppendLine(GenerateInstance(each, classDeclarationSyntax, item.FullNamespace));
        }

        return sb.ToString();
    }

    private static string GenerateInstance(
        InstanceProperties instanceProperties,
        TypeDeclarationSyntax classDeclarationSyntax, 
        string itemFullNamespace)
    {
        var instanceValue = instanceProperties.ValueAsText;

        return $@"
// instance...

{BuildInstanceComment(classDeclarationSyntax.Identifier, instanceProperties.TripleSlashComments, itemFullNamespace)}public static {classDeclarationSyntax.Identifier} {Util.EscapeIfRequired(instanceProperties.Name)} = new {classDeclarationSyntax.Identifier}({instanceValue});";
    }

    private static string BuildInstanceComment(SyntaxToken syntaxToken, string? commentText, string fullNamespace)
    {
        if (string.IsNullOrEmpty(commentText))
        {
            return string.Empty;
        }

        var x = new XElement("summary", commentText);
        var y = new XElement("returns", $"An immutable shared instance of \"T:{fullNamespace}.{syntaxToken}\"");

        return $@"    
/// {x}
/// {y}
";
    }

    public record BuildResult(bool Success, string Value, string ErrorMessage = "");

    public static BuildResult TryBuildInstanceValueAsText(string propertyName,
        object propertyValue, INamedTypeSymbol? itemUnderlyingType)
    {
        var underlyingType = itemUnderlyingType?.FullName();

        try
        {
            if (underlyingType == typeof(string).FullName)
            {
                return new(true, $@"""{propertyValue}""");
            }

            if (underlyingType == typeof(decimal).FullName)
            {
                if (propertyValue is char c)
                {
                    return new(true, string.Format(@"{0}m", c, CultureInfo.InvariantCulture));
                }

                return new(true,
                    Convert.ToDecimal(propertyValue).ToString(CultureInfo.InvariantCulture) + "m");
            }

            if (underlyingType == typeof(double).FullName)
            {
                if (propertyValue is char c)
                {
                    return new(true, string.Format(@"{0}d", c, CultureInfo.InvariantCulture));
                }

                return new(true,
                    Convert.ToDecimal(propertyValue).ToString(CultureInfo.InvariantCulture) + "d");
            }

            if (underlyingType == typeof(float).FullName)
            {
                if (propertyValue is char c)
                {
                    return new(true, string.Format(@"{0}f", c, CultureInfo.InvariantCulture));
                }

                return new(true,
                    Convert.ToSingle(propertyValue).ToString(CultureInfo.InvariantCulture) + "f");
            }

            if (underlyingType == typeof(char).FullName)
            {
                return new(true, $@"'{propertyValue}'");
            }

            if (underlyingType == typeof(byte).FullName)
            {
                return new(true, $@"{propertyValue}");
            }

            return new(true, propertyValue.ToString());
        }
        catch (Exception e)
        {
            return new(false, string.Empty,
                $"Instance value named {propertyName} has an attribute with a '{propertyValue.GetType()}' of '{propertyValue}' which cannot be converted to the underlying type of '{underlyingType}' - {e}");
        }
    }
}