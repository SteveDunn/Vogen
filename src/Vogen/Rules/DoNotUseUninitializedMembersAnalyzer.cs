using Vogen.Diagnostics;
namespace Vogen.Rules;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

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

        context.RegisterSyntaxNodeAction(AnalyzeMemberNode, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeMemberNode, SyntaxKind.FieldDeclaration);
    }

    private static void AnalyzeMemberNode(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.ContainingSymbol is null)
        {
            return;
        }

        var memberSymbol = ctx.ContainingSymbol;
        if (!TryGetCandidate(memberSymbol, out var memberType))
        {
            return;
        }

        if (memberSymbol.ContainingSymbol is not INamedTypeSymbol containingType)
        {
            return;
        }

        var constructors = containingType.InstanceConstructors;

        var c = constructors
            .SelectMany(x => x.DeclaringSyntaxReferences)
            .Select(syntaxRef => syntaxRef.GetSyntax())
            .OfType<ConstructorDeclarationSyntax>()
            .Select(x => ctx.SemanticModel.GetOperation(x))
            .Where(x => x is not null)
            .ToArray();
        if (c.Length != 0)
        {

        }
        // There can be only one static constructor.
        var staticConstructor = containingType.StaticConstructors.FirstOrDefault();


        if (IsMemberSafelyInitialized(ctx.SemanticModel, memberSymbol, staticConstructor, constructors))
        {
            return;
        }

        var location = memberSymbol.Locations.FirstOrDefault() ?? Location.None;
        ctx.ReportDiagnostic(
            Diagnostic.Create(
                _rule,
                location,
                memberSymbol.Name,
                memberType.ToDisplayString()));
    }

    private static bool IsMemberSafelyInitialized(
        SemanticModel semanticModel,
        ISymbol member,
        IMethodSymbol? staticConstructor,
        ImmutableArray<IMethodSymbol> constructors)
    {
        if (member.IsStatic)
        {
            return staticConstructor != null
                   && ConstructorAssignsMember(
                       staticConstructor,
                       member,
                       semanticModel,
                       new ConcurrentDictionary<IMethodSymbol, bool>(SymbolEqualityComparer.Default));
        }

        if (constructors.IsDefaultOrEmpty)
        {
            return false;
        }

        var visitedConstructors = new ConcurrentDictionary<IMethodSymbol, bool>(SymbolEqualityComparer.Default);
        return constructors
            .All(ctor => ConstructorAssignsMember(ctor, member, semanticModel, visitedConstructors));
    }


    /// <summary>
    /// Tries to determine if the given member could be problematic.
    /// </summary>
    /// <param name="member"></param>
    /// <param name="memberType"></param>
    /// <returns></returns>
    // ReSharper disable once CognitiveComplexity Yes this is complex,
    // but it's necessary to handle all edge cases and splitting the method up further reduces readability
    private static bool TryGetCandidate(
        ISymbol member,
        out INamedTypeSymbol memberType)
    {
        memberType = null!;

        ITypeSymbol declaredType;
        switch (member)
        {
            case IPropertySymbol prop:
                if (!CheckIfPropertyNeedsHandling(prop, out declaredType))
                {
                    return false;
                }

                break;
            case IFieldSymbol field:
                if (!CheckIfFieldNeedsHandling(field, out declaredType))
                {
                    return false;
                }

                break;
            default:
                return false;
        }

        // unnamed types cannot be Vogen value objects.
        if (declaredType is not INamedTypeSymbol namedDeclaredType)
        {
            return false;
        }

        // Skip properties that are annotated as nullable
        if (namedDeclaredType.NullableAnnotation == NullableAnnotation.Annotated
            || namedDeclaredType.SpecialType == SpecialType.System_Nullable_T)
        {
            return false;
        }

        // Skip non-Vogen types
        if (!VoFilter.IsTarget(namedDeclaredType))
        {
            return false;
        }

        // Skip positional record properties (they are assigned by the synthesized primary constructor).
        if (HasParameterDeclaringSyntax(member))
        {
            return false;
        }

        // Skip members with an inline initializer.
        if (HasInlineInitializer(member))
        {
            return false;
        }

        memberType = namedDeclaredType;
        return true;
    }

    private static bool CheckIfFieldNeedsHandling(IFieldSymbol field, out ITypeSymbol outDeclaredType)
    {
        if (field.IsRequired)
        {
            outDeclaredType = null!;
            return false;
        }

        // const cannot fall back to the default value, skip them.
        // implicitly declared fields cannot be handled either, skip them.
        if (field.IsConst || field.IsImplicitlyDeclared)
        {
            outDeclaredType = null!;
            return false;
        }

        outDeclaredType = field.Type;
        return true;
    }

    private static bool CheckIfPropertyNeedsHandling(IPropertySymbol prop, out ITypeSymbol typeSymbol)
    {
        if (prop.IsRequired)
        {
            typeSymbol = null!;
            return false;
        }


        // Ignore indexer and abstract properties.
        if (prop.IsIndexer || prop.IsAbstract)
        {
            typeSymbol = null!;
            return false;
        }

        // Read-only get-only computed properties (no setter, no auto-property backing) cannot
        // accidentally surface a default value; skip them.
        if (prop.SetMethod is null && !IsAutoProperty(prop))
        {
            typeSymbol = null!;
            return false;
        }

        typeSymbol = prop.Type;
        return true;
    }


    private static bool IsAutoProperty(IPropertySymbol property)
    {
        foreach (var syntaxRef in property.DeclaringSyntaxReferences)
        {
            if (syntaxRef.GetSyntax() is not PropertyDeclarationSyntax decl)
            {
                continue;
            }

            if (decl.AccessorList is null)
            {
                continue;
            }

            var allAccessorsHaveNoBody =
                decl.AccessorList.Accessors.All(a => a.Body is null && a.ExpressionBody is null);
            if (allAccessorsHaveNoBody)
            {
                return true;
            }
        }

        return false;
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


    /// <summary>
    /// Checks if the method <paramref name="constructor"/> assigns the member <paramref name="member"/>.
    /// </summary>
    /// <param name="constructor">The method symbol of the constructor to check.</param>
    /// <param name="member">The member symbol to check for assignment.</param>
    /// <param name="semanticModel">The compilation context for symbol resolution.</param>
    /// <param name="visited">A cache to avoid redundant checks on the same constructor.</param>
    /// <returns>True if the constructor assigns the member, otherwise false.</returns>
    private static bool ConstructorAssignsMember(
        IMethodSymbol constructor,
        ISymbol member,
        SemanticModel semanticModel,
        ConcurrentDictionary<IMethodSymbol, bool> visited)
    {
        if (visited.TryGetValue(constructor, out var result))
        {
            return result;
        }

        visited.TryAdd(constructor, value: true);

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
        ConcurrentDictionary<IMethodSymbol, bool> visited,
        ConstructorDeclarationSyntax constructorSyntax)
    {
        var operation = semanticModel.GetOperation(constructorSyntax);
        var assignments = operation
            .Descendants()
            .OfType<IAssignmentOperation>();

        if (assignments.Any(assignmentOperation => OperationAssignsToMember(member, assignmentOperation)))
        {
            return true;
        }

        var thisConstructor = constructorSyntax.Initializer is { } initializer
                              && initializer.ThisOrBaseKeyword.IsKind(SyntaxKind.ThisKeyword)
            ? initializer
            : null;

        if (thisConstructor is null)
        {
            return false;
        }

        if (semanticModel.GetSymbolInfo(thisConstructor).Symbol is IMethodSymbol target
            && ConstructorAssignsMember(target, member, semanticModel, visited))
        {
            return true;
        }

        return false;
    }

    private static bool OperationAssignsToMember(ISymbol member, IAssignmentOperation assignmentOperation)
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

        if (IsManualBackingFieldFor(fieldSymbol, memberProperty))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks whether a given field is the backing field for property with a manual backing field.
    /// </summary>
    /// <param name="field"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    private static bool IsManualBackingFieldFor(IFieldSymbol field, IPropertySymbol property) =>
        property
            .DeclaringSyntaxReferences
            .Select(x => x.GetSyntax())
            .SelectMany(x => x.DescendantNodes())
            .OfType<IdentifierNameSyntax>()
            .Any(x => x.Identifier.Text == field.Name);
}