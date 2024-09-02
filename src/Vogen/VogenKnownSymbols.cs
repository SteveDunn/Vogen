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
}