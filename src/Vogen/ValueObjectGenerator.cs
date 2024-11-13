using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Generators;

// ReSharper disable NullableWarningSuppressionIsUsed
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable RedundantSuppressNullableWarningExpression

namespace Vogen;

[Generator]
public class ValueObjectGenerator : IIncrementalGenerator
{
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<VogenKnownSymbols> knownSymbols = context.CompilationProvider
            .Select((compilation, _) => new VogenKnownSymbols(compilation));
        
        Found found = GetTargets(context.SyntaxProvider);
        
        IncrementalValueProvider<ImmutableArray<VoTarget>> collectedVos = found.Vos.Collect();
            
        var targetsAndConfig = collectedVos.Combine(found.GlobalConfig.Collect());
            
        var targetsConfigAndConversionMarkers = targetsAndConfig.Combine(found.ConverterMarkerClasses.Collect());

        var compilationAndValues = context.CompilationProvider.Combine(targetsConfigAndConversionMarkers);
        
        var everything = compilationAndValues.Combine(knownSymbols);
            
        context.RegisterSourceOutput(everything,
            static (spc, source) =>
            {
                var left = source.Left;
        
                // todo: break apart building the work items, as that only needs global config,
                // which will make the following less horrific!
                Compilation compilation = left.Left;
                var targets = left.Right.Left.Left;
                var globalConfig = left.Right.Left.Right;
                var ks = source.Right;
                var ef = left.Right.Right;
                
                Execute(
                    compilation,
                    ks,
                    targets,
                    globalConfig,
                    ef,
                    spc);
            });
    }

    private static Found GetTargets(SyntaxValueProvider syntaxProvider)
    {
        IncrementalValuesProvider<VoTarget> targets = syntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => VoFilter.IsTarget(node),
                transform: static (ctx, _) => VoFilter.TryGetTarget(ctx))
            .Where(static m => m is not null)!;

        IncrementalValuesProvider<VogenConfigurationBuildResult> globalConfig = syntaxProvider.ForAttributeWithMetadataName(
                "Vogen.VogenDefaultsAttribute",
                predicate: (node, _) => node is CompilationUnitSyntax,
                transform: (ctx, _) => ManageAttributes.GetDefaultConfigFromGlobalAttribute(ctx))
            .Where(static m => m is not null)!;

        IncrementalValuesProvider<MarkerClassDefinition> converterMarkerClasses = syntaxProvider.CreateSyntaxProvider(
                predicate: (node, _) => ConversionMarkers.IsTarget(node),
                transform: (ctx, _) => ConversionMarkers.GetConversionMarkerClassDefinitionFromAttribute(ctx))
            .Where(static m => m is not null)!;

        return new Found(targets, globalConfig, converterMarkerClasses);
    }

    record struct Found(
        IncrementalValuesProvider<VoTarget> Vos,
        IncrementalValuesProvider<VogenConfigurationBuildResult> GlobalConfig,
        IncrementalValuesProvider<MarkerClassDefinition> ConverterMarkerClasses);
    
    private static void Execute(
        Compilation compilation,
        VogenKnownSymbols vogenKnownSymbols,
        ImmutableArray<VoTarget> targets,
        ImmutableArray<VogenConfigurationBuildResult> globalConfigBuildResult,
        ImmutableArray<MarkerClassDefinition> markerClasses,
        SourceProductionContext spc)
    {
        var csharpCompilation = compilation as CSharpCompilation;
        if (csharpCompilation is null) return;

        using var internalDiags = InternalDiagnostics.TryCreateIfSpecialClassIsPresent(compilation, spc, vogenKnownSymbols);
        internalDiags.IncrementGeneratedCount();

        internalDiags.RecordTargets(targets);

        var conversionMarkerErrors = markerClasses.SelectMany(x => x.Diagnostics);
        
        foreach (var diagnostic in conversionMarkerErrors)
        {
            spc.ReportDiagnostic(diagnostic);
        }
        
        // if there are some, get the default global config
        VogenConfigurationBuildResult buildResult = globalConfigBuildResult.IsDefaultOrEmpty
            ? VogenConfigurationBuildResult.Null
            : globalConfigBuildResult.ElementAt(0);

        foreach (var diagnostic in buildResult.Diagnostics)
        {
            spc.ReportDiagnostic(diagnostic);
        }
            
        VogenConfiguration? globalConfig = buildResult.ResultingConfiguration;
            
        internalDiags.RecordGlobalConfig(globalConfig);

        // get all the ValueObject types found.
        List<VoWorkItem> workItems = GetWorkItems(targets, spc, globalConfig, csharpCompilation.LanguageVersion, vogenKnownSymbols, compilation).ToList();
            
        GenerateCodeForOpenApiSchemaCustomization.WriteIfNeeded(globalConfig, spc, workItems, vogenKnownSymbols, compilation);

        GenerateCodeForEfCoreMarkers.Generate(spc, compilation, markerClasses);
        
        // the user can specify to create the MessagePack generated code as an attribute
        // or as marker in another project.
        GenerateCodeForMessagePack.GenerateForApplicableValueObjects(spc, compilation, workItems);
        GenerateCodeForMessagePackMarkers.GenerateForMarkerClasses(spc, compilation, markerClasses, vogenKnownSymbols);
        
        GenerateCodeForBsonSerializers.WriteIfNeeded(spc, compilation, workItems);
        
        GenerateCodeForOrleansSerializers.WriteIfNeeded(spc, workItems);

        if (workItems.Count > 0)
        {
            var mergedConfig = CombineConfigurations.CombineAndResolveAnyGlobalConfig(globalConfig);

            internalDiags.RecordResolvedGlobalConfig(mergedConfig);
                
            GenerateCodeForStaticAbstracts.WriteInterfacesAndMethodsIfNeeded(mergedConfig, spc, compilation);

            GenerateCodeForSystemTextJsonConverterFactories.WriteIfNeeded(mergedConfig, workItems, spc, compilation, vogenKnownSymbols);

            foreach (var eachWorkItem in workItems)
            {
                var parameters = new GenerationParameters(eachWorkItem, spc, vogenKnownSymbols, compilation);
                WriteWorkItems.WriteVo(parameters);
            }
        }
    }
        
    private static IEnumerable<VoWorkItem> GetWorkItems(ImmutableArray<VoTarget> targets,
        SourceProductionContext context,
        VogenConfiguration? globalConfig,
        LanguageVersion languageVersion,
        VogenKnownSymbols vogenKnownSymbols,
        Compilation compilation)
    {
        if (targets.IsDefaultOrEmpty)
        {
            yield break;
        }

        foreach (var eachTarget in targets)
        {
            if (eachTarget is null)
            {
                continue;
            }
                
            var ret = BuildWorkItems.TryBuild(eachTarget, context, globalConfig, languageVersion, vogenKnownSymbols, compilation);
                
            if (ret is not null)
            {
                yield return ret;
            }
        }
    }
}