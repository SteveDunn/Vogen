using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using Vogen.Diagnostics;
// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferReadonlyStructAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.UseReadonlyStructInsteadOfStruct,
        "Use readonly struct instead of struct",
        "Type '{0}' should be a readonly struct",
        RuleCategories.Usage,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description:
        "The struct is not readonly. This can lead to invalid value objects in your domain. Use readonly struct instead.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.StructDeclaration, SyntaxKind.RecordStructDeclaration);
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeDeclaration)
        {
            return;
        }

        var symbol = context.SemanticModel.GetDeclaredSymbol(typeDeclaration);
        if (symbol is null)
        {
            return;
        }

        // readonly struct became available in C# 7.2
        var languageVersion = GetLanguageVersion(context);
        if (languageVersion < LanguageVersion.CSharp7_2)
        {
            return;
        }

        if (!VoFilter.IsTarget(symbol))
        {
            return;
        }
        
        if (symbol.IsReadOnly)
        {
            return;
        }

        var ixml = context.SemanticModel.Compilation.GetTypeByMetadataName("System.Xml.Serialization.IXmlSerializable");
        if (ixml is not null && symbol.AllInterfaces.Contains(ixml, SymbolEqualityComparer.Default))
        {
            return;
        }

        ReportDiagnostic(context, symbol);
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, INamedTypeSymbol symbol)
    {
        var diagnostic = Diagnostic.Create(_rule, symbol.Locations[0], symbol.Name);
        context.ReportDiagnostic(diagnostic);
    }

    private static LanguageVersion GetLanguageVersion(SyntaxNodeAnalysisContext context)
    {
        var compilation = context.SemanticModel.Compilation;
        var parseOptions = (CSharpParseOptions)compilation.SyntaxTrees.First().Options;
        return parseOptions.LanguageVersion;
    }
}