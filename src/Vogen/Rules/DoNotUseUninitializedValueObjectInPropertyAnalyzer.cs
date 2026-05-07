using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Vogen.Diagnostics;

namespace Vogen.Rules;

/// <summary>
/// An analyzer that warns when a Value Object type is used as a non-nullable, non-required property
/// without an initializer, which can lead to uninitialized Value Object instances at runtime.
/// </summary>
/// <remarks>
/// <para>
/// When a class or struct declares a property whose type is a Vogen Value Object, and that property
/// is not nullable, not marked <c>required</c>, and has no initializer, then constructing the
/// containing type without explicitly setting the property will produce an invalid (uninitialized)
/// Value Object. This commonly occurs with EF Core entities and other data-transfer types.
/// </para>
/// <para>
/// The following are acceptable patterns that suppress this diagnostic:
/// <list type="bullet">
///   <item>Make the property nullable: <c>public MyVo? Prop { get; set; }</c></item>
///   <item>Add the <c>required</c> modifier: <c>public required MyVo Prop { get; set; }</c></item>
///   <item>Provide an initializer: <c>public MyVo Prop { get; set; } = MyVo.From(0);</c></item>
///   <item>Use a getter-only property (must be set via the constructor).</item>
/// </list>
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseUninitializedValueObjectInPropertyAnalyzer : DiagnosticAnalyzer
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it
    // won't find this and will report:
    // "error RS2002: Rule 'XYZ123' is part of the next unshipped analyzer release, but is not a supported diagnostic for any analyzer"
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.PropertyOfValueObjectShouldBeInitialized,
        "Value Object property should be initialized",
        "Property of Value Object type '{0}' is not nullable, not required, and has no initializer - this may result in an uninitialized Value Object at runtime",
        RuleCategories.Usage,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description:
        "A property whose type is a Value Object is not nullable, not marked 'required', and has no initializer. " +
        "Creating an instance of the containing type without explicitly setting this property will produce an " +
        "invalid (uninitialized) Value Object. Make the property nullable, add the 'required' modifier, or " +
        "provide an initializer.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        if (VoFilter.IsInCodeThatShouldNotBeAnalyzed(context.Node)) return;

        var propertyDeclaration = (PropertyDeclarationSyntax) context.Node;

        // Properties with an explicit initializer are fine.
        if (propertyDeclaration.Initializer is not null) return;

        // Expression-bodied properties compute their value; they can't be uninitialized.
        if (propertyDeclaration.ExpressionBody is not null) return;

        // Properties with no accessor list are unusual; skip them.
        if (propertyDeclaration.AccessorList is null) return;

        // Getter-only auto-properties *must* be set via a constructor — the compiler enforces this.
        // Only warn about properties that callers can leave un-set.
        bool hasSetterOrInit = false;
        foreach (var accessor in propertyDeclaration.AccessorList.Accessors)
        {
            if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration) ||
                accessor.IsKind(SyntaxKind.InitAccessorDeclaration))
            {
                hasSetterOrInit = true;
                break;
            }
        }

        if (!hasSetterOrInit) return;

        // Static properties are populated explicitly; skip them.
        if (propertyDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword)) return;

        // Properties with the 'required' modifier must be set by the caller.
        if (propertyDeclaration.Modifiers.Any(SyntaxKind.RequiredKeyword)) return;

        // Nullable properties (e.g. MyVo?) are intentionally nullable.
        if (propertyDeclaration.Type is NullableTypeSyntax) return;

        // Resolve the property type.
        var typeInfo = context.SemanticModel.GetTypeInfo(propertyDeclaration.Type);
        if (typeInfo.Type is not INamedTypeSymbol namedType) return;

        // Double-check nullability via the semantic model (handles #nullable contexts).
        if (typeInfo.Nullability.Annotation == NullableAnnotation.Annotated) return;

        // Skip if the *containing* type is itself a Value Object — avoid flagging VO internals.
        var containingType = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration)?.ContainingType;
        if (containingType is not null && VoFilter.IsTarget(containingType)) return;

        // Only flag properties whose type is a Vogen Value Object.
        if (!VoFilter.IsTarget(namedType)) return;

        context.ReportDiagnostic(
            DiagnosticsCatalogue.BuildDiagnostic(_rule, namedType.Name, propertyDeclaration.GetLocation()));
    }
}
