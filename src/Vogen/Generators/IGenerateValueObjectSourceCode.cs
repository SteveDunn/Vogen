using Microsoft.CodeAnalysis;

namespace Vogen.Generators;

public interface IGenerateValueObjectSourceCode
{
    string Generate(GenerationParameters parameters);
}

public record struct GenerationParameters(VoWorkItem WorkItem, SourceProductionContext Context, VogenKnownSymbols VogenKnownSymbols, Compilation Compilation);
