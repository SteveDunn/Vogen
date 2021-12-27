using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

internal class VoTarget
{
    public VoTarget(TypeDeclarationSyntax typeToAugment, INamedTypeSymbol? containingType, INamedTypeSymbol symbolForType)
    {
        TypeToAugment = typeToAugment ?? throw new InvalidOperationException("No type to augment!");
        ContainingType = containingType;
        SymbolForType = symbolForType ?? throw new InvalidOperationException("No symbol for type!");
    }

    public TypeDeclarationSyntax TypeToAugment { get; set; }
    
    public INamedTypeSymbol? ContainingType { get; set; }
    
    public INamedTypeSymbol SymbolForType { get; set; }
}