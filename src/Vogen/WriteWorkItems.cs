// The symbol 'Environment' is banned for use by analyzers: see https://github.com/dotnet/roslyn-analyzers/issues/6467 
#pragma warning disable RS1035 

using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vogen.Generators;

namespace Vogen;

internal static class WriteWorkItems
{
    private static readonly ClassGenerator _classGenerator;
    private static readonly RecordClassGenerator _recordClassGenerator;
    private static readonly RecordStructGenerator _recordStructGenerator;
    private static readonly StructGenerator _structGenerator;

    static WriteWorkItems()
    {
        _classGenerator = new ClassGenerator();
        _recordClassGenerator = new RecordClassGenerator();
        _recordStructGenerator = new RecordStructGenerator();
        _structGenerator = new StructGenerator();
    }

    public static void WriteVo(GenerationParameters parameters)
    {
        var item = parameters.WorkItem;
        var context = parameters.Context;
        TypeDeclarationSyntax voClass = item.TypeToAugment;

        IGenerateValueObjectSourceCode generator = GetGenerator(item);

        string classAsText = GeneratedCodeSegments.Preamble + Environment.NewLine + generator.Generate(parameters);
        SourceText sourceText = SourceText.From(classAsText, Encoding.UTF8);        
        var unsanitized = $"ff{item.FullNamespace}_{voClass.Identifier}.g.cs";

        string filename = Util.SanitizeToALegalFilename(unsanitized);

        Util.TryWriteUsingUniqueFilename(filename, context, sourceText);
    }

    private static IGenerateValueObjectSourceCode GetGenerator(VoWorkItem item) =>
        item.TypeToAugment switch
        {
            ClassDeclarationSyntax => _classGenerator,
            StructDeclarationSyntax => _structGenerator,
            RecordDeclarationSyntax rds when rds.IsKind(SyntaxKind.RecordDeclaration) => _recordClassGenerator,
            RecordDeclarationSyntax rds when rds.IsKind(SyntaxKind.RecordStructDeclaration) => _recordStructGenerator,
            _ => throw new InvalidOperationException("Don't know how to get the generator!")
        };
}