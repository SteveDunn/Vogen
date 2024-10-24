using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
// ReSharper disable NullableWarningSuppressionIsUsed
// ReSharper disable InconsistentNaming

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
    public bool IsTheUnderlyingAReferenceType => !IsTheUnderlyingAValueType;

    public bool IsTheWrapperAValueType { get; init; }
    public bool IsTheWrapperAReferenceType => !IsTheWrapperAValueType;

    public List<InstanceProperties> InstanceProperties { get; init; } = new();

    public string FullNamespace { get; init; } = string.Empty;

    public INamedTypeSymbol ValidationExceptionSymbol => Config.ValidationExceptionType ?? DefaultValidationExceptionSymbol;
    
    public required INamedTypeSymbol DefaultValidationExceptionSymbol { get; init; }

    public string ValidationExceptionFullName => Config.ValidationExceptionType?.FullName() ?? "global::Vogen.ValueObjectValidationException";

    public string VoTypeName => TypeToAugment.Identifier.ToString();
    
    public string UnderlyingTypeFullName => _underlyingTypeFullName;

    public bool IsSealed { get; init; }

    public string AccessibilityKeyword { get; init; } = "public";
    
    public required UserProvidedOverloads UserProvidedOverloads { get; init; }
    
    public required INamedTypeSymbol WrapperType { get; init; }
    
    public required ParsingInformation ParsingInformation { get; init; }
    
    public required ToStringInformation ToStringInformation { get; init; }

    public required LanguageVersion LanguageVersion { get; init; }
    
    public required VogenConfiguration Config { get; init; }
    public required Nullable Nullable { get; init; }
}

public class ParsingInformation
{
    public required List<IMethodSymbol> TryParseMethodsOnThePrimitive { get; init; }
    
    public required List<IMethodSymbol> ParseMethodsOnThePrimitive { get; init; }

    public bool PrimitiveHasNoParseOrTryParseMethods => TryParseMethodsOnThePrimitive.Count == 00 && ParseMethodsOnThePrimitive.Count == 0;

    public required bool UnderlyingIsAString { get; init; }
    
    // ReSharper disable once InconsistentNaming
    public required bool IParsableIsAvailable { get; init; }
    
    public required bool UnderlyingDerivesFromIParsable { get; init; }
    
    public required bool UnderlyingDerivesFromISpanParsable { get; init; }
    
    public required bool UnderlyingDerivesFromIUtf8SpanParsable { get; init; }
    public required INamedTypeSymbol? IFormatProviderType { get; init; }
}

public class ToStringInformation
{
    public required List<IMethodSymbol> ToStringMethodsOnThePrimitive { get; init; }
    
    public required bool UnderlyingIsAString { get; init; }
    
    public required bool UnderlyingTypeHasADefaultToStringMethod { get; init; }
}

public class UserProvidedOverloads
{
    public required UserProvidedToStringMethods ToStringOverloads { get; init; }

    public UserProvidedGetHashCode HashCodeInfo { get; init; }
    
    public UserProvidedEqualsForWrapper EqualsForWrapper { get; init; }
    
    public UserProvidedEqualsForUnderlying EqualsForUnderlying { get; init; }
    
    public required UserProvidedParseMethods ParseMethods { get; init; }
    
    public required UserProvidedTryParseMethods TryParseMethods { get; init; }
}

