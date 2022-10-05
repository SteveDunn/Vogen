using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Vogen.Diagnostics;

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseNewAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.DoNotUseNew,
        "Using new to create Value Objects is prohibited - use the From method for creation",
        "Type '{0}' cannot be constructed with 'new' as it is prohibited",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "The value object is created with a new expression. This can lead to invalid value objects in your domain. Use the From method instead.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            compilationContext.RegisterOperationAction(AnalyzeExpression, OperationKind.ObjectCreation);
        });
    }

    private void AnalyzeExpression(OperationAnalysisContext context)
    {
        if (context.Operation is not IObjectCreationOperation c) return;

        if (c.Type is not INamedTypeSymbol symbol) return;

        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();

        if (attributes.Length == 0) return;

        AttributeData? attr = attributes.SingleOrDefault(a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");

        if (attr is null) return;
        
        var diagnostic = DiagnosticItems.BuildDiagnostic(_rule, symbol.Name, context.Operation.Syntax.GetLocation());

        context.ReportDiagnostic(diagnostic);
    }
}