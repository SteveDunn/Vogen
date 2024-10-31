using System;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vogen.Generators.Conversions;

[assembly: InternalsVisibleTo("SmallTests")]
[assembly: InternalsVisibleTo("Vogen.Tests")]

namespace Vogen;

public static class Util
{
    public static string EscapeTypeNameForTripleSlashComment(string typeName) =>
        typeName.Replace("<", "{").Replace(">", "}");

    static readonly IGenerateConversion[] _conversionGenerators =
    {
        new GenerateSystemTextJsonConversions(),
        new GenerateNewtonsoftJsonConversions(),
        new GenerateTypeConverterConversions(),
        new GenerateDapperConversions(),
        new GenerateEfCoreTypeConversions(),
        new GenerateLinqToDbConversions()
    };

    public static string SanitizeToALegalFilename(string input) => input.Replace('@', '_');

    public static SourceText FormatSource(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
        SyntaxNode root = syntaxTree.GetRoot();
        SyntaxNode formatted = root.NormalizeWhitespace();

        return SourceText.From(formatted.ToFullString(), Encoding.UTF8);
    }

    public static void TryWriteUsingUniqueFilename(string filename, SourceProductionContext context, SourceText sourceText)
    {
        int count = 0;
        string hintName = filename;

        while (true)
        {
            try
            {
                context.AddSource(hintName, sourceText);
                return;
            }
            catch (ArgumentException)
            {
                if (++count >= 10)
                {
                    throw;
                }

                hintName = $"{count}{filename}";
            }
        }
    }


    public static string GenerateCallToValidationAndThrowIfRequired(VoWorkItem workItem)
    {
        if (workItem.ValidateMethod is not null)
        {
            return $$"""
                     var validation = {{workItem.TypeToAugment.Identifier}}.{{workItem.ValidateMethod.Identifier.Value}}(value);
                     if (validation != Vogen.Validation.Ok)
                     {
                         ThrowHelper.ThrowWhenValidationFails(validation);
                     }

                     """;
        }

        return string.Empty;
    }

    public static string GenerateCallToValidationAndReturnFalseIfNeeded(VoWorkItem workItem)
    {
        if (workItem.ValidateMethod is not null)
        {
            return @$"var validation = {workItem.TypeToAugment.Identifier}.{workItem.ValidateMethod.Identifier.Value}(value);
            if (validation != Vogen.Validation.Ok)
            {{
                vo = default!;
                return false;
            }}
";
        }

        return string.Empty;
    }

    public static string GenerateNotNullWhenTrueAttribute() =>
        """
        #if NETCOREAPP3_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
        #endif

        """;

    public static string GenerateMaybeNullWhenFalse() =>
        """
        #if NETCOREAPP3_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)]
        #endif

        """;


    public static string GenerateCallToValidationAndReturnValueObjectOrErrorIfNeeded(SyntaxToken className, VoWorkItem workItem)
    {
        if (workItem.ValidateMethod is not null)
        {
            return @$"var validation = {workItem.TypeToAugment.Identifier}.{workItem.ValidateMethod.Identifier.Value}(value);
            if (validation != Vogen.Validation.Ok)
            {{
                return new Vogen.ValueObjectOrError<{className}>(validation);
            }}
";
        }

        return string.Empty;
    }

    public static string GenerateCallToValidateForDeserializing(VoWorkItem workItem)
    {
        StringBuilder sb = new StringBuilder();

        if (workItem.Config.DeserializationStrictness.HasFlag(DeserializationStrictness.AllowKnownInstances))
        {
            foreach (var eachInstance in workItem.InstanceProperties)
            {
                string escapedName = EscapeIfRequired(eachInstance.Name);
                sb.AppendLine($"        if(value == {escapedName}.Value) return {escapedName};");
            }
        }

        if (workItem.ValidateMethod is null)
        {
            return sb.ToString();
        }

        if (workItem.Config.DeserializationStrictness.HasFlag(DeserializationStrictness.RunMyValidationMethod))
        {
            sb.AppendLine(
                $$"""
                  var validation = {{workItem.TypeToAugment.Identifier}}.{{workItem.ValidateMethod.Identifier.Value}}(value);
                  if (validation != Vogen.Validation.Ok)
                  {
                      ThrowHelper.ThrowWhenValidationFails(validation);
                  }
                  """);
        }

        return sb.ToString();
    }

    public static string GenerateCallToNormalizeMethodIfNeeded(VoWorkItem workItem, string nameOfValueVariable = "value")
    {
        if (workItem.NormalizeInputMethod is not null)
        {
            return
                @$"{nameOfValueVariable} = {workItem.TypeToAugment.Identifier}.{workItem.NormalizeInputMethod.Identifier.Value}({nameOfValueVariable});
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

    private static string GenerateAnyConversionAttributesForDebuggerProxy(VoWorkItem item) => item.Config.Conversions.ToString();

    public static string GenerateAnyConversionBodies(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var conversionGenerator in _conversionGenerators)
        {
            sb.AppendLine(conversionGenerator.GenerateAnyBody(tds, item));
        }

        return sb.ToString();
    }

    public static string GenerateDebuggerProxyForStructs(VoWorkItem item)
    {
        var createdWithMethod = item.Config.DisableStackTraceRecordingInDebug
            ? """
              public global::System.String CreatedWith => "the From method"
              """
            : $"""
               public global::System.String CreatedWith => _t._stackTrace{item.Nullable.QuestionMarkForOtherReferences}.ToString() ?? "the From method"
               """;

        string code =
            $$"""
              #nullable disable

              internal sealed class {{item.VoTypeName}}DebugView
              {
                  private readonly {{item.VoTypeName}} _t;
              
                  {{item.VoTypeName}}DebugView({{item.VoTypeName}} t)
                  {
                      _t = t;
                  }
              
                  public global::System.Boolean IsInitialized => _t.IsInitialized();
                  public global::System.String UnderlyingType => "{{item.UnderlyingTypeFullName}}";
                  public global::System.String Value => _t.IsInitialized() ? _t._value.ToString() : "[not initialized]" ;
              
                  #if DEBUG
                      {{createdWithMethod}};
                  #endif
              
                  public global::System.String Conversions => @"{{Util.GenerateAnyConversionAttributesForDebuggerProxy(item)}}";
              }

              #nullable restore
              """;

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

    public static string GenerateGuidFactoryMethodIfNeeded(VoWorkItem item)
    {
        if (item.UnderlyingTypeFullName == "System.Guid" && item.Config.Customizations.HasFlag(Customizations.AddFactoryMethodForGuids))
        {
            return $"public static {item.VoTypeName} FromNewGuid() => From(global::System.Guid.NewGuid());";
        }

        return string.Empty;
    }

    public static string GenerateIsInitializedMethod(bool @readonly, VoWorkItem item)
    {
        string ro = @readonly ? " readonly" : "";
        string accessibility = item.Config.IsInitializedMethodGeneration == IsInitializedMethodGeneration.Generate ? "public" : "private";
        return $$"""
                 [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                 #if VOGEN_NO_VALIDATION
                 #pragma warning disable CS8775
                   {{accessibility}}{{ro}} bool IsInitialized() => true;
                 #pragma warning restore CS8775
                 #else
                   {{accessibility}}{{ro}} bool IsInitialized() => _isInitialized;
                 #endif
                 """;
    }

    public static string EscapeTypeNameForTripleSlashComment(INamedTypeSymbol symbol)
    {
        var symbolToUse = symbol.IsGenericType ? symbol.OriginalDefinition : symbol;

        var displayString = symbolToUse.FullName() ?? symbolToUse.Name;

        return symbolToUse.IsGenericType
            ? EscapeTypeNameForTripleSlashComment(symbolToUse.ToDisplayString())
            : displayString;
    }

    public static string GenerateCommentForValueProperty(VoWorkItem item) =>
        $"""
         /// <summary>
         /// Gets the underlying <see cref="{EscapeTypeNameForTripleSlashComment(item.UnderlyingType)}" /> value if set, otherwise a <see cref="{EscapeTypeNameForTripleSlashComment(item.ValidationExceptionSymbol)}" /> is thrown.
         /// </summary>
         """;

    public static string GenerateEnsureInitializedMethod(VoWorkItem item, bool readOnly)
    {
        string ro = readOnly ? "readonly " : " ";
        string st = item.Config.DisableStackTraceRecordingInDebug ? string.Empty : "_stackTrace";
        return $$"""
                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         private {{ro}}void EnsureInitialized()
                         {
                             if (!IsInitialized())
                             {
                             #if DEBUG
                                ThrowHelper.ThrowWhenNotInitialized({{st}});
                            #else
                             ThrowHelper.ThrowWhenNotInitialized();
                            #endif
                             }
                         }
                 """;
    }

    public static string GenerateThrowHelper(VoWorkItem item)
    {
        return $$"""
                 static class ThrowHelper
                 {
                     #if NETCOREAPP3_0_OR_GREATER
                     [global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
                     #endif
                     internal static void ThrowInvalidOperationException(string message) => throw new global::System.InvalidOperationException(message);
                 
                     #if NETCOREAPP3_0_OR_GREATER
                     [global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
                     #endif
                     internal static void ThrowArgumentException(string message, string arg) => throw new global::System.ArgumentException(message, arg);
                 
                     #if NETCOREAPP3_0_OR_GREATER
                     [global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
                     #endif
                     internal static void ThrowWhenCreatedWithNull() => 
                            throw new {{item.ValidationExceptionFullName}}("Cannot create a value object with null.");
                 
                     #if NETCOREAPP3_0_OR_GREATER
                     [global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
                     #endif
                     internal static void ThrowWhenNotInitialized() => 
                        throw new {{item.ValidationExceptionFullName}}("Use of uninitialized Value Object.");
                     
                     #if NETCOREAPP3_0_OR_GREATER
                     [global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
                     #endif
                     internal static void ThrowWhenNotInitialized(global::System.Diagnostics.StackTrace{{item.Nullable.QuestionMarkForOtherReferences}} stackTrace) =>  
                        throw new {{item.ValidationExceptionFullName}}({{GetMessageToReport(item)}});
                 
                     #if NETCOREAPP3_0_OR_GREATER
                     [global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
                     #endif
                     internal static void ThrowWhenValidationFails(Vogen.Validation validation)
                     {
                         var ex = new {{item.ValidationExceptionFullName}}(validation.ErrorMessage);
                 
                         if (validation.Data is not null) 
                         {
                             foreach (var kvp in validation.Data)
                             {
                                 ex.Data[kvp.Key] = kvp.Value;
                             }
                         }
                         
                         throw ex;
                     }
                 }
                 """;

        static string GetMessageToReport(VoWorkItem item) =>
            item.Config.DisableStackTraceRecordingInDebug
                ? "\"Use of uninitialized Value Object.\""
                : "\"Use of uninitialized Value Object at: \" + stackTrace ?? \"\" ";
    }

    /// <summary>
    ///  If we're generating `IXmlSerializable`, then we can't specify that _value and _isInitialized are readonly
    /// as the 'ReadXml' method has to mutate these fields.
    /// </summary>
    /// <param name="workItem"></param>
    /// <returns></returns>
    public static string GetModifiersForValueAndIsInitializedFields(VoWorkItem workItem) => 
        workItem.Config.Conversions.HasFlag(Vogen.Conversions.XmlSerializable) ? "" : "readonly";
}

public static class DebugGeneration
{
    public static string GenerateDebugAttributes(VoWorkItem item, SyntaxToken className, string itemUnderlyingType)
    {
        var source = $$"""
                       [global::System.Diagnostics.DebuggerTypeProxyAttribute(typeof({{className}}DebugView))]
                           [global::System.Diagnostics.DebuggerDisplayAttribute("Underlying type: {{itemUnderlyingType}}, Value = { _value }")]
                       """;
        if (item.Config.DebuggerAttributes == DebuggerAttributeGeneration.Basic)
        {
            return
                $@"/* Debug attributes omitted because the 'debuggerAttributes' flag is set to {nameof(DebuggerAttributeGeneration.Basic)} on the Vogen attribute.
This is usually set to avoid issues in Rider where it doesn't fully handle the attributes support by Visual Studio and
causes Rider's debugger to crash.

{source}

*/";
        }

        return source;
    }

    public static string GenerateStackTraceFieldIfNeeded(VoWorkItem item)
    {
        if (item.Config.DisableStackTraceRecordingInDebug)
        {
            return string.Empty;
        }

        return $"""
                #if DEBUG   
                private readonly global::System.Diagnostics.StackTrace{item.Nullable.QuestionMarkForOtherReferences} _stackTrace = null!;
                #endif
                """;
    }

    public static string SetStackTraceIfNeeded(VoWorkItem voWorkItem) => voWorkItem.Config.DisableStackTraceRecordingInDebug
        ? string.Empty
        : "_stackTrace = new global::System.Diagnostics.StackTrace();";
}