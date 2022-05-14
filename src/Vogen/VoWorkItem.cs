using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public class VoWorkItem
{
    public MethodDeclarationSyntax? NormalizeInputMethod { get; set; }
    
    public MethodDeclarationSyntax? ValidateMethod { get; set; }

    public INamedTypeSymbol? UnderlyingType { get; set; } = null!;

    /// <summary>
    /// The syntax information for the type to augement.
    /// </summary>
    public TypeDeclarationSyntax TypeToAugment { get; set; } = null!;
    
    public bool IsValueType { get; set; }

    public List<InstanceProperties> InstanceProperties { get; set; } = new();

    public string FullNamespace { get; set; } = string.Empty;

    public Conversions Conversions { get; set; }

    public INamedTypeSymbol? TypeForValidationExceptions { get; set; } = null!;

    public string ValidationExceptionFullName => TypeForValidationExceptions?.FullName() ?? "global::Vogen.ValueObjectValidationException";

    public string VoTypeName => TypeToAugment.Identifier.ToString();
    
    public string UnderlyingTypeFullName => UnderlyingType.FullName() ?? UnderlyingType?.Name ?? "global::System.Int32";
}