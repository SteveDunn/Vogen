using Microsoft.CodeAnalysis;

namespace Vogen;

public static class DisplayFormats
{
    private static readonly SymbolDisplayMiscellaneousOptions _optionsWheNullabilityIsOn =
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
        SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier;

    private static readonly SymbolDisplayMiscellaneousOptions _optionsWheNullabilityIsOff =
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        SymbolDisplayMiscellaneousOptions.UseSpecialTypes;

    public static readonly SymbolDisplayFormat SymbolFormatWhenNullabilityIsOn =
        new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: _optionsWheNullabilityIsOn);

    public static readonly SymbolDisplayFormat SymbolFormatWhenNullabilityIsOff =
        new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: _optionsWheNullabilityIsOff);
    
}