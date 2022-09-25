﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Vogen.Analyzers;

namespace Vogen.CodeFixers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Analyzer1CodeFixProvider)), Shared]
    public class Analyzer1CodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(Analyzer1Analyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
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
                    title: Resources.CodeFixTitle,
                    createChangedDocument: c => MakeUppercaseAsync(context.Document, declaration, c),
                    // createChangedSolution: c => MakeUppercaseAsync(context.Document, declaration, c),
                    equivalenceKey: nameof(Resources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> MakeUppercaseAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            SyntaxNode root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;
            
            var newMember = GenerateMethod();
            
            var newTypeDecl = typeDecl.AddMembers(newMember);
            
            var newRoot = root.ReplaceNode(typeDecl, newTypeDecl);

            return document.WithSyntaxRoot(newRoot);
        }


        private static MethodDeclarationSyntax GenerateMethod()
        {
            return (MethodDeclarationSyntax) ParseMember(
@"private static Validation Validate(int input)
{
    return input > 0 ? Validation.Ok : Validation.Invalid(""!!"");
}").WithAdditionalAnnotations(Simplifier.Annotation);
        }
        
        private static MemberDeclarationSyntax ParseMember(string member)
        {
            MemberDeclarationSyntax decl = ((ClassDeclarationSyntax)SyntaxFactory.ParseCompilationUnit("class x {\r\n" + member + "\r\n}").Members[0]).Members[0];
            return decl.WithAdditionalAnnotations(Formatter.Annotation);
        }
    }
}
