using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;
using Document = Microsoft.CodeAnalysis.Document;

namespace Vogen.Rules.ValidateMethodFixers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddStaticToExistingMethodCodeFixProvider)), Shared]
    public class AddStaticToExistingMethodCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => 
            ImmutableArray.Create(RuleIdentifiers.AddStaticToExistingValidationMethod);

        public sealed override FixAllProvider GetFixAllProvider() => null!;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            SyntaxToken identifierToken = root!.FindToken(diagnosticSpan.Start);
            
            var declaration = identifierToken.Parent!.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            string title = "Make method static";

            var codeAction = CodeAction.Create(
                title: title,
                createChangedDocument: c => MakeMethodStatic(context.Document, declaration, c),
                // createChangedSolution: c => GenerateValidationMethodAsync(context.Document, declaration, c),
                equivalenceKey: title);
            
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> MakeMethodStatic(Document document,
            MethodDeclarationSyntax methodSyntax,
            CancellationToken cancellationToken)
        {
            MethodDeclarationSyntax modifiedMethod = methodSyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));

            SyntaxNode root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;
            SyntaxNode newRoot = root.ReplaceNode(methodSyntax, modifiedMethod);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
