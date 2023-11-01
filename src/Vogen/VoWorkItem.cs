using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public class VoWorkItem
{
    private INamedTypeSymbol _underlyingType = null!;
    private string _underlyingTypeFullName = null!;
    public MethodDeclarationSyntax? NormalizeInputMethod { get; init; }
    
    public MethodDeclarationSyntax? ValidateMethod { get; init; }

    public INamedTypeSymbol UnderlyingType
    {
        get => _underlyingType;
        init
        {
            _underlyingType = value;
            _underlyingTypeFullName = value.FullName() ?? value?.Name ?? throw new InvalidOperationException(
                "No underlying type specified - please file a bug at https://github.com/SteveDunn/Vogen/issues/new?assignees=&labels=bug&template=BUG_REPORT.yml");
            IsUnderlyingAString = typeof(string).IsAssignableFrom(Type.GetType(_underlyingTypeFullName));
        }
    }

    public bool IsRecordClass => TypeToAugment is RecordDeclarationSyntax rds && rds.IsKind(SyntaxKind.RecordDeclaration);
    
    public bool IsRecordStruct => TypeToAugment is RecordDeclarationSyntax rds && rds.IsKind(SyntaxKind.RecordStructDeclaration);


    public bool IsUnderlyingAString { get; private set; }

    /// <summary>
    /// The syntax information for the type to augment.
    /// </summary>
    public TypeDeclarationSyntax TypeToAugment { get; init; } = null!;
    
    public bool IsTheUnderlyingAValueType { get; init; }

    public bool IsTheWrapperAValueType { get; init; }

    public List<InstanceProperties> InstanceProperties { get; init; } = new();

    public string FullNamespace { get; init; } = string.Empty;

    public Conversions Conversions { get; init; }
    
    public DeserializationStrictness DeserializationStrictness { get; init; }
    
    public Customizations Customizations { get; init; }

    public INamedTypeSymbol? TypeForValidationExceptions { get; init; } = null!;

    public string ValidationExceptionFullName => TypeForValidationExceptions?.FullName() ?? "global::Vogen.ValueObjectValidationException";

    public string VoTypeName => TypeToAugment.Identifier.ToString();
    
    public string UnderlyingTypeFullName => _underlyingTypeFullName;

    public bool HasToString { get; init; }
    
    public DebuggerAttributeGeneration DebuggerAttributes { get; init; }
    
    public ComparisonGeneration ComparisonGeneration { get; init; }
    
    public StringComparersGeneration StringComparersGeneration { get; init; }
    
    public bool IsSealed { get; init; }
    
    public CastOperator ToPrimitiveCastOperator { get; init; }
    
    public CastOperator FromPrimitiveCastOperator { get; init; }
    
    public bool DisableStackTraceRecordingInDebug { get; init; }

    public string AccessibilityKeyword { get; init; } = "public";
}