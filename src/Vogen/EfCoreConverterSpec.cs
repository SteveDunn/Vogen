using Microsoft.CodeAnalysis;

namespace Vogen;

internal record class EfCoreConverterSpec(INamedTypeSymbol VoSymbol, INamedTypeSymbol UnderlyingType, INamedTypeSymbol SourceType);