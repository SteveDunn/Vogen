using Microsoft.CodeAnalysis;
// ReSharper disable InconsistentNaming

namespace Vogen;

public class VogenKnownSymbols(Compilation compilation) : KnownSymbols(compilation)
{
    public INamedTypeSymbol ValueObjectValidationException => GetOrResolveType("Vogen.ValueObjectValidationException", ref _ValueObjectValidationException)!;
    private Option<INamedTypeSymbol?> _ValueObjectValidationException;

    public INamedTypeSymbol? VogenProduceDiagnosticsMarkerType => GetOrResolveType("Vogen.__ProduceDiagnostics", ref _VogenProduceDiagnosticsMarkerType);
    private Option<INamedTypeSymbol?> _VogenProduceDiagnosticsMarkerType;

    public INamedTypeSymbol? SwaggerISchemaFilter => GetOrResolveType("Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter", ref _SwaggerISchemaFilter);
    private Option<INamedTypeSymbol?> _SwaggerISchemaFilter;

    public INamedTypeSymbol? JsonConverterFactory => GetOrResolveType("System.Text.Json.Serialization.JsonConverterFactory", ref _JsonConverterFactory);
    private Option<INamedTypeSymbol?> _JsonConverterFactory;

    public INamedTypeSymbol? OpenApiOptions => GetOrResolveType("Microsoft.AspNetCore.OpenApi.OpenApiOptions", ref _OpenApiOptions);
    private Option<INamedTypeSymbol?> _OpenApiOptions;

    public INamedTypeSymbol? TypeShapeAttribute => GetOrResolveType("PolyType.TypeShapeAttribute", ref _TypeShapeAttribute);
    private Option<INamedTypeSymbol?> _TypeShapeAttribute;

    public INamedTypeSymbol? DateOnly => GetOrResolveType("System.DateOnly", ref _DateOnly);
    private Option<INamedTypeSymbol?> _DateOnly;

    public INamedTypeSymbol? TimeOnly => GetOrResolveType("System.DateOnly", ref _TimeOnly);
    private Option<INamedTypeSymbol?> _TimeOnly;
}