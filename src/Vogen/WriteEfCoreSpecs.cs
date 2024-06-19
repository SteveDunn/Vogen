using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Vogen.Generators.Conversions;

namespace Vogen;

internal class WriteEfCoreSpecs
{
    public static void WriteIfNeeded(SourceProductionContext context, Compilation compilation, ImmutableArray<EfCoreConverterSpecResults> convertSpecs)
    {
        if (compilation is not CSharpCompilation { LanguageVersion: >= LanguageVersion.CSharp12 })
        {
            return;
        }
        
        foreach (EfCoreConverterSpecResults? eachSpecs in convertSpecs)
        {
            WriteEncompassingExtensionMethod(eachSpecs, context);
            
            foreach (var eachSpec in eachSpecs.Specs)
            {
                WriteEachIfNeeded(context, eachSpec);
            }
        }
    }

    private static void WriteEncompassingExtensionMethod(EfCoreConverterSpecResults resultsParticularMethod, SourceProductionContext context)
    {
        var sourceSymbol = resultsParticularMethod.SourceSymbol;
        if (sourceSymbol is null)
        {
            return;
        }
        
        var fullNamespace = sourceSymbol.FullNamespace();

        var isPublic = sourceSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        var ns = string.IsNullOrEmpty(fullNamespace) ? string.Empty : $"namespace {fullNamespace};";

        string allCalls = GenerateBody();

        string sb =
            $$"""
              #if NET8_0_OR_GREATER

              {{GeneratedCodeSegments.Preamble}}

              {{ns}}
                  
                {{accessor}} static class {{sourceSymbol.Name}}__Ext
                {
                    {{accessor}} static global::Microsoft.EntityFrameworkCore.ModelConfigurationBuilder RegisterAllIn{{sourceSymbol.Name}}(this global::Microsoft.EntityFrameworkCore.ModelConfigurationBuilder configurationBuilder)
                    {
                      {{allCalls}}
        
                      return configurationBuilder; 
                    }
                }

              #endif
              """;

        SourceText sourceText = SourceText.From(sb, Encoding.UTF8);

        var unsanitized = $"{sourceSymbol.ToDisplayString()}.g.cs";

        string filename = sanitizeToALegalFilename(unsanitized);

        tryWriteUsingUniqueFilename();

        static string sanitizeToALegalFilename(string input)
        {
            return input.Replace('@', '_');
        }

        void tryWriteUsingUniqueFilename()
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
                catch(ArgumentException)
                {
                    if (++count >= 10)
                    {
                        throw;
                    }

                    hintName = $"{count}{filename}";
                }
            }
        }

        string GenerateBody()
        {
            StringBuilder sb2 = new StringBuilder();

            foreach (EfCoreConverterSpecResult eachSpec in resultsParticularMethod.Specs)
            {
                if (eachSpec.Spec is null)
                {
                    continue;
                }

                var voSymbol = eachSpec.Spec.VoSymbol;
                sb2.AppendLine($$"""configurationBuilder.Properties<{{voSymbol.FullName()}}>().HaveConversion<{{resultsParticularMethod.SourceSymbol.FullName()}}.{{voSymbol.Name}}EfCoreValueConverter, {{resultsParticularMethod.SourceSymbol.FullName()}}.{{voSymbol.Name}}EfCoreValueComparer>();""");
            }

            return sb2.ToString();
        }
    }

    private static void WriteEachIfNeeded(SourceProductionContext context, EfCoreConverterSpecResult specResult)
    {
        var spec = specResult.Spec;
        
        if (spec is null)
        {
            return;
        }

        var body = GenerateEfCoreTypes.GenerateOuter(spec.UnderlyingType, spec.VoSymbol.IsValueType, spec.VoSymbol);
        var extensionMethod = GenerateEfCoreTypes.GenerateOuterExtensionMethod(spec);

        var fullNamespace = spec.SourceType.FullNamespace();

        var isPublic = spec.SourceType.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        var ns = string.IsNullOrEmpty(fullNamespace) ? string.Empty : $"namespace {fullNamespace};";
        
        
        string sb =
$$"""
#if NET8_0_OR_GREATER

{{GeneratedCodeSegments.Preamble}}

{{ns}}
    
{{accessor}} partial class {{spec.SourceType.Name}}
{
    {{body}}
}

{{extensionMethod}}

#endif
""";
        
        SourceText sourceText = SourceText.From(sb, Encoding.UTF8);

        var unsanitized = $"{spec.SourceType.ToDisplayString()}_{spec.VoSymbol.ToDisplayString()}.g.cs";
        string filename = sanitizeToALegalFilename(unsanitized);

        tryWriteUsingUniqueFilename();

        static string sanitizeToALegalFilename(string input)
        {
            return input.Replace('@', '_');
        }
        
        void tryWriteUsingUniqueFilename()
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
                catch(ArgumentException)
                {
                    if (++count >= 10)
                    {
                        throw;
                    }

                    hintName = $"{count}{filename}";
                }
            }
        }

    }
}