using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators;

public interface IGenerateSourceCode
{
    string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds);
}