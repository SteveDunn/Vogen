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

        context.RegisterSymbolAction(
            AnalyzeNamedType,
            SymbolKind.NamedType);
    }




    private static void AnalyzeNamedType(
        SymbolAnalysisContext context)
    {
        if (context.FilterTree is not null && VoFilter.IsInCodeThatShouldNotBeAnalyzed(context.FilterTree))
        {
            return;
        }

        var typeSymbol = (INamedTypeSymbol) context.Symbol;

        if (typeSymbol.TypeKind != TypeKind.Class
            && typeSymbol.TypeKind != TypeKind.Struct)
        {
            return;
        }

        var constructors = typeSymbol.InstanceConstructors;
        var staticConstructor = typeSymbol.StaticConstructors.FirstOrDefault();

        foreach (var member in typeSymbol.GetMembers())
        {
            if (!TryGetCandidate(
                    member,
                    out var memberType))
            {
                continue;
            }

            if (IsMemberSafelyInitialized(context, member, staticConstructor, constructors))
            {
                continue;
            }

            var location = member.Locations.FirstOrDefault() ?? Location.None;
            context.ReportDiagnostic(
                Diagnostic.Create(
                    _rule,
                    location,
                    member.Name,
                    memberType.ToDisplayString()));
        }
    }

    private static bool IsMemberSafelyInitialized(
        SymbolAnalysisContext context,
        ISymbol member,
        IMethodSymbol? staticConstructor,
        ImmutableArray<IMethodSymbol> constructors)
    {
        if (constructors.IsDefaultOrEmpty)
        {
            return false;
        }

        if (member.IsStatic)
        {
            return staticConstructor != null
                   && ConstructorAssignsMember(
                       staticConstructor,
                       member,
                       context.Compilation,
                       new ConcurrentDictionary<IMethodSymbol, bool>(SymbolEqualityComparer.Default));
        }

        var visitedConstructors = new ConcurrentDictionary<IMethodSymbol, bool>(SymbolEqualityComparer.Default);
        return constructors
            .All(ctor => ConstructorAssignsMember(ctor, member, context.Compilation, visitedConstructors));
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


    private static bool ConstructorAssignsMember(
        IMethodSymbol constructor,
        ISymbol member,
        Compilation compilation,
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
            .DeclaringSyntaxReferences.Select(syntaxRef => syntaxRef.GetSyntax())
            .OfType<ConstructorDeclarationSyntax>()
            .Any(constructorSyntax =>
                CheckIfConstructorSyntaxAssignsToTheMember(member, compilation, visited, constructorSyntax));
    }

    private static bool CheckIfConstructorSyntaxAssignsToTheMember(
        ISymbol member,
        Compilation compilation,
        ConcurrentDictionary<IMethodSymbol, bool> visited,
        ConstructorDeclarationSyntax constructorSyntax)
    {
        // This is effectively a simple check if any members are assigned insidte the constructor
        // To prevent retrieving the Semantic Model
        // This may or may not be more efficient than just retrieving the semantic model
        var bodyNodes = constructorSyntax.Body?.DescendantNodes();
        bodyNodes ??= constructorSyntax.ExpressionBody?.DescendantNodes();
        var assignmentExpressionSyntaxNodes = bodyNodes
            ?
            .OfType<AssignmentExpressionSyntax>()
            .Where(x => x.IsKind(SyntaxKind.SimpleAssignmentExpression))
            .ToImmutableArray();

        var thisConstructor = constructorSyntax.Initializer is { } initializer
                              && initializer.ThisOrBaseKeyword.IsKind(SyntaxKind.ThisKeyword)
            ? initializer
            : null;


        // Check if the constructor assigns any members
        // If not and if it does not assign 'this' then it is not interesting
        if (assignmentExpressionSyntaxNodes is not { IsDefaultOrEmpty: false } && thisConstructor is null)
        {
            return false;
        }
        // -- Check for assignments ends here --

#pragma warning disable RS1030
        var semanticModel = compilation.GetSemanticModel(constructorSyntax.SyntaxTree);
#pragma warning restore RS1030
        var operation = semanticModel.GetOperation(constructorSyntax);
        var assignments = operation
            .Descendants()
            .OfType<IAssignmentOperation>();

        if (assignments.Any(assignmentOperation => OperationAssignsToMember(member, assignmentOperation)))
        {
            return true;
        }

        if (thisConstructor is null)
        {
            return false;
        }

        if (semanticModel.GetSymbolInfo(thisConstructor).Symbol is IMethodSymbol target
            && ConstructorAssignsMember(target, member, compilation, visited))
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