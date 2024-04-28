using System.Collections.Immutable;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Vogen.Diagnostics;

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotDeriveFromVogenAttributesAnalyzer : DiagnosticAnalyzer
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it
    // won't find this and will report:
    // "error RS2002: Rule 'XYZ123' is part of the next unshipped analyzer release, but is not a supported diagnostic for any analyzer"
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.DoNotDeriveFromVogenAttributes,
        "Deriving from a Vogen attribute will be disallowed in a future release, use a type alias instead if you're using C# 12 or greater",
        "Type '{0}' should not derive from a Vogen attribute",
        RuleCategories.Usage,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description:
        "The value object is created with a new expression. This can lead to invalid value objects in your domain. Use the From method instead.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var symbol = (INamedTypeSymbol) context.Symbol;

        var a1 = context.Compilation.GetTypeByMetadataName("Vogen.ValueObjectAttribute");
        var a2 = context.Compilation.GetTypeByMetadataName("Vogen.VogenDefaultsAttribute");

        if (symbol.DerivesFrom(a1) || symbol.DerivesFrom(a2))
        {
            var diagnostic = Diagnostic.Create(_rule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}