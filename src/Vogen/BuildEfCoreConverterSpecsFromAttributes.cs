using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Vogen.Diagnostics;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace Vogen;

internal class BuildEfCoreConverterSpecsFromAttributes
{
    private readonly bool _hasErroredAttributes;
    private readonly AttributeData _matchingAttribute;
    private readonly INamedTypeSymbol? _symbol;

    private BuildEfCoreConverterSpecsFromAttributes(AttributeData att, INamedTypeSymbol? symbol)
    {
        _matchingAttribute = att;
        _symbol = symbol;

        ImmutableArray<TypedConstant> args = _matchingAttribute.ConstructorArguments;

        _hasErroredAttributes = args.Any(a => a.Kind == TypedConstantKind.Error);
    }

    public static EfCoreConverterSpecResult TryBuild(AttributeData matchingAttribute, INamedTypeSymbol? symbol) => 
        new BuildEfCoreConverterSpecsFromAttributes(matchingAttribute, symbol).Build();

    private EfCoreConverterSpecResult Build()
    {
        if (_hasErroredAttributes)
        {
            // skip further generator execution and let compiler generate the errors
            return EfCoreConverterSpecResult.Null;
        }

        if (_matchingAttribute.AttributeClass == null)
        {
            return EfCoreConverterSpecResult.Null;
        }

        ImmutableArray<ITypeSymbol> t = _matchingAttribute.AttributeClass.TypeArguments;

        if (t.Length != 1) return EfCoreConverterSpecResult.Null;

        var voSymbol = t[0] as INamedTypeSymbol;
        if (voSymbol is null) return EfCoreConverterSpecResult.Null;

        if (!VoFilter.IsTarget(voSymbol))
        {
            return EfCoreConverterSpecResult.Error(DiagnosticsCatalogue.EfCoreTargetMustBeAVo(_symbol!, voSymbol));
        }

        List<AttributeData> attrs = VoFilter.TryGetValueObjectAttributes(voSymbol).ToList();
        
        if(attrs.Count != 1) return EfCoreConverterSpecResult.Null;

        AttributeData a = attrs[0];
        
        VogenConfigurationBuildResult config = BuildConfigurationFromAttributes.TryBuildFromValueObjectAttribute(a);
        
        if(config.HasDiagnostics) return EfCoreConverterSpecResult.Null;

        VogenConfiguration c = config.ResultingConfiguration!;

        var underlyingType = c.UnderlyingType ?? ResolveUnderlyingType(voSymbol);

        if (underlyingType is null)
        {
            return EfCoreConverterSpecResult.Error(DiagnosticsCatalogue.VoMustExplicitlySpecifyPrimitiveToBeAnEfCoreTarget(voSymbol));
        }
        
        return EfCoreConverterSpecResult.Ok(voSymbol, underlyingType, _symbol!);
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