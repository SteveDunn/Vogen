using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Vogen.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseUninitializedMembersAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor _rule = new(
        RuleIdentifiers.DoNotUseUninitializedMembers,
        "Value Object property should be initialized",
        "Property of Value Object type '{0}' is not nullable, not required, and has no initializer - this may result in an uninitialized Value Object at runtime",
        RuleCategories.Usage,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        "A property whose type is a Value Object is not nullable, not marked 'required', and has no initializer. " +
        "Creating an instance of the containing type without explicitly setting this property will produce an " +
        "invalid (uninitialized) Value Object. Make the property nullable, add the 'required' modifier, or " +
        "provide an initializer.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var vogenTargetCache = new ConcurrentDictionary<INamedTypeSymbol, bool>(SymbolEqualityComparer.Default);

        context.RegisterSyntaxNodeAction(x => AnalyzePropertyNode(x, vogenTargetCache), SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(x => AnalyzeFieldNode(x, vogenTargetCache), SyntaxKind.FieldDeclaration);
    }

    private static void AnalyzePropertyNode(
        SyntaxNodeAnalysisContext ctx,
        ConcurrentDictionary<INamedTypeSymbol, bool> vogenTargetCache)
    {
        if (ctx.ContainingSymbol is not IPropertySymbol property)
        {
            throw new InvalidOperationException("Expected a property node, but got: " + ctx.Node.Kind());
        }

        var propertyDeclarationSyntax = (PropertyDeclarationSyntax) ctx.Node;


        if (!CheckIfPropertyNeedsHandling(property, propertyDeclarationSyntax))
        {
            return;
        }

        if (property.Type is not INamedTypeSymbol namedMemberTypeSymbol)
        {
            return;
        }

        AnalyzeMemberNode(ctx, property, namedMemberTypeSymbol, vogenTargetCache);
    }

    private static void AnalyzeFieldNode(
        SyntaxNodeAnalysisContext ctx,
        ConcurrentDictionary<INamedTypeSymbol, bool> vogenTargetCache)
    {
        if (ctx.ContainingSymbol is not IFieldSymbol field)
        {
            throw new InvalidOperationException("Expected a property node, but got: " + ctx.Node.Kind());
        }

        var fieldDeclarationSyntax = (FieldDeclarationSyntax) ctx.Node;

        if (!CheckIfFieldNeedsHandling(field, fieldDeclarationSyntax))
        {
            return;
        }

        if (field.Type is not INamedTypeSymbol namedMemberTypeSymbol)
        {
            return;
        }

        AnalyzeMemberNode(ctx, field, namedMemberTypeSymbol, vogenTargetCache);
    }

    private static void AnalyzeMemberNode(
        SyntaxNodeAnalysisContext ctx,
        ISymbol memberSymbol,
        INamedTypeSymbol memberTypeSymbol,
        ConcurrentDictionary<INamedTypeSymbol, bool> vogenTargetCache)
    {
        if (memberSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == nameof(MayBeUninitializedAttribute)))
        {
            return;
        }

        if (!CheckIfTypeNeedsHandling(memberTypeSymbol, memberSymbol, vogenTargetCache))
        {
            return;
        }

        // Get the class/struct this property is contained in
        if (memberSymbol.ContainingSymbol is not INamedTypeSymbol containingType)
        {
            return;
        }

        var constructors = containingType.InstanceConstructors;
        var staticConstructors = containingType.StaticConstructors;

        if (IsMemberSafelyInitialized(ctx.SemanticModel, memberSymbol, staticConstructors, constructors))
        {
            return;
        }

        var location = memberSymbol.Locations.FirstOrDefault() ?? Location.None;
        ctx.ReportDiagnostic(
            Diagnostic.Create(
                _rule,
                location,
                memberSymbol.Name,
                memberTypeSymbol.ToDisplayString()));
    }

    private static bool IsMemberSafelyInitialized(
        SemanticModel semanticModel,
        ISymbol member,
        ImmutableArray<IMethodSymbol> staticConstructors,
        ImmutableArray<IMethodSymbol> constructors)
    {
        if (member.IsStatic)
        {
            if (staticConstructors.IsDefaultOrEmpty)
            {
                return false;
            }

            var constructorCache = new Dictionary<IMethodSymbol, bool>(SymbolEqualityComparer.Default);
            return staticConstructors
                .All(ctor => ConstructorAssignsMember(ctor, member, semanticModel, constructorCache));
        }

        if (constructors.IsDefaultOrEmpty)
        {
            return false;
        }

        var visitedConstructors = new Dictionary<IMethodSymbol, bool>(SymbolEqualityComparer.Default);
        return constructors
            .All(ctor => ConstructorAssignsMember(ctor, member, semanticModel, visitedConstructors));
    }


    /// <summary>
    /// Tries to determine if the given member could be problematic.
    /// </summary>
    /// <param name="namedDeclaredType">The type of the member.</param>
    /// <param name="memberSymbol">The symbol of the member (field or property).</param>
    /// <param name="vogenTargetCache"></param>
    /// <returns>True if the type and member need handling, otherwise false.</returns>
    private static bool CheckIfTypeNeedsHandling(
        INamedTypeSymbol namedDeclaredType,
        ISymbol memberSymbol,
        ConcurrentDictionary<INamedTypeSymbol, bool> vogenTargetCache)
    {
        var isVoTarget = vogenTargetCache.GetOrAdd(namedDeclaredType, VoFilter.IsTarget);

        // Skip non-Vogen types
        if (!isVoTarget)
        {
            return false;
        }

        // Skip properties that are annotated as nullable
        if (namedDeclaredType.NullableAnnotation == NullableAnnotation.Annotated
            || namedDeclaredType.SpecialType == SpecialType.System_Nullable_T)
        {
            return false;
        }

        // Skip positional record properties (they are assigned by the synthesized primary constructor).
        if (HasParameterDeclaringSyntax(memberSymbol))
        {
            return false;
        }

        // Skip members with an inline initializer.
        if (HasInlineInitializer(memberSymbol))
        {
            return false;
        }

        return true;
    }

    private static bool CheckIfFieldNeedsHandling(IFieldSymbol field, FieldDeclarationSyntax fieldDeclarationSyntax)
    {
        if (field.IsRequired)
        {
            return false;
        }

        // Check for inline initializer (private MyVo _field = new MyVo();)
        foreach (var variableDeclaratorSyntax in fieldDeclarationSyntax.Declaration.Variables)
        {
            if (variableDeclaratorSyntax.Initializer is not null)
            {
                return false;
            }
        }

        // const cannot fall back to the default value, skip them.
        // implicitly declared fields cannot be handled either, skip them.
        if (field.IsConst || field.IsImplicitlyDeclared)
        {
            return false;
        }

        return true;
    }

    private static bool CheckIfPropertyNeedsHandling(
        IPropertySymbol prop,
        PropertyDeclarationSyntax propertyDeclarationSyntax)
    {
        if (prop.IsRequired)
        {
            return false;
        }

        // Ignore indexer and abstract properties.
        if (prop.IsIndexer || prop.IsAbstract)
        {
            return false;
        }

        // Read-only get-only computed properties (no setter, no auto-property backing) cannot
        // accidentally surface a default value; skip them.
        if (prop.SetMethod is null)
        {
            return false;
        }

        if (propertyDeclarationSyntax.Initializer is not null)
        {
            return false;
        }


        return true;
    }


    private static bool HasParameterDeclaringSyntax(ISymbol member) =>
        member
            .DeclaringSyntaxReferences
            .Select(x => x.GetSyntax())
            .OfType<ParameterSyntax>()
            .Any();

    private static bool HasInlineInitializer(ISymbol member)
    {
        foreach (var syntaxRef in member.DeclaringSyntaxReferences)
        {
            var syntax = syntaxRef.GetSyntax();
            switch (syntax)
            {
                case PropertyDeclarationSyntax { Initializer: not null }:
                case VariableDeclaratorSyntax { Initializer: not null }:
                    return true;
            }
        }

        return false;
    }

    private static bool ConstructorAssignsMember(
        IMethodSymbol constructor,
        ISymbol member,
        SemanticModel semanticModel,
        Dictionary<IMethodSymbol, bool> visited)
    {
        if (visited.TryGetValue(constructor, out var cachedResult))
        {
            return cachedResult;
        }

        var result = ConstructorAssignsMemberCore(constructor, member, semanticModel, visited);
        visited[constructor] = result;
        return result;
    }

    /// <summary>
    /// Checks if the method <paramref name="constructor"/> assigns the member <paramref name="member"/>.
    /// </summary>
    /// <param name="constructor">The method symbol of the constructor to check.</param>
    /// <param name="member">The member symbol to check for assignment.</param>
    /// <param name="semanticModel">The compilation context for symbol resolution.</param>
    /// <param name="visited">A cache to avoid redundant checks on the same constructor.</param>
    /// <returns>True if the constructor assigns the member, otherwise false.</returns>
    private static bool ConstructorAssignsMemberCore(
        IMethodSymbol constructor,
        ISymbol member,
        SemanticModel semanticModel,
        Dictionary<IMethodSymbol, bool> visited)
    {
        // This is an escape hatch for constructors that set required members using helper methods or in unusual ways.
        if (constructor
            .GetAttributes()
            .Any(x => x.AttributeClass?.MetadataName == nameof(SetsUninitializedMembersAttribute)))
        {
            return true;
        }

        // Implicit constructors have no syntax and assign nothing of interest.
        if (constructor.DeclaringSyntaxReferences.IsDefaultOrEmpty)
        {
            return false;
        }

        return constructor
            .DeclaringSyntaxReferences
            .Select(syntaxRef => syntaxRef.GetSyntax())
            .OfType<ConstructorDeclarationSyntax>()
            .Any(constructorSyntax =>
                CheckIfConstructorSyntaxAssignsToTheMember(member, semanticModel, visited, constructorSyntax));
    }

    private static bool CheckIfConstructorSyntaxAssignsToTheMember(
        ISymbol member,
        SemanticModel semanticModel,
        Dictionary<IMethodSymbol, bool> visited,
        ConstructorDeclarationSyntax constructorSyntax)
    {
        var operation = semanticModel.GetOperation(constructorSyntax);
        var assignments = operation
            .Descendants()
            .OfType<IAssignmentOperation>();

        if (assignments.Any(assignmentOperation =>
                OperationAssignsToMember(member, assignmentOperation, semanticModel)))
        {
            return true;
        }

        var nextConstructor = constructorSyntax.Initializer is { } initializer
                              && (initializer.ThisOrBaseKeyword.IsKind(SyntaxKind.ThisKeyword)
                                  || initializer.ThisOrBaseKeyword.IsKind(SyntaxKind.BaseKeyword))
            ? initializer
            : null;

        if (nextConstructor is not null && semanticModel.GetSymbolInfo(nextConstructor).Symbol is IMethodSymbol target
                                        && ConstructorAssignsMember(target, member, semanticModel, visited))
        {
            return true;
        }

        return false;
    }

    private static bool OperationAssignsToMember(
        ISymbol member,
        IAssignmentOperation assignmentOperation,
        SemanticModel semanticModel)
    {
        ISymbol? targetSymbol = assignmentOperation.Target switch
        {
            IPropertyReferenceOperation propRef => propRef.Property,
            IFieldReferenceOperation fieldRef => fieldRef.Field,
            _ => null
        };

        if (targetSymbol is null)
        {
            return false;
        }

        if (SymbolEqualityComparer.Default.Equals(targetSymbol, member))
        {
            return true;
        }

        if (targetSymbol is not IFieldSymbol fieldSymbol || member is not IPropertySymbol memberProperty)
        {
            return false;
        }

        // Check for assignments to the backing field of auto properties.
        if (fieldSymbol.AssociatedSymbol is IPropertySymbol propertySymbol
            && SymbolEqualityComparer.Default.Equals(propertySymbol, member))
        {
            return true;
        }

        if (IsManualBackingFieldFor(semanticModel, fieldSymbol, memberProperty))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks whether a given field is the backing field for property with a manual backing field.
    /// </summary>
    /// <param name="semanticModel"></param>
    /// <param name="field"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    private static bool IsManualBackingFieldFor(
        SemanticModel semanticModel,
        IFieldSymbol field,
        IPropertySymbol property) =>
        property
            .DeclaringSyntaxReferences
            .Select(x => x.GetSyntax())
            .SelectMany(x => x.DescendantNodes())
            .OfType<IdentifierNameSyntax>()
            .Any(x => semanticModel.GetSymbolInfo(x).Symbol is IFieldSymbol backingField &&
                      SymbolEqualityComparer.Default.Equals(backingField, field));
}