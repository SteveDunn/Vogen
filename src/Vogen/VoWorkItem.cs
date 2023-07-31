using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public class VoWorkItem
{
    private INamedTypeSymbol _underlyingType = null!;
    private string _underlyingTypeFullName = null!;
    public MethodDeclarationSyntax? NormalizeInputMethod { get; set; }
    
    public MethodDeclarationSyntax? ValidateMethod { get; set; }

    public INamedTypeSymbol UnderlyingType
    {
        get => _underlyingType;
        set
        {
            _underlyingType = value;
            _underlyingTypeFullName = value.FullName() ?? value?.Name ?? throw new InvalidOperationException(
                "No underlying type specified - please file a bug at https://github.com/SteveDunn/Vogen/issues/new?assignees=&labels=bug&template=BUG_REPORT.yml");
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
    
    public DebuggerAttributeGeneration DebuggerAttributes { get; set; }
    
    public ComparisonGeneration ComparisonGeneration { get; set; }
}