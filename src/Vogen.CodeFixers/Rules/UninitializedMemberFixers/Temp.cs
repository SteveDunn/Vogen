using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;

namespace Vogen.Rules.UninitializedMemberFixers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UninitializedMemberFixersFixers))]
[Shared]
public class UninitializedMemberFixersFixers : CodeFixProvider
{
    public const string MakeRequiredEquivalenceKey = "Voegen.Analyzers."+ RuleIdentifiers.DoNotUseUninitializedMembers + ".MakeRequired";
    public const string MakeNullableEquivalenceKey = "Voegen.Analyzers."+ RuleIdentifiers.DoNotUseUninitializedMembers + ".MakeNullable";

    public override ImmutableArray<string> FixableDiagnosticIds => [RuleIdentifiers.DoNotUseUninitializedMembers];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var memberDeclaration = FindMemberDeclaration(node);

            if (memberDeclaration is null)
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Mark member as 'required'",
                    createChangedDocument: ct => MakeRequiredAsync(context.Document, memberDeclaration, ct),
                    equivalenceKey: MakeRequiredEquivalenceKey),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Make member type nullable",
                    createChangedDocument: ct => MakeNullableAsync(context.Document, memberDeclaration, ct),
                    equivalenceKey: MakeNullableEquivalenceKey),
                diagnostic);
        }
    }

    private static MemberDeclarationSyntax? FindMemberDeclaration(SyntaxNode node)
    {
        var current = node;
        while (current is not null)
        {
            if (current is PropertyDeclarationSyntax or FieldDeclarationSyntax)
            {
                return (MemberDeclarationSyntax)current;
            }
            current = current.Parent;
        }
        return null;
    }

    private static Task<Document> MakeRequiredAsync(Document document, MemberDeclarationSyntax member, CancellationToken cancellationToken)
        => ReplaceMemberAsync(document, member, AddRequiredModifier(member), cancellationToken);

    private static MemberDeclarationSyntax AddRequiredModifier(MemberDeclarationSyntax member)
    {
        var modifiers = member.Modifiers;
        if (modifiers.Any(m => m.IsKind(SyntaxKind.RequiredKeyword)))
        {
            return member;
        }

        var requiredToken = SyntaxFactory.Token(SyntaxKind.RequiredKeyword)
            .WithTrailingTrivia(SyntaxFactory.Space);

        // Insert `required` after the access modifier(s) but before the rest, preserving existing leading trivia.
        var insertIndex = 0;
        for (var i = 0; i < modifiers.Count; i++)
        {
            var kind = modifiers[i].Kind();
            if (kind
                is SyntaxKind.PublicKeyword
                or SyntaxKind.PrivateKeyword
                or SyntaxKind.ProtectedKeyword
                or SyntaxKind.InternalKeyword
                or SyntaxKind.StaticKeyword)
            {
                insertIndex = i + 1;
            }
        }

        SyntaxTokenList newModifiers;
        if (modifiers.Count == 0)
        {
            // Preserve any leading trivia currently attached to the declaration's first token.
            var leadingTrivia = member.GetLeadingTrivia();
            requiredToken = requiredToken.WithLeadingTrivia(leadingTrivia);
            newModifiers = SyntaxFactory.TokenList(requiredToken);
            return member.WithLeadingTrivia(SyntaxFactory.TriviaList())
                .WithModifiers(newModifiers);
        }

        newModifiers = modifiers.Insert(insertIndex, requiredToken);
        return member.WithModifiers(newModifiers);
    }

    private static Task<Document> MakeNullableAsync(Document document, MemberDeclarationSyntax member, CancellationToken cancellationToken)
    {
        MemberDeclarationSyntax replacement;
        switch (member)
        {
            case PropertyDeclarationSyntax property:
                replacement = property.WithType(MakeNullableType(property.Type));
                break;
            case FieldDeclarationSyntax field:
                replacement = field.WithDeclaration(field.Declaration.WithType(MakeNullableType(field.Declaration.Type)));
                break;
            default:
                return Task.FromResult(document);
        }
        return ReplaceMemberAsync(document, member, replacement, cancellationToken);
    }

    private static TypeSyntax MakeNullableType(TypeSyntax type)
    {
        if (type is NullableTypeSyntax)
        {
            return type;
        }
        return SyntaxFactory.NullableType(type.WithoutTrailingTrivia()).WithTrailingTrivia(type.GetTrailingTrivia());
    }

    private static async Task<Document> ReplaceMemberAsync(
        Document document,
        MemberDeclarationSyntax oldMember,
        MemberDeclarationSyntax newMember,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }
        var newRoot = root.ReplaceNode(oldMember, newMember);
        return document.WithSyntaxRoot(newRoot);
    }
}