using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using Vogen.Diagnostics;
using Document = Microsoft.CodeAnalysis.Document;
using Formatter = Microsoft.CodeAnalysis.Formatting.Formatter;

namespace Vogen.Rules.NormalizeInputMethodFixers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddMethodCodeFixProvider)), Shared]
    public class AddMethodCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(RuleIdentifiers.AddNormalizeInputMethod); }
        }

        public sealed override FixAllProvider GetFixAllProvider() => null!;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var syntaxToken = root!.FindToken(diagnosticSpan.Start);
            
            var declaration = syntaxToken.Parent!.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            string title = "Add normalize method";

            var codeAction = CodeAction.Create(
                title: title,
                createChangedDocument: c => GenerateMethodAsync(context.Document, context.Diagnostics, declaration, c),
                equivalenceKey: title);
            
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> GenerateMethodAsync(Document document,
            ImmutableArray<Diagnostic> contextDiagnostics,
            TypeDeclarationSyntax typeDecl,
            CancellationToken cancellationToken)
        {
            var properties = contextDiagnostics[0].Properties;

            string returnType = properties["PrimitiveType"]!;

            SyntaxNode root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;

            var newMember = GenerateMethod(returnType);

            var newTypeDecl = typeDecl.AddMembers(newMember);

            var newRoot = root.ReplaceNode(typeDecl, newTypeDecl);

            return document.WithSyntaxRoot(newRoot);
        }

        private static MethodDeclarationSyntax GenerateMethod(string primitiveType) =>
            (MethodDeclarationSyntax) ParseMember(
                $$"""
private static {{primitiveType}} NormalizeInput({{primitiveType}} input)
{
    // todo: normalize (sanitize) your input;
    return input;
}
""").WithAdditionalAnnotations(Simplifier.Annotation).NormalizeWhitespace();

        private static MemberDeclarationSyntax ParseMember(string member)
        {
            MemberDeclarationSyntax decl = ((ClassDeclarationSyntax) SyntaxFactory.ParseCompilationUnit($@"class x {{
{member}
}}").Members[0]).Members[0];
            return decl.WithAdditionalAnnotations(Formatter.Annotation);
        }
    }
}
