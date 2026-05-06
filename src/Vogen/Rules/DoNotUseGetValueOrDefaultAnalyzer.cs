using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Vogen.Diagnostics;

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseGetValueOrDefaultAnalyzer : DiagnosticAnalyzer
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it
    // won't find this and will report:
    // "error RS2002: Rule 'XYZ123' is part of the next unshipped analyzer release, but is not a supported diagnostic for any analyzer"
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.DoNotUseGetValueOrDefault,
        "Calling GetValueOrDefault on a nullable Value Object is prohibited",
        "Type '{0}' cannot be constructed with GetValueOrDefault() as it is prohibited",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "Calling GetValueOrDefault() on a nullable value object returns an uninitialized value object. Use the HasValue property and Value property instead, or pattern match.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(compilationContext =>
        {
            compilationContext.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
        });
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
        if (context.Operation is not IInvocationOperation invocation) return;

        if (invocation.TargetMethod.Name != "GetValueOrDefault") return;
        if (invocation.Arguments.Length != 0) return;

        var containingType = invocation.TargetMethod.ContainingType;
        if (containingType is not { IsGenericType: true }) return;
        if (containingType.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T) return;

        var typeArgument = containingType.TypeArguments[0] as INamedTypeSymbol;
        if (typeArgument is null) return;
        if (!VoFilter.IsTarget(typeArgument)) return;

        var diagnostic = DiagnosticsCatalogue.BuildDiagnostic(_rule, typeArgument.Name, context.Operation.Syntax.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}
