using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

[Generator]
public class ValueObjectGenerator : IIncrementalGenerator
{
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Found found = GetTargets(context);

        IncrementalValueProvider<ImmutableArray<VoTarget>> collectedVos = found.Vos.Collect();
            
        var targetsAndConfig = collectedVos.Combine(found.GlobalConfig.Collect());
            
        var targetsConfigAndEfCoreSpecs = targetsAndConfig.Combine(found.EfCoreConverterSpecs.Collect());

        var compilationAndValues = context.CompilationProvider.Combine(targetsConfigAndEfCoreSpecs);
            
        context.RegisterSourceOutput(compilationAndValues,
            static (spc, source) => Execute(
                source.Left, 
                source.Right.Left.Left, 
                source.Right.Left.Right, 
                source.Right.Right,
                spc));
    }

    private static Found GetTargets(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<VoTarget> targets = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => VoFilter.IsTarget(node),
                transform: static (ctx, _) => VoFilter.TryGetTarget(ctx))
            .Where(static m => m is not null)!;

        IncrementalValuesProvider<VogenConfigurationBuildResult> globalConfig = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Vogen.VogenDefaultsAttribute",
                predicate: (node, _) => node is CompilationUnitSyntax,
                transform: (ctx, _) => ManageAttributes.GetDefaultConfigFromGlobalAttribute(ctx))
            .Where(static m => m is not null)!;

        IncrementalValuesProvider<EfCoreConverterMarkerClassResults> efCoreConverterSpecs = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Vogen.EfCoreConverterAttribute`1",
                predicate: (node, _) => node is ClassDeclarationSyntax,
                transform: (ctx, _) => ManageAttributes.GetEfCoreConverterSpecFromAttribute(ctx))
            .Where(static m => m is not null)!;

        return new Found(targets, globalConfig, efCoreConverterSpecs);
    }

    record struct Found(
        IncrementalValuesProvider<VoTarget> Vos,
        IncrementalValuesProvider<VogenConfigurationBuildResult> GlobalConfig,
        IncrementalValuesProvider<EfCoreConverterMarkerClassResults> EfCoreConverterSpecs);
    
    private static void Execute(
        Compilation compilation, 
        ImmutableArray<VoTarget> targets,
        ImmutableArray<VogenConfigurationBuildResult> globalConfigBuildResult,
        ImmutableArray<EfCoreConverterMarkerClassResults> efCoreConverterSpecs,
        SourceProductionContext spc)
    {
        using var internalDiags = InternalDiagnostics.TryCreateIfSpecialClassIsPresent(compilation, spc);
        internalDiags.IncrementGeneratedCount();

        internalDiags.RecordTargets(targets);

        var efSpecErrors = efCoreConverterSpecs.SelectMany(x => x.Diagnostics);
        
        foreach (var diagnostic in efSpecErrors)
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
        List<VoWorkItem> workItems = GetWorkItems(targets, spc, globalConfig, compilation).ToList();
            
        WriteOpenApiSchemaCustomizationCode.WriteIfNeeded(globalConfig, spc, compilation, workItems);

        WriteEfCoreSpecs.WriteIfNeeded(spc, compilation, efCoreConverterSpecs);
        
        WriteBsonSerializers.WriteIfNeeded(spc, compilation, workItems);
        
        WriteOrleansSerializers.WriteIfNeeded(spc, workItems);

        if (workItems.Count > 0)
        {
            var mergedConfig = CombineConfigurations.CombineAndResolveAnyGlobalConfig(globalConfig);

            internalDiags.RecordResolvedGlobalConfig(mergedConfig);
                
            WriteStaticAbstracts.WriteInterfacesAndMethodsIfNeeded(mergedConfig, spc, compilation);

            WriteSystemTextJsonConverterFactories.WriteIfNeeded(mergedConfig, workItems, spc, compilation);

            foreach (var eachWorkItem in workItems)
            {
                WriteWorkItems.WriteVo(eachWorkItem, spc);
            }
        }
    }
        
    private static IEnumerable<VoWorkItem> GetWorkItems(ImmutableArray<VoTarget> targets,
        SourceProductionContext context,
        VogenConfiguration? globalConfig,
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
                
            var ret = BuildWorkItems.TryBuild(eachTarget, context, globalConfig, compilation);
                
            if (ret is not null)
            {
                yield return ret;
            }
        }
    }
}