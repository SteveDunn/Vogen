using System;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Generators.Conversions;

[assembly: InternalsVisibleTo("SmallTests")]
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

    public static string GenerateCallToValidateForDeserializing(VoWorkItem workItem)
    {
        StringBuilder sb = new StringBuilder();

        if (workItem.DeserializationStrictness.HasFlag(DeserializationStrictness.AllowKnownInstances))
        {
            foreach (var eachInstance in workItem.InstanceProperties)
            {
                string escapedName = EscapeIfRequired(eachInstance.Name);
                sb.AppendLine($"        if(value == {escapedName}.Value) return {escapedName};");
            }
        }

        if (workItem.ValidateMethod == null)
        {
            return sb.ToString();
        }

        if (workItem.DeserializationStrictness.HasFlag(DeserializationStrictness.RunMyValidationMethod))
        {
            sb.AppendLine(@$"var validation = {workItem.TypeToAugment.Identifier}.{workItem.ValidateMethod.Identifier.Value}(value);
            if (validation != Vogen.Validation.Ok)
            {{
                throw new {workItem.ValidationExceptionFullName}(validation.ErrorMessage);
            }}");
        }

        return sb.ToString();
    }

    public static string GenerateNormalizeInputMethodIfNeeded(VoWorkItem workItem)
    {
        if (workItem.NormalizeInputMethod != null)
        {
            return @$"value = {workItem.TypeToAugment.Identifier}.{workItem.NormalizeInputMethod.Identifier.Value}(value);
";
        }

        return string.Empty;
    }


    public static string EscapeIfRequired(string name)
    {
        bool match = SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ||
                     SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None;

        return match ? "@" + name : name;
    }

    public static string GenerateModifiersFor(TypeDeclarationSyntax tds) => string.Join(" ", tds.Modifiers);

    public static string WriteStartNamespace(string @namespace)
    {
        if (string.IsNullOrEmpty(@namespace))
        {
            return string.Empty;
        }

        return @$"namespace {EscapeIfRequired(@namespace)}
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

    public static string GenerateYourAssemblyName() => typeof(Util).Assembly.GetName().Name!;
    public static string GenerateYourAssemblyVersion() => typeof(Util).Assembly.GetName().Version!.ToString();

    public static string GenerateToString(VoWorkItem item) =>
        item.HasToString ? string.Empty
            : $@"/// <summary>Returns the string representation of the underlying <see cref=""{item.UnderlyingTypeFullName}"" />.</summary>
    /// <inheritdoc cref=""{item.UnderlyingTypeFullName}.ToString()"" />
    public override global::System.String ToString() => Value.ToString();";

    public static string GenerateToStringReadOnly(VoWorkItem item) =>
        item.HasToString ? string.Empty :
            $@"/// <summary>Returns the string representation of the underlying type</summary>
    /// <inheritdoc cref=""{item.UnderlyingTypeFullName}.ToString()"" />
    public readonly override global::System.String ToString() => Value.ToString();";

    public static string GenerateIComparableHeaderIfNeeded(string precedingText, VoWorkItem item,
        TypeDeclarationSyntax tds)
    {
        if (item.UnderlyingType.ImplementsInterfaceOrBaseClass(typeof(IComparable<>)))
        {
            return $"{precedingText} global::System.IComparable<{tds.Identifier}>";
        }

        return string.Empty;
    }

    public static string GenerateIComparableImplementationIfNeeded(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        INamedTypeSymbol? primitiveSymbol = item.UnderlyingType;
        if (!primitiveSymbol.ImplementsInterfaceOrBaseClass(typeof(IComparable<>)))
        {
            return string.Empty;
        }

        var primitive = tds.Identifier;
        var s = @$"public int CompareTo({primitive} other) => Value.CompareTo(other.Value);";

         return s;
    }

    public static string GenerateDebugAttributes(VoWorkItem item, SyntaxToken className, string itemUnderlyingType)
    {
        var source = $$"""
[global::System.Diagnostics.DebuggerTypeProxyAttribute(typeof({{className}}DebugView))]
    [global::System.Diagnostics.DebuggerDisplayAttribute("Underlying type: {{itemUnderlyingType}}, Value = { _value }")]
""";
        if (item.OmitDebugAttributes)
        {
            return $@"/* Debug attributes omitted because the 'OmitDebugAttributes' flag is set on the Vogen attribute.
This is usually set to avoid issues in Rider where it doesn't fully handle the attributes support by Visual Studio and
causes Rider's debugger to crash

{source}

*/";
        }
    
        return source;
    }
}