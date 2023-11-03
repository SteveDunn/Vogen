using System;
using Microsoft.CodeAnalysis;

namespace Vogen;

public class NestingInfo
{
    private INamedTypeSymbol? _containingType;

    private NestingInfo()
    {
    }
    
    public static NestingInfo AsNestedIn(INamedTypeSymbol containingType) => new() { _containingType = containingType };

    public static readonly NestingInfo NotNested = new();

    public INamedTypeSymbol ContainingType => _containingType ?? throw new InvalidOperationException("Not a nested type!");

    public bool IsNested => _containingType is not null; 
}