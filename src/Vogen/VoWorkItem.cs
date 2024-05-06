using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace Vogen;

public class VoWorkItem
{
    private readonly INamedTypeSymbol _underlyingType = null!;
    private readonly string _underlyingTypeFullName = null!;
    public MethodDeclarationSyntax? NormalizeInputMethod { get; init; }
    
    public MethodDeclarationSyntax? ValidateMethod { get; init; }

    public INamedTypeSymbol UnderlyingType
    {
        get => _underlyingType;
        init
        {
            _underlyingType = value;
            _underlyingTypeFullName = value.FullName() ?? value.Name ?? throw new InvalidOperationException(
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

    public INamedTypeSymbol? TypeForValidationExceptions { get; init; }

    public string ValidationExceptionFullName => TypeForValidationExceptions?.FullName() ?? "global::Vogen.ValueObjectValidationException";

    public string VoTypeName => TypeToAugment.Identifier.ToString();
    
    public string UnderlyingTypeFullName => _underlyingTypeFullName;
    
    public DebuggerAttributeGeneration DebuggerAttributes { get; init; }
    
    public ComparisonGeneration ComparisonGeneration { get; init; }
    
    public StringComparersGeneration StringComparersGeneration { get; init; }
    
    public ParsableForStrings ParsableForStrings { get; init; }
    
    public ParsableForPrimitives ParsableForPrimitives { get; init; }
    
    public bool IsSealed { get; init; }
    
    public CastOperator ToPrimitiveCastOperator { get; init; }
    
    public CastOperator FromPrimitiveCastOperator { get; init; }
    
    public bool DisableStackTraceRecordingInDebug { get; init; }

    public string AccessibilityKeyword { get; init; } = "public";
    
    public required UserProvidedOverloads UserProvidedOverloads { get; init; }
    
    public required INamedTypeSymbol WrapperType { get; set; }
    
    public required ParsingInformation ParsingInformation { get; set; }
    
    public TryFromGeneration TryFromGeneration { get; init; }
    
    public IsInitializedGeneration IsInitializedGeneration { get; init; }
}

public class ParsingInformation
{
    public required List<IMethodSymbol> TryParseMethodsOnThePrimitive { get; set; }
    
    public required List<IMethodSymbol> ParseMethodsOnThePrimitive { get; set; }

    public bool PrimitiveHasNoParseOrTryParseMethods => TryParseMethodsOnThePrimitive.Count == 00 && ParseMethodsOnThePrimitive.Count == 0;

    public required bool UnderlyingIsAString { get; init; }
    
    public required bool IParsableIsAvailable { get; init; }
    
    public required bool UnderlyingDerivesFromIParsable { get; init; }
    
    public required bool UnderlyingDerivesFromISpanParsable { get; init; }
    
    public required bool UnderlyingDerivesFromIUtf8SpanParsable { get; init; }
    public required INamedTypeSymbol? IFormatProviderType { get; init; }
}

public class UserProvidedOverloads
{
    public UserProvidedToString ToStringInfo { get; set; }

    public UserProvidedGetHashCode HashCodeInfo { get; set; }
    
    public UserProvidedEqualsForWrapper EqualsForWrapper { get; set; }
    
    public UserProvidedEqualsForUnderlying EqualsForUnderlying { get; set; }
    
    public required UserProvidedParseMethods ParseMethods { get; set; }
    
    public required UserProvidedTryParseMethods TryParseMethods { get; set; }
}

