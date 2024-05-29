using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Vogen.Diagnostics;

namespace Vogen.Rules;

/// <summary>
/// An analyzer that stops implicit types in the attribute, e.g. `[ValueObject]`.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ExplicitlySpecifyTypeInValueObjectAttributeAnalyzer : DiagnosticAnalyzer
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it
    // won't find this and will report:
    // "error RS2002: Rule 'XYZ123' is part of the next unshipped analyzer release, but is not a supported diagnostic for any analyzer"
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.ExplicitlySpecifyTypeInValueObjectAttribute,
        "Explicitly specify the type of primitive being wrapped",
        "The type '{0}' should explicitly specify the type of primitive that it wraps",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "This rule checks that value objects explicitly specify the type of primitive that they wrap. Value objects that don't make this explicit can be harder to read and understand.");

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

        var attrs = VoFilter.TryGetValueObjectAttributes(symbol).ToList();

        if (attrs.Count != 1)
        {
            return;
        }

        AttributeData att = attrs[0];
        if (att.ConstructorArguments.Length == 0)
        {
            return;
        }

        if (!att.ConstructorArguments[0].IsNull)
        {
            return;
        }

        var v = ManageAttributes.GetDefaultConfigFromGlobalAttribute(context.Compilation);
        if (v.ResultingConfiguration is null)
        {
            return;
        }
        
        if (v.ResultingConfiguration.ExplicitlySpecifyTypeInValueObject)
        {
            var diagnostic = Diagnostic.Create(_rule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}