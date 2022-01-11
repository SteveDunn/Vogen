using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

public interface IGenerateConversion
{
    string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item);

    string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item);
}