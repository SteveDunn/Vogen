using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Vogen.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CreationUsingNewAnalyzer2 : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CreationUsingNewAnalyzer2";

    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AddValidationAnalyzerTitle),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.AddValidationAnalyzerMessageFormat), Resources.ResourceManager,
            typeof(Resources));
    
    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.AddValidationAnalyzerDescription), Resources.ResourceManager,
            typeof(Resources));
    
    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
        Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get { return ImmutableArray.Create(Rule); }
    }

    public override void Initialize(AnalysisContext context)
    {

        //context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        
//        context.RegisterSymbolAction(OnSymbolAction, SymbolKind.NamedType);

        context.RegisterCompilationStartAction(compilationContext =>
        {
            compilationContext.RegisterOperationAction(AnalyzeExpression, OperationKind.ExpressionStatement);
        });
    }

    private void AnalyzeExpression(OperationAnalysisContext context)
    {
        var c = context.Operation as IObjectCreationOperation;
        if (c == null) return;
        var t = c.Type as INamedTypeSymbol;
        if (t == null) return;

       // if (!VoFilter.IsTarget(t)) return;

        context.ReportDiagnostic(Rule, context.Operation);
    }
}