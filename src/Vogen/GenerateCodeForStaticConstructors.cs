using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Vogen;

internal static class GenerateConstructors
{
    public static string GenerateParameterlessStructConstructorIfAllowed(VoWorkItem item)
    {
        SyntaxToken wrapperName = item.TypeToAugment.Identifier;
        string wrapperBang = item.Nullable.BangForWrapper;
        
        if (item.LanguageVersion < LanguageVersion.CSharp10) return "";

        return $$"""
                         [global::System.Diagnostics.DebuggerStepThroughAttribute]
                         [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                         public {{wrapperName}}()
                         {
                 #if DEBUG
                             {{DebugGeneration.SetStackTraceIfNeeded(item)}}
                 #endif
                 

                 #if !VOGEN_NO_VALIDATION
                             _isInitialized = false;
                 #endif
                             _value = default{{wrapperBang}};
                         }
                 """;
    }
}

internal static class GenerateCodeForStaticConstructors
{
    public static string GenerateIfNeeded(VoWorkItem item)
    {
        if (!item.Config.Conversions.HasFlag(Conversions.ServiceStackDotText))
        {
            return string.Empty;
        }

        if (UnderlyingIsDateOrTimeRelated())
        {
            return $$"""
                     static {{item.VoTypeName}}() 
                     {
                        global::ServiceStack.Text.JsConfig<{{item.VoTypeName}}>.DeSerializeFn = v => {{item.VoTypeName}}.Parse(v, global::System.Globalization.CultureInfo.InvariantCulture);
                        global::ServiceStack.Text.JsConfig<{{item.VoTypeName}}>.SerializeFn = v => v.Value.ToString("o", global::System.Globalization.CultureInfo.InvariantCulture);
                     }
                     """;
        }

        if (UnderlyingIsADateTime())
        {
            return $$"""
                     static {{item.VoTypeName}}() 
                     {
                        global::ServiceStack.Text.JsConfig<{{item.VoTypeName}}>.DeSerializeFn = v => From(global::System.DateTime.ParseExact(v, "O", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.RoundtripKind));
                        global::ServiceStack.Text.JsConfig<{{item.VoTypeName}}>.SerializeFn = v => v.Value.ToUniversalTime().ToString("O", global::System.Globalization.CultureInfo.InvariantCulture);
                     }
                     """;
        }

        string deserialiseFn = item.IsUnderlyingAString
            ? $"{item.VoTypeName}.From"
            : $"v => {item.VoTypeName}.Parse(v)";

        return $$"""
                 static {{item.VoTypeName}}() 
                 {
                    global::ServiceStack.Text.JsConfig<{{item.VoTypeName}}>.DeSerializeFn = {{deserialiseFn}};
                    global::ServiceStack.Text.JsConfig<{{item.VoTypeName}}>.SerializeFn = v => v.ToString();
                 }
                 """;

        bool UnderlyingIsDateOrTimeRelated()
        {
            var symbol = item.UnderlyingType;
            
            if (symbol.ContainingNamespace.ToDisplayString() != "System")
            {
                return false;
            }
            
            return symbol.Name is "DateTimeOffset" or "TimeOnly" or "DateOnly";
        }

        bool UnderlyingIsADateTime()
        {
            var symbol = item.UnderlyingType;
            
            if (symbol.ContainingNamespace.ToDisplayString() != "System")
            {
                return false;
            }
            
            return symbol.Name is "DateTime";
        }
    }
}