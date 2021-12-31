using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public class VoWorkItem
{
    public MethodDeclarationSyntax? ValidateMethod { get; set; }

    public INamedTypeSymbol? UnderlyingType { get; set; }

    public TypeDeclarationSyntax TypeToAugment { get; set; } = null!;
    
    public bool IsValueType { get; set; }

    public List<InstanceProperties> InstanceProperties { get; set; } = new();

    public string FullNamespace { get; set; } = string.Empty;
}