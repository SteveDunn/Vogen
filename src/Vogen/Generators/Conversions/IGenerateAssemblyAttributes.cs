using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

public interface IGenerateAssemblyAttributes
{
    string GenerateAssemblyAttributes(TypeDeclarationSyntax tds, VoWorkItem item);
}