using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace Vogen;

[Flags]
public enum DiagnosticReportOptions
{
    None = 0x0,
    ReportOnMethodName = 0x1,
}

internal static class ContextExtensions
{
    private static Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, Location location, ImmutableDictionary<string, string?>? properties, params string[] messageArgs)
    {
        return Diagnostic.Create(descriptor, location, properties, messageArgs);
    }

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken syntaxToken, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, syntaxToken, messageArgs);
    }

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxToken syntaxToken, params string[] messageArgs)
    {
        context.ReportDiagnostic(CreateDiagnostic(descriptor, syntaxToken.GetLocation(), properties, messageArgs));
    }

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode syntaxNode, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, syntaxNode, messageArgs);
    }

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxNode syntaxNode, params string[] messageArgs)
    {
        context.ReportDiagnostic(CreateDiagnostic(descriptor, syntaxNode.GetLocation(), properties, messageArgs));
    }

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, symbol, messageArgs);
    }

    public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ISymbol symbol, params string[] messageArgs)
    {
        foreach (var location in symbol.Locations)
        {
            context.ReportDiagnostic(CreateDiagnostic(descriptor, location, properties, messageArgs));
        }
    }

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, symbol, messageArgs);
    }

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ISymbol symbol, params string[] messageArgs)
    {
        foreach (var location in symbol.Locations)
        {
            ReportDiagnostic(context, descriptor, properties, location, messageArgs);
        }
    }

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxReference syntaxReference, params string[] messageArgs)
    {
        var syntaxNode = syntaxReference.GetSyntax(context.CancellationToken);
        context.ReportDiagnostic(Diagnostic.Create(descriptor, syntaxNode.GetLocation(), ImmutableDictionary<string, string?>.Empty, messageArgs));
    }

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, params string[] messageArgs)
    {
        context.ReportDiagnostic(Diagnostic.Create(descriptor, location, ImmutableDictionary<string, string?>.Empty, messageArgs));
    }

    public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, Location location, params string[] messageArgs)
    {
        context.ReportDiagnostic(CreateDiagnostic(descriptor, location, properties, messageArgs));
    }

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken token, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, token, messageArgs);
    }

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxToken token, params string[] messageArgs)
    {
        context.ReportDiagnostic(CreateDiagnostic(descriptor, token.GetLocation(), properties, messageArgs));
    }

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode node, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, node, messageArgs);
    }

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxNode node, params string[] messageArgs)
    {
        context.ReportDiagnostic(CreateDiagnostic(descriptor, node.GetLocation(), properties, messageArgs));
    }

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, IOperation operation, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, operation, messageArgs);
    }

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IOperation operation, params string[] messageArgs)
    {
        context.ReportDiagnostic(CreateDiagnostic(descriptor, operation.Syntax.GetLocation(), properties, messageArgs));
    }

    public static void ReportDiagnostic(this OperationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, IInvocationOperation operation, DiagnosticReportOptions options, params string[] messageArgs)
    {
        if (options.HasFlag(DiagnosticReportOptions.ReportOnMethodName) &&
            operation.Syntax.ChildNodes().FirstOrDefault() is MemberAccessExpressionSyntax memberAccessExpression)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, memberAccessExpression.Name.GetLocation(), properties, messageArgs));
            return;
        }

        context.ReportDiagnostic(descriptor, properties, operation, messageArgs);
    }

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, symbol, messageArgs);
    }

    public static void ReportDiagnostic(this CompilationAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ISymbol symbol, params string[] messageArgs)
    {
        foreach (var location in symbol.Locations)
        {
            context.ReportDiagnostic(CreateDiagnostic(descriptor, location, properties, messageArgs));
        }
    }

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, symbol, messageArgs);
    }

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, ISymbol symbol, params string[] messageArgs)
    {
        foreach (var location in symbol.Locations)
        {
            ReportDiagnostic(context, descriptor, properties, location, messageArgs);
        }
    }

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode syntax, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, syntax.GetLocation(), messageArgs);
    }

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken token, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, ImmutableDictionary<string, string?>.Empty, token.GetLocation(), messageArgs);
    }

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, SyntaxNode syntax, params string[] messageArgs)
    {
        ReportDiagnostic(context, descriptor, properties, syntax.GetLocation(), messageArgs);
    }

    public static void ReportDiagnostic(this OperationBlockAnalysisContext context, DiagnosticDescriptor descriptor, ImmutableDictionary<string, string?>? properties, Location location, params string[] messageArgs)
    {
        context.ReportDiagnostic(CreateDiagnostic(descriptor, location, properties, messageArgs));
    }
}