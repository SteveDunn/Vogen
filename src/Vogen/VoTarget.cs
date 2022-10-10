using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

/// <summary>
/// The 'first stage' pass of the analyzer creates these.
/// </summary>
internal class VoTarget
{
    public VoTarget(
        SemanticModel semanticModel, 
        TypeDeclarationSyntax typeToAugment,
        INamedTypeSymbol? containingType, 
        INamedTypeSymbol symbolForType,
        ImmutableArray<AttributeData> dataForAttributes)
    {
        SemanticModel = semanticModel;
        VoSyntaxInformation = typeToAugment ?? throw new InvalidOperationException("No type to augment!");
        ContainingType = containingType;
        VoSymbolInformation = symbolForType ?? throw new InvalidOperationException("No symbol for type!");
        DataForAttributes = dataForAttributes;
    }

    public SemanticModel SemanticModel { get; }

    /// <summary>
    /// The syntax for the type that the user provides, e.g. `CustomerId`
    /// </summary>
    public TypeDeclarationSyntax VoSyntaxInformation { get; set; }
    
    /// <summary>
    /// The type that contains this Value Object. Used to check (and throw) if
    /// it is a nested class as that is not supported.
    /// </summary>
    public INamedTypeSymbol? ContainingType { get; set; }
    
    /// <summary>
    /// The symbol information for the Value Object
    /// </summary>
    public INamedTypeSymbol VoSymbolInformation { get; set; }

    public ImmutableArray<AttributeData> DataForAttributes { get; }
}