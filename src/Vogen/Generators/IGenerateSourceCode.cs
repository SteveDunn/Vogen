using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators;

public interface IGenerateSourceCode
{
    string BuildClass(ValueObjectWorkItem item, TypeDeclarationSyntax tds, List<string> log);
}