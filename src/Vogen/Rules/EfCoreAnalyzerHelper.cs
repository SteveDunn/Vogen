using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Rules;

internal static class EfCoreAnalyzerHelper
{
    public static bool IsLinqToEntities(IMethodSymbol methodSymbol, Compilation compilation)
    {
        if (methodSymbol.ContainingType.EscapedFullName() == "System.Linq.Queryable")
        {
            return true;
        }

        var dbSetType = compilation.GetTypeByMetadataName("Microsoft.EntityFrameworkCore.DbSet`1");
        if (dbSetType is not null && InheritsFrom(methodSymbol.ContainingType, dbSetType))
        {
            return true;
        }

        return false;
    }

    private static bool InheritsFrom(ITypeSymbol? type, INamedTypeSymbol? baseType)
    {
        if (baseType is null) return false;

        while (type != null)
        {
            if (SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, baseType))
            {
                return true;
            }
            type = type.BaseType!;
        }

        return false;
    }

    public static bool IsValueObject(ITypeSymbol type) =>
        type is INamedTypeSymbol symbol && VoFilter.IsTarget(symbol);

    public static bool IsEfQuery(QueryExpressionSyntax queryExpr, SemanticModel semanticModel)
    {
        var fromClauseSymbol = semanticModel.GetSymbolInfo(queryExpr.FromClause.Expression).Symbol;
        ITypeSymbol? fromType = fromClauseSymbol switch
        {
            ILocalSymbol local => local.Type,
            IPropertySymbol property => property.Type,
            IFieldSymbol field => field.Type,
            _ => semanticModel.GetTypeInfo(queryExpr.FromClause.Expression).Type
        };

        if (fromType is null) return false;
        
        var iQueryableType = semanticModel.Compilation.GetTypeByMetadataName("System.Linq.IQueryable`1");

        if (iQueryableType is not null && InheritsFrom(fromType, iQueryableType))
        {
            return true;
        }

        var dbSetType = semanticModel.Compilation.GetTypeByMetadataName("Microsoft.EntityFrameworkCore.DbSet`1");

        if (dbSetType is not null && InheritsFrom(fromType, dbSetType))
        {
            return true;
        }
        return false;
    }
}
