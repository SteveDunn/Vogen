using Microsoft.CodeAnalysis.CSharp.Syntax;
// ReSharper disable UnusedParameter.Global

namespace Vogen.Generators.Conversions;

public interface IGenerateConversion
{
    string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item, VogenKnownSymbols knownSymbols);

    string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item, VogenKnownSymbols knownSymbols);
}