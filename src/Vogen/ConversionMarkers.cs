using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;

namespace Vogen;

internal static class ConversionMarkers
{
    private static readonly Dictionary<string, ConversionMarkerKind> _knownMarkerAttributes = new()
    {
        { "OpenApiMarkerAttribute`1", ConversionMarkerKind.OpenApi },
        { "EfCoreConverterAttribute`1", ConversionMarkerKind.EFCore },
        { "MessagePackAttribute`1", ConversionMarkerKind.MessagePack },
        { "BsonSerializerAttribute`1", ConversionMarkerKind.Bson }
    };

    public static MarkerClassDefinition? GetMarkerClassFromAttribute(GeneratorSyntaxContext context)
    {
        var semanticModel = context.SemanticModel;

        ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(context.Node)!;

        var markerClassSymbol = (INamedTypeSymbol) declaredSymbol;

        ImmutableArray<AttributeData> attributeData = GetAnyMarkerAttributes(markerClassSymbol).ToImmutableArray();

        if (attributeData.Length == 0)
        {
            return null;
        }

        return new MarkerClassDefinition(
            markerClassSymbol, 
            attributeData.Select(a => TryBuild(a, markerClassSymbol)).Where(a => a is not null)!);
    }

    private static IEnumerable<AttributeData> GetAnyMarkerAttributes(INamedTypeSymbol markerClass)
    {
        var attrs = markerClass.GetAttributes();

        return attrs.Where(
            a => _knownMarkerAttributes.ContainsKey(a.AttributeClass?.MetadataName ?? ""));
    }

    public static bool IsTarget(SyntaxNode node) => 
        node is TypeDeclarationSyntax { AttributeLists.Count: > 0 };

    private static MarkerAttributeDefinition? TryBuild(AttributeData markerAtt, in INamedTypeSymbol? markerClassSymbol)
    {
        ImmutableArray<TypedConstant> args = markerAtt.ConstructorArguments;

        var hasErroredAttributes = args.Any(a => a.Kind == TypedConstantKind.Error);

        if (hasErroredAttributes)
        {
            // skip further generator execution and let compiler generate the errors
            return null;
        }

        if (markerAtt.AttributeClass?.TypeArguments.SingleOrDefaultIfMultiple() is not INamedTypeSymbol voSymbol)
        {
            return null;
        }

        ConversionMarkerKind markerKind = ResolveMarkerKind(markerAtt);
        
        if(markerKind is ConversionMarkerKind.Unrecognized)
        {
            return null;
        }

        if (!VoFilter.IsTarget(voSymbol))
        {
            return MarkerAttributeDefinition.Error(
                DiagnosticsCatalogue.TypesReferencedInAConversionMarkerMustBeaValueObjects(markerClassSymbol!, voSymbol));
        }

        List<AttributeData> voAttributes = VoFilter.TryGetValueObjectAttributes(voSymbol).ToList();
        
        if(voAttributes.Count != 1)
        {
            return null;
        }

        AttributeData voAttribute = voAttributes[0];
        
        VogenConfigurationBuildResult config = BuildConfigurationFromAttributes.TryBuildFromValueObjectAttribute(voAttribute);
        
        if(config.HasDiagnostics)
        {
            return null;
        }

        VogenConfiguration c = config.ResultingConfiguration!;

        var underlyingType = c.UnderlyingType ?? TryResolveUnderlyingType(voSymbol);

        if (underlyingType is null)
        {
            return MarkerAttributeDefinition.Error(
                DiagnosticsCatalogue.VoReferencedInAConversionMarkerMustExplicitlySpecifyPrimitive(
                    markerClassSymbol!, 
                    voSymbol, 
                    markerAtt.ApplicationSyntaxReference?.GetSyntax().GetLocation()));
        }
        
        return MarkerAttributeDefinition.Ok(markerKind, voSymbol, underlyingType, markerClassSymbol!);
    }

    private static ConversionMarkerKind ResolveMarkerKind(AttributeData att) => 
        // ReSharper disable once CanSimplifyDictionaryTryGetValueWithGetValueOrDefault
        _knownMarkerAttributes.TryGetValue(att.AttributeClass?.MetadataName ?? "", out var kind) ? kind : ConversionMarkerKind.Unrecognized;

    private static INamedTypeSymbol? TryResolveUnderlyingType(INamedTypeSymbol method)
    {
        ImmutableArray<ISymbol> ms = method.GetMembers("Value");
        
        if (ms.Length == 0)
        {
            return null;
        }

        IPropertySymbol? prop = ms[0] as IPropertySymbol;
        
        return prop!.Type as INamedTypeSymbol;
    }
}