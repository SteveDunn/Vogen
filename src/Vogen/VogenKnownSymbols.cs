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

    /// <summary>
    /// OpenApiSchema from Microsoft.OpenApi namespace, as used in OpenApi v2+
    /// </summary>
    public INamedTypeSymbol? OpenApiSchemaV2 => GetOrResolveType("Microsoft.OpenApi.OpenApiSchema", ref _OpenApiSchemaV2Options);
    private Option<INamedTypeSymbol?> _OpenApiSchemaV2Options;

    /// <summary>
    /// OpenApiSchema from Microsoft.OpenApi.Models namespace, as used in OpenApi v1+
    /// </summary>
    public INamedTypeSymbol? OpenApiSchemaV1 => GetOrResolveType("Microsoft.OpenApi.Models.OpenApiSchema", ref _OpenApiSchemaV1Options);
    private Option<INamedTypeSymbol?> _OpenApiSchemaV1Options;

    public INamedTypeSymbol? JsonSchemaType => GetOrResolveType("Microsoft.OpenApi.JsonSchemaType", ref _JsonSchemaType);
    private Option<INamedTypeSymbol?> _JsonSchemaType;

    public INamedTypeSymbol? TypeShapeAttribute => GetOrResolveType("PolyType.TypeShapeAttribute", ref _TypeShapeAttribute);
    private Option<INamedTypeSymbol?> _TypeShapeAttribute;

    public INamedTypeSymbol? DateOnly => GetOrResolveType("System.DateOnly", ref _DateOnly);
    private Option<INamedTypeSymbol?> _DateOnly;

    public INamedTypeSymbol? TimeOnly => GetOrResolveType("System.DateOnly", ref _TimeOnly);
    private Option<INamedTypeSymbol?> _TimeOnly;

    public INamedTypeSymbol? StjSerializer => GetOrResolveType("System.Text.Json.JsonSerializer", ref _StjSerializer);
    private Option<INamedTypeSymbol?> _StjSerializer;
}