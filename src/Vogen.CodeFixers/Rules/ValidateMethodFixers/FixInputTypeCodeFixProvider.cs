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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FixInputTypeCodeFixProvider)), Shared]
    public class FixInputTypeCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => 
            // ReSharper disable once UseCollectionExpression
            ImmutableArray.Create(RuleIdentifiers.FixInputTypeOfValidationMethod);

        public sealed override FixAllProvider GetFixAllProvider() => null!;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            string? ulType = diagnostic.Properties["PrimitiveType"];
            
            if (ulType is null)
                return;

            // Find the type declaration identified by the diagnostic.
            SyntaxToken identifierToken = root!.FindToken(diagnosticSpan.Start);
            
            var declaration = identifierToken.Parent!.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            string title = "Fix input parameter";

            var codeAction = CodeAction.Create(
                title: title,
                createChangedDocument: ct => ApplyFix(context.Document, declaration, ulType, ct),
                equivalenceKey: title);
            
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> ApplyFix(Document document,
            MethodDeclarationSyntax methodSyntax,
            string primitiveType,
            CancellationToken cancellationToken)
        {
            if (methodSyntax.ParameterList.Parameters.Count != 1)
            {
                return document;
            }

            ParameterSyntax oldParameter = methodSyntax.ParameterList.Parameters[0];

            ParameterSyntax newParameter = oldParameter
                .WithType(SyntaxFactory.ParseTypeName(primitiveType))
                .WithIdentifier(SyntaxFactory.Identifier(oldParameter.Identifier.ToString()));
            
            SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            
            if (root is null)
            {
                return document;
            }

            SyntaxNode newRoot = root.ReplaceNode(oldParameter, newParameter);
            return document.WithSyntaxRoot(newRoot);            
        }
    }
}
