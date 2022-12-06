using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public class VoWorkItem
{
    private INamedTypeSymbol? _underlyingType = null!;
    private string _underlyingTypeFullName = null!;
    public MethodDeclarationSyntax? NormalizeInputMethod { get; set; }
    
    public MethodDeclarationSyntax? ValidateMethod { get; set; }

    public INamedTypeSymbol? UnderlyingType
    {
        get => _underlyingType;
        set
        {
            _underlyingType = value;
            _underlyingTypeFullName = value.FullName() ?? value?.Name ?? "global::System.Int32";
        }
    }

    /// <summary>
    /// The syntax information for the type to augment.
    /// </summary>
    public TypeDeclarationSyntax TypeToAugment { get; set; } = null!;
    
    public bool IsValueType { get; set; }

    public List<InstanceProperties> InstanceProperties { get; set; } = new();

    public string FullNamespace { get; set; } = string.Empty;

    public Conversions Conversions { get; set; }
    
    public DeserializationStrictness DeserializationStrictness { get; set; }
    
    public Customizations Customizations { get; set; }

    public INamedTypeSymbol? TypeForValidationExceptions { get; set; } = null!;

    public string ValidationExceptionFullName => TypeForValidationExceptions?.FullName() ?? "global::Vogen.ValueObjectValidationException";

    public string VoTypeName => TypeToAugment.Identifier.ToString();
    
    public string UnderlyingTypeFullName => _underlyingTypeFullName;

    public bool HasToString { get; set; }
}