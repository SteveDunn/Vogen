using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Vogen.Diagnostics;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace Vogen;

internal static class BuildConverterMarkerDefinitionsFromAttributes
{
    public static ConversionMarkerAttributeDefinition? TryBuild(AttributeData att, in INamedTypeSymbol? markerClassSymbol)
    {
        ImmutableArray<TypedConstant> args = att.ConstructorArguments;

        var hasErroredAttributes = args.Any(a => a.Kind == TypedConstantKind.Error);

        if (hasErroredAttributes)
        {
            // skip further generator execution and let compiler generate the errors
            return null;
        }

        if (att.AttributeClass == null)
        {
            return null;
        }

        ImmutableArray<ITypeSymbol> argumentSymbols = att.AttributeClass.TypeArguments;

        if (argumentSymbols.Length != 1)
        {
            return null;
        }

        var voSymbol = argumentSymbols[0] as INamedTypeSymbol;
        
        if (voSymbol is null)
        {
            return null;
        }

        if (!VoFilter.IsTarget(voSymbol))
        {
            return ConversionMarkerAttributeDefinition.Error(ConverterMarkerKind.EFCore, DiagnosticsCatalogue.EfCoreTargetMustBeAVo(markerClassSymbol!, voSymbol));
        }

        List<AttributeData> attrs = VoFilter.TryGetValueObjectAttributes(voSymbol).ToList();
        
        if(attrs.Count != 1) return null;

        AttributeData a = attrs[0];
        
        VogenConfigurationBuildResult config = BuildConfigurationFromAttributes.TryBuildFromValueObjectAttribute(a);
        
        if(config.HasDiagnostics) return null;

        VogenConfiguration c = config.ResultingConfiguration!;

        var underlyingType = c.UnderlyingType ?? ResolveUnderlyingType(voSymbol);

        if (underlyingType is null)
        {
            return ConversionMarkerAttributeDefinition.Error(ConverterMarkerKind.EFCore, DiagnosticsCatalogue.VoMustExplicitlySpecifyPrimitiveToBeAnEfCoreTarget(voSymbol));
        }
        
        return ConversionMarkerAttributeDefinition.Ok(ConverterMarkerKind.EFCore, voSymbol, underlyingType, markerClassSymbol!);
    }

    private static INamedTypeSymbol? ResolveUnderlyingType(INamedTypeSymbol method)
    {
        ImmutableArray<ISymbol> ms = method.GetMembers("Value");
        
        if (ms.Length == 0)
        {
            return null;
        }

        IPropertySymbol? prop = ms[0] as IPropertySymbol;
        
        return (INamedTypeSymbol)prop!.Type;
    }
}