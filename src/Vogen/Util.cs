using System;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vogen.Generators.Conversions;
using Vogen.Types;

[assembly: InternalsVisibleTo("Vogen.Tests, PublicKey=00240000048000001401000006020000002400005253413100080000010001008575302965198d963e8bcffd71091f0d00be96f44db024d49d03b04c5e47a3c4d2d53c25ec89558c91bb68993a67b2461daff8d5a4a930f51982887dda3c5b2f3ad2686b18d32aee14c5b8182e25dfc6f281fb033beb9c2efa0297a7f9b4e33d93d60e453cb2f8abccc18d163cc2d82ad7e89fe65cdd4be1c9f2bfd958e59bee80fa7131e411a9e8466c6c593e998c142fd16795d2c80716c9c00283617d58a6eeecf27c281ed044277f79455e10b52a275784c53febf9a6ac456c8a5b6d628c3bc5321f09ddfe0da07e311a73c70621bedef7f48832a730fcab9b378694dfc04037e24339c5d3d04ad0f595f359c7ec99bb301236f7b4d30063b89534d3348e")]

namespace Vogen;

internal static class Util
{
    public static string GenerateNullCheckAndThrowIfNeeded(VoWorkItem voWorkItem, bool generateReturnDefault = false)
    {
        string returnStatement = generateReturnDefault ? "return default!;" : string.Empty;
        return voWorkItem.IsTheUnderlyingAValueType
            ? string.Empty
            : $$"""
                  if (value is null)
                  {
                      ThrowHelper.ThrowWhenCreatedWithNull();
                      {{returnStatement}}
                  }

              """;
    }

    public static string EscapeTypeNameForTripleSlashComment(string typeName) =>
        typeName.Replace("<", "{").Replace(">", "}");

    static readonly IGenerateConversion[] _conversionGenerators =
    {
        new GenerateSystemTextJsonConversions(),
        new GenerateNewtonsoftJsonConversions(),
        new GenerateTypeConverterConversions(),
        new GenerateDapperConversions(),
        new GenerateEfCoreTypeConversions(),
        new GenerateLinqToDbConversions(),
        new GenerateCodeForMessagePack()
    };

    public static string SanitizeToALegalFilename(string input) => input.Replace('@', '_');

    public static SourceText FormatSource(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
        SyntaxNode root = syntaxTree.GetRoot();
        SyntaxNode formatted = root.NormalizeWhitespace();

        return SourceText.From(formatted.ToFullString(), Encoding.UTF8);
    }

    public static void TryWriteUsingUniqueFilename(Filename filename, SourceProductionContext context, SourceText sourceText)
        => TryWriteUsingUniqueFilename(filename.Value, context, sourceText);

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

    public static string GenerateChecksForKnownInstancesIfRequired(VoWorkItem workItem)
    {
        if (!workItem.Config.DeserializationStrictness.HasFlag(DeserializationStrictness.AllowKnownInstances))
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();
        
        foreach (var eachInstance in workItem.InstanceProperties)
        {
            string escapedName = EscapeKeywordsIfRequired(eachInstance.Name);
            sb.AppendLine($"        if(value == {escapedName}.Value) return {escapedName};");
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


    public static string EscapeKeywordsIfRequired(string name)
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

        return @$"namespace {EscapeKeywordsIfRequired(@namespace)}
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
        if (symbol.IsTupleType)
        {
            return EscapeTypeNameForTripleSlashComment(symbol.OriginalDefinition.ToDisplayString(_formatForTuples));
        }

        var symbolToUse = symbol.IsGenericType ? symbol.OriginalDefinition : symbol;

        var displayString = symbolToUse.EscapedFullName();

        return symbolToUse.IsGenericType
            ? EscapeTypeNameForTripleSlashComment(symbolToUse.ToDisplayString())
            : displayString;
    }

    // private static readonly SymbolDisplayFormat _myFormat =
    //     new(
    //             typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
        

    private static readonly SymbolDisplayFormat _formatForTuples =
        new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions:
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                SymbolDisplayMiscellaneousOptions.UseSpecialTypes
#if ROSLYN_4_6_OR_GREATER
                | SymbolDisplayMiscellaneousOptions.ExpandValueTuple
#endif
            );
    
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
        return $$$"""
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
                             throw CreateCannotBeNullException();
                  
                      #if NETCOREAPP3_0_OR_GREATER
                      [global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
                      #endif
                      internal static void ThrowWhenNotInitialized() => 
                         throw CreateValidationException("Use of uninitialized Value Object.");
                      
                      #if NETCOREAPP3_0_OR_GREATER
                      [global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
                      #endif
                      internal static void ThrowWhenNotInitialized(global::System.Diagnostics.StackTrace{{{item.Nullable.QuestionMarkForOtherReferences}}} stackTrace) =>  
                         throw CreateValidationException({{{GetMessageToReport(item)}}});
                  
                      #if NETCOREAPP3_0_OR_GREATER
                      [global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
                      #endif
                      internal static void ThrowWhenValidationFails(Vogen.Validation validation)
                      {
                          throw CreateValidationException(validation);
                      }
                      
                      internal static System.Exception CreateValidationException(string message) =>
                        new {{{item.ValidationExceptionFullName}}}(message);

                      internal static System.Exception CreateCannotBeNullException() =>
                        new {{{item.ValidationExceptionFullName}}}("Cannot create a value object with null.");
                      
                      internal static System.Exception CreateValidationException(Vogen.Validation validation)
                      {
                          var ex = CreateValidationException(validation.ErrorMessage);
                      
                          if (validation.Data != null) 
                          {
                              foreach (var kvp in validation.Data)
                              {
                                  ex.Data[kvp.Key] = kvp.Value;
                              }
                          }
                          
                          return ex;
                      }
                  }
                  """;

        static string GetMessageToReport(VoWorkItem item) =>
            item.Config.DisableStackTraceRecordingInDebug
                ? "\"Use of uninitialized Value Object.\""
                : "\"Use of uninitialized Value Object at: \" + stackTrace ?? \"\" ";
    }

    public static string GenerateLinqPadDump(VoWorkItem item) =>
        item.Config.Customizations.HasFlag(Customizations.GenerateLinqPadToDump)
            ? """
              private global::System.Object ToDump() =>
              #if VOGEN_NO_VALIDATION 
                   _value!
              #else
                  _isInitialized ? _value! : "[not initialized]";
              #endif
              """
            : string.Empty;

    /// <summary>
    ///  If we're generating `IXmlSerializable`, then we can't specify that _value and _isInitialized are readonly
    /// as the 'ReadXml' method has to mutate these fields.
    /// </summary>
    /// <param name="workItem"></param>
    /// <returns></returns>
    public static string GetModifiersForValueAndIsInitializedFields(VoWorkItem workItem) => 
        workItem.Config.Conversions.HasFlag(Conversions.XmlSerializable) ? "" : "readonly";

    internal static string GetLegalFilenameForMarkerClass(INamedTypeSymbol markerClassSymbol, ConversionMarkerKind conversionKind)
    {
        var unsanitized = $"{markerClassSymbol.ToDisplayString()}.{conversionKind}.g.cs";
        string filename = Util.SanitizeToALegalFilename(unsanitized);

        return filename;
    }

    internal static string GetLegalFilenameForMarkerClass(INamedTypeSymbol markerClassSymbol, INamedTypeSymbol voSymbol, ConversionMarkerKind conversionKind)
    {
        var unsanitized = $"{markerClassSymbol.ToDisplayString()}.{voSymbol.ToDisplayString()}.{conversionKind}.g.cs";
        string filename = Util.SanitizeToALegalFilename(unsanitized);

        return filename;
    }

    /// <summary>
    /// Generates PolyType attribute if PolyType.TypeShapeAttribute is available in the compilation
    /// </summary>
    public static string GeneratePolyTypeAttributeIfAvailable(VogenKnownSymbols knownSymbols)
    {
        if (knownSymbols.TypeShapeAttribute is null)
        {
            return string.Empty;
        }

        return @"    [global::PolyType.TypeShapeAttribute(Marshaler = typeof(PolyTypeMarshaler))]";
    }

    /// <summary>
    /// Generates PolyType marshaler class if PolyType.TypeShapeAttribute is available in the compilation
    /// </summary>
    public static string GeneratePolyTypeMarshalerIfAvailable(VogenKnownSymbols knownSymbols, VoWorkItem item)
    {
        if (knownSymbols.TypeShapeAttribute is null)
        {
            return string.Empty;
        }

        var typeName = item.VoTypeName;
        var underlyingType = item.UnderlyingTypeFullName;

        return $@"
        #nullable disable
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public sealed class PolyTypeMarshaler : global::PolyType.IMarshaler<{typeName}, {underlyingType}>
        {{
            {underlyingType} global::PolyType.IMarshaler<{typeName}, {underlyingType}>.Marshal({typeName} value) => value.Value;
            {typeName} global::PolyType.IMarshaler<{typeName}, {underlyingType}>.Unmarshal({underlyingType} value) => From(value);
        }}
        #nullable restore";
    }

    public static string GenerateCoverageExcludeAndGeneratedCodeAttributes() =>
        $"""
         [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] 
         [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GenerateYourAssemblyName()}", "{GenerateYourAssemblyVersion()}")]
         """;
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

        string setter = "";
        if (item.LanguageVersion >= LanguageVersion.CSharp10) setter = " = null!";
            

        return $"""
                #if DEBUG   
                private readonly global::System.Diagnostics.StackTrace{item.Nullable.QuestionMarkForOtherReferences} _stackTrace {setter};
                #endif
                """;
    }

    public static string SetStackTraceIfNeeded(VoWorkItem voWorkItem) => voWorkItem.Config.DisableStackTraceRecordingInDebug
        ? ""
        : "_stackTrace = new global::System.Diagnostics.StackTrace();";

    /// <summary>
    /// For structs before C# 10, fields couldn't be initialised. To we have to initialise them manually in the constructor.
    /// </summary>
    /// <param name="voWorkItem"></param>
    /// <returns></returns>
    public static string SetStackTraceToNullIfNeeded(VoWorkItem voWorkItem) => voWorkItem.Config.DisableStackTraceRecordingInDebug
        ? ""
        : "_stackTrace = null;";
}