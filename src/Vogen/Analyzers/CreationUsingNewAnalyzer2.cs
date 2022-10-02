using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Vogen.Diagnostics;

namespace Vogen.Analyzers;

internal static class RuleIdentifiers
{
    public const string UsingNewProhibited = "VOG010";
}

internal static class RuleCategories
{
    public const string Design = "Design";
    public const string Naming = "Naming";
    public const string Style = "Style";
    public const string Usage = "Usage";
    public const string Performance = "Performance";
    public const string Security = "Security";
}


[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CreationUsingNewAnalyzer2 : DiagnosticAnalyzer
{
    //private static readonly DiagnosticDescriptor _rule = DiagnosticItems._usingNewProhibited;

    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        "VOG010",
        "Using new to create Value Objects is prohibited - use the From method for creation",
        "Type '{0}' cannot be constructed with 'new' as it is prohibited",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "The value object is created with a new expression. This can lead to invalid value objects in your domain. Use the From method instead.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get { return ImmutableArray.Create(_rule); }
    }

    public override void Initialize(AnalysisContext context)
    {

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        // context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        //        context.RegisterSymbolAction(OnSymbolAction, SymbolKind.NamedType);

        context.RegisterCompilationStartAction(compilationContext =>
        {
            compilationContext.RegisterOperationAction(AnalyzeExpression, OperationKind.ObjectCreation);
            //compilationContext.RegisterOperationAction(AnalyzeExpression, OperationKind.ExpressionStatement);
        });
    }

    private void AnalyzeExpression(OperationAnalysisContext context)
    {
        //var c2 = context.Operation as IObjectCreationOperation;
        
        
        var c = context.Operation as IObjectCreationOperation;
        if (c == null) return;
        var t = c.Type as INamedTypeSymbol;
        if (t == null) return;

        ImmutableArray<AttributeData> attributes = t.GetAttributes();

        if (attributes.Length == 0)
        {
            return;
        }

        AttributeData? attr = attributes.SingleOrDefault(a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");

        if (attr is null)
        {
            return;
        }
        

        var d = DiagnosticItems.BuildDiagnostic(_rule, t.Name, context.Operation.Syntax.GetLocation());

        context.ReportDiagnostic(d);
    }
}