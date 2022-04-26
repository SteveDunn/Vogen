using System;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Generators.Conversions;

[assembly: InternalsVisibleTo("Vogen.Tests")]

namespace Vogen;

public static class Util
{
    static readonly IGenerateConversion[] _conversionGenerators = 
    {
        new GenerateSystemTextJsonConversions(),
        new GenerateNewtonsoftJsonConversions(),
        new GenerateTypeConverterConversions(),
        new GenerateDapperConversions(),
        new GenerateEfCoreTypeConversions(),
        new GenerateLinqToDbConversions(),
    };
    
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
            return @$"var validation = {workItem.TypeToAugment.Identifier}.{workItem.ValidateMethod.Identifier.Value}(value);
            if (validation != Vogen.Validation.Ok)
            {{
                throw new {workItem.ValidationExceptionFullName}(validation.ErrorMessage);
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

    /// <summary>
    /// These are the attributes that are written to the top of the type, things like
    /// `TypeConverter`, `System.Text.JsonConverter` etc.
    /// </summary>
    /// <param name="tds"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string GenerateAnyConversionAttributes(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        StringBuilder sb = new StringBuilder();
        
        foreach (var conversionGenerator in _conversionGenerators)
        {
            var attribute = conversionGenerator.GenerateAnyAttributes(tds, item);
            if (!string.IsNullOrEmpty(attribute))
            {
                sb.AppendLine(attribute);
            }
        }

        return sb.ToString();
    }
    
    public static string GenerateAnyConversionAttributesForDebuggerProxy(TypeDeclarationSyntax tds, VoWorkItem item) => item.Conversions.ToString();

    public static string GenerateAnyConversionBodies(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var conversionGenerator in _conversionGenerators)
        {
            sb.AppendLine(conversionGenerator.GenerateAnyBody(tds, item));
        }
        
        return sb.ToString();
    }

    public static string GenerateDebuggerProxyForStructs(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        string code = $@"internal sealed class {item.VoTypeName}DebugView
        {{
            private readonly {item.VoTypeName} _t;

            {item.VoTypeName}DebugView({item.VoTypeName} t)
            {{
                _t = t;
            }}

            public global::System.Boolean IsInitialized => _t._isInitialized;
            public global::System.String UnderlyingType => ""{item.UnderlyingTypeFullName}"";
            public global::System.String Value => _t._isInitialized ? _t._value.ToString() : ""[not initialized]"" ;

            #if DEBUG
            public global::System.String CreatedWith => _t._stackTrace?.ToString() ?? ""the From method"";
            #endif

            public global::System.String Conversions => @""{Util.GenerateAnyConversionAttributesForDebuggerProxy(tds, item)}"";
                }}";

        return code;
    }

    public static string GenerateDebuggerProxyForClasses(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        string code = $@"internal sealed class {item.VoTypeName}DebugView
        {{
            private readonly {item.VoTypeName} _t;

            {item.VoTypeName}DebugView({item.VoTypeName} t)
            {{
                _t = t;
            }}

            public global::System.String UnderlyingType => ""{item.UnderlyingTypeFullName}"";
            public {item.UnderlyingTypeFullName} Value => _t.Value ;

            public global::System.String Conversions => @""{Util.GenerateAnyConversionAttributes(tds, item)}"";
                }}";

        return code;
    }
}