using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
// ReSharper disable NullableWarningSuppressionIsUsed
// ReSharper disable InconsistentNaming

namespace Vogen;

public class VoWorkItem
{
    private readonly INamedTypeSymbol _underlyingType = null!;
    private readonly string _underlyingTypeFullNameWithGlobalAlias = null!;
    private readonly string _underlyingTypeFullName = null!;
    
    public MethodDeclarationSyntax? NormalizeInputMethod { get; init; }
    
    public MethodDeclarationSyntax? ValidateMethod { get; init; }

    public INamedTypeSymbol UnderlyingType
    {
        get => _underlyingType;
#if !NETSTANDARD
        [System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(_underlyingTypeFullNameWithGlobalAlias))]
#endif
        init
        {
            _underlyingType = value;
            _underlyingTypeFullName = value.EscapedFullName();
            _underlyingTypeFullNameWithGlobalAlias = value.FullNameWithGlobalAlias();
            IsUnderlyingAString = SeeIfAssignableFromString();
        }
    }

    private bool SeeIfAssignableFromString()
    {
        // for issue: https://github.com/dotnet/runtime/issues/113534
        try
        {
            return typeof(string).IsAssignableFrom(Type.GetType(_underlyingTypeFullName));
        }
        catch (FileLoadException)
        {
            return false;
        }
    }

    public bool IsRecordClass => TypeToAugment is RecordDeclarationSyntax rds && rds.IsKind(SyntaxKind.RecordDeclaration);
    
    public bool IsRecordStruct => TypeToAugment is RecordDeclarationSyntax rds && rds.IsKind(SyntaxKind.RecordStructDeclaration);

    public bool IsUnderlyingAString { get; private set; }
    
    /// <summary>
    /// The syntax information for the type to augment.
    /// </summary>
    public required TypeDeclarationSyntax TypeToAugment { get; init; }
    
    public required bool IsTheUnderlyingAValueType { get; init; }

    public required bool IsTheWrapperAValueType { get; init; }
    public bool IsTheWrapperAReferenceType => !IsTheWrapperAValueType;

    public List<InstanceProperties> InstanceProperties { get; init; } = new();

    /// <summary>
    /// The full namespace including the global namespace, e.g. global::MyNamespace.Whatever
    /// </summary>
    public required string FullAliasedNamespace { get; init; }

    /// <summary>
    /// The namespace excluding the global namespace, e.g. MyNamespace.Whatever
    /// </summary>
    public required string FullUnaliasedNamespace { get; init; }

    public INamedTypeSymbol ValidationExceptionSymbol => Config.ValidationExceptionType ?? DefaultValidationExceptionSymbol;
    
    public required INamedTypeSymbol DefaultValidationExceptionSymbol { get; init; }

    public string ValidationExceptionFullName => Config.ValidationExceptionType?.EscapedFullName() ?? "global::Vogen.ValueObjectValidationException";

    public string VoTypeName => TypeToAugment.Identifier.ToString();
    
    /// <summary>
    /// The full name of the underlying type, including the full namespace and the global namespace.
    /// The global namespace is included as we want to differentiate the type type name (e.g. `System.Guid`) from
    /// any property names the user might have chosen (e.g. `System`). This is so the generated code will call
    /// `global::System.Guid.TryParse` instead of `System.Guid.TryParse`, where the later might assume the property
    /// name `System` if there was one.
    /// </summary>
    public string UnderlyingTypeFullNameWithGlobalAlias => _underlyingTypeFullNameWithGlobalAlias;

    public string UnderlyingTypeFullName => _underlyingTypeFullName;
    

    public required bool IsSealed { get; init; }

    public string AccessibilityKeyword { get; init; } = "public";
    
    public required UserProvidedOverloads UserProvidedOverloads { get; init; }
    public required UserProvidedPartials UserProvidedPartials { get; init; }
    
    public required INamedTypeSymbol WrapperType { get; init; }
    
    public required ParsingInformation ParsingInformation { get; init; }
    
    public required ToStringInformation ToStringInformation { get; init; }

    public required LanguageVersion LanguageVersion { get; init; }
    
    public required VogenConfiguration Config { get; init; }

    public required Nullable Nullable { get; init; }

    public bool HasConversion(Conversions conversion) => this.Config.Conversions.HasFlag(conversion);
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
    public required UserProvidedTryFormatMethods TryFormatMethods { get; init; }
}

public record UserProvidedPartial(
    Accessibility DeclaredAccessibility
);

public class UserProvidedPartials
{
    public required UserProvidedPartial? PartialValue { get; init; }
    public required UserProvidedPartial? PartialFrom { get; init; }
    public required UserProvidedPartial? PartialBoolTryFrom { get; init; }
    public required UserProvidedPartial? PartialErrorTryFrom { get; init; }
}
