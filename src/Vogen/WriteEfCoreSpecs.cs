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
    public static void WriteIfNeeded(SourceProductionContext context, Compilation compilation, ImmutableArray<EfCoreConverterSpecResult> convertSpecs)
    {
        if (compilation is not CSharpCompilation { LanguageVersion: >= LanguageVersion.CSharp12 })
        {
            return;
        }
        
        foreach (var eachSpec in convertSpecs)
        {
            WriteEachIfNeeded(context, eachSpec);
        }
    }

    private static void WriteEachIfNeeded(SourceProductionContext context, EfCoreConverterSpecResult specResult)
    {
        var spec = specResult.Spec;
        
        if (spec is null)
        {
            return;
        }

        var body = GenerateEfCoreTypeConversions.GenerateAnyBody2(spec.UnderlyingType, spec.VoSymbol.IsValueType, spec.VoSymbol);
        string sb =
$$"""
#if NET8_OR_GREATER

{{GeneratedCodeSegments.Preamble}}

namespace {{spec.SourceType.FullNamespace()}};
    
public partial class {{spec.SourceType.Name}}
{
    {{body}}
}

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