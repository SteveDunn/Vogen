using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using Vogen.Diagnostics;

namespace Vogen.Rules.MakeStructReadonlyFixers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MakeStructReadonlyCodeFixProvider)), Shared]
public sealed class MakeStructReadonlyCodeFixProvider : CodeFixProvider
{
    private const string _title = "Make struct readonly";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(RuleIdentifiers.UseReadonlyStructInsteadOfStruct);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var typeDeclarationSyntax = root.FindToken(diagnosticSpan.Start)
            .Parent?
            .AncestorsAndSelf()
            .OfType<TypeDeclarationSyntax>()
            .FirstOrDefault(syntax => syntax is StructDeclarationSyntax or RecordDeclarationSyntax);

        if (typeDeclarationSyntax is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                _title,
                c => MakeStructReadonlyAsync(context.Document, typeDeclarationSyntax, c),
                _title),
            diagnostic);
    }

    private static async Task<Document> MakeStructReadonlyAsync(Document document, TypeDeclarationSyntax typeDeclarationSyntax, CancellationToken cancellationToken)
    {
        var readonlyModifier = SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword);

        var newModifiers = typeDeclarationSyntax.Modifiers;

        // Ensure that the readonly keyword is inserted before the partial keyword
        if (newModifiers.Any(SyntaxKind.PartialKeyword))
        {
            var partialIndex = newModifiers.IndexOf(SyntaxKind.PartialKeyword);
            newModifiers = newModifiers.Insert(partialIndex, readonlyModifier);
        }
        else
        {
            newModifiers = newModifiers.Add(readonlyModifier);
        }

        var newStructDeclaration = typeDeclarationSyntax.WithModifiers(newModifiers);

        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var newRoot = root!.ReplaceNode(typeDeclarationSyntax, newStructDeclaration);
        return document.WithSyntaxRoot(newRoot);
    }
}
