using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vogen.Diagnostics;
using Document = Microsoft.CodeAnalysis.Document;
using Formatter = Microsoft.CodeAnalysis.Formatting.Formatter;

namespace Vogen.Rules
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddValidationAnalyzerCodeFixProvider)), Shared]
    public class AddValidationAnalyzerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(RuleIdentifiers.AddValidationMethod); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return null!;
            // // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            // return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root!.FindToken(diagnosticSpan.Start).Parent!.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Resources.AddValidationCodeFixTitle,
                    createChangedDocument: c => GenerateValidationMethodAsync(context.Document, context.Diagnostics, declaration, c),
                    // createChangedSolution: c => GenerateValidationMethodAsync(context.Document, declaration, c),
                    equivalenceKey: nameof(Resources.AddValidationCodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> GenerateValidationMethodAsync(Document document,
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

        private static MethodDeclarationSyntax GenerateMethod(string primitiveType)
        {
            return (MethodDeclarationSyntax) ParseMember(
@$"private static Validation Validate({primitiveType} input)
{{
    bool isValid = true ; // todo: your validation
    return isValid ? Validation.Ok : Validation.Invalid(""[todo: describe the validation]"");
}}
").WithAdditionalAnnotations(Simplifier.Annotation);
        }

        private static MemberDeclarationSyntax ParseMember(string member)
        {
            MemberDeclarationSyntax decl = ((ClassDeclarationSyntax) SyntaxFactory.ParseCompilationUnit($@"class x {{
{member}
}}").Members[0]).Members[0];
            return decl.WithAdditionalAnnotations(Formatter.Annotation);
        }
    }
}
