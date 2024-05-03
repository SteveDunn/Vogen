using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Vogen.Diagnostics;

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseNewAnalyzer : DiagnosticAnalyzer
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it
    // won't find this and will report:
    // "error RS2002: Rule 'XYZ123' is part of the next unshipped analyzer release, but is not a supported diagnostic for any analyzer"
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.DoNotUseNew,
        "Using new to create Value Objects is prohibited - use the From method for creation",
        "Type '{0}' cannot be constructed with 'new' as it is prohibited",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "The value object is created with a new expression. This can lead to invalid value objects in your domain. Use the From method instead.");

    private static readonly DiagnosticDescriptor _rule2 = new DiagnosticDescriptor(
        RuleIdentifiers.IncorrectUseOfInstanceField,
        "Instance fields should be declared as public and static",
        "Type '{0}' cannot be constructed as a field with 'new' as it should be public and static",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "The value object is created with a new expression. This can lead to invalid value objects in your domain. Use the From method instead.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
        ImmutableArray.Create(_rule, _rule2);

    public override void Initialize(AnalysisContext context)
    {

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            compilationContext.RegisterOperationAction(AnalyzeExpression, OperationKind.ObjectCreation);
        });
    }

    private static void AnalyzeExpression(OperationAnalysisContext context)
    {
        if (context.Operation is not IObjectCreationOperation c) return;

        if (c.Type is not INamedTypeSymbol symbol) return;

        if (!VoFilter.IsTarget(symbol)) return;

        var instanceFieldState = IsAPublicStaticFieldInAValueObject();
        
        if (instanceFieldState == InstanceField.NotValid)
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.BuildDiagnostic(_rule2, symbol.Name, context.Operation.Syntax.GetLocation()));

            return;
        }
        
        if (instanceFieldState == InstanceField.Valid) return;

        var diagnostic = DiagnosticsCatalogue.BuildDiagnostic(_rule, symbol.Name, context.Operation.Syntax.GetLocation());

        context.ReportDiagnostic(diagnostic);

        return;

        InstanceField IsAPublicStaticFieldInAValueObject()
        {
            var cs = context.ContainingSymbol as IFieldSymbol;
            if (cs is null) return InstanceField.NotApplicable;
            var type = cs.ContainingType;
            var isVo = VoFilter.IsTarget(type);
            if (!isVo)
            {
                return InstanceField.NotApplicable;
            }
            
            if (cs.DeclaredAccessibility != Accessibility.Public) return InstanceField.NotValid;
            if (!cs.IsStatic) return InstanceField.NotValid;

            return InstanceField.Valid;
        }
        
    }

    internal enum InstanceField
    {
        Valid,
        NotValid,
        NotApplicable
                
    }
}