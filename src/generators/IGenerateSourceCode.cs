using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace generator.generators;

public interface IGenerateSourceCode
{
    string BuildClass(ValueObjectWorkItem item, TypeDeclarationSyntax tds, List<string> log);
}