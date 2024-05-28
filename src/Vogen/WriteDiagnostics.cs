using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Vogen;

internal interface IInternalDiagnostics : IDisposable
{
    void RecordGlobalConfig(VogenConfiguration? globalConfig);

    void RecordTargets(ImmutableArray<VoTarget> targets);
    
    void RecordResolvedGlobalConfig(VogenConfiguration mergedConfig);
}

internal class InternalDiagnostics : IInternalDiagnostics
{
    private readonly bool _ignore;
    private VogenConfiguration? _userProvidedGlobalConfig;
    private VogenConfiguration? _resolvedGlobalConfig;
    private readonly Compilation _compilation;
    private readonly SourceProductionContext _context;
    private ImmutableArray<VoTarget>? _targets;

    private static readonly IInternalDiagnostics _nullImplementation = new NullInternalDiagnostics();

    public static IInternalDiagnostics TryCreateIfSpecialClassIsPresent(Compilation compilation, SourceProductionContext context)
        => compilation.GetTypeByMetadataName("Vogen.__ProduceDiagnostics") is null
            ? _nullImplementation
            : new InternalDiagnostics(compilation, context);

    private InternalDiagnostics(Compilation compilation, SourceProductionContext context)
    {
        _compilation = compilation;
        _context = context;
        _ignore = compilation.GetTypeByMetadataName("Vogen.__ProduceDiagnostics") is null;
    }
    
    public void RecordGlobalConfig(VogenConfiguration? globalConfig) => _userProvidedGlobalConfig = _ignore ? null : globalConfig;

    public void Dispose() => WriteIfNeeded();

    private void WriteIfNeeded()
    {
        if (_ignore)
        {
            return;
        }
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("/*").AppendLine();
        sb.AppendLine($"LanguageVersion: {(_compilation as CSharpCompilation)?.LanguageVersion.ToString() ?? "not C#!"}");
        ReportConfig(sb, "User provided global config", _userProvidedGlobalConfig);
        ReportConfig(sb, "Resolved global config", _resolvedGlobalConfig);
        ReportTargets(sb);
        sb.AppendLine().AppendLine("*/");

        _context.AddSource("diagnostics.cs", sb.ToString());
    }

    private void ReportTargets(StringBuilder sb)
    {
        AppendSectionName("Targets", sb);
        
        if (_targets is null || _targets.Value.IsDefaultOrEmpty)
        {
            sb.AppendLine("No targets found");
            return;
        }
        
        foreach (var eachTarget in _targets)
        {
            sb.AppendLine($"{eachTarget.VoSymbolInformation.ToString()}");
        }
    }

    private static void AppendSectionName(string sectionName, StringBuilder sb) => 
        sb.AppendLine().AppendLine(sectionName).AppendLine("===========");

    private static void ReportConfig(StringBuilder sb, string sectionName, VogenConfiguration? c)
    {
        AppendSectionName(sectionName, sb);
        
        if (c is null)
        {
            sb.AppendLine("Not provided");
            return;
        }

        sb.AppendLine($"UnderlyingType: {c.UnderlyingType?.ToDisplayString() ?? "null"}")
            .AppendLine($"ValidationExceptionType: {c.ValidationExceptionType?.ToDisplayString() ?? "null"}")
            .AppendLine($"Conversions: {c.Conversions.ToString()}")
            .AppendLine($"Customizations: {c.Customizations.ToString()}")
            .AppendLine($"DeserializationStrictness: {c.DeserializationStrictness.ToString()}")
            .AppendLine($"DebuggerAttributes: {c.DebuggerAttributes.ToString()}")
            .AppendLine($"Comparison: {c.Comparison.ToString()}")
            .AppendLine($"StringComparers: {c.StringComparers.ToString()}")
            .AppendLine($"ParsableForStrings: {c.ParsableForStrings.ToString()}")
            .AppendLine($"ParsableForPrimitives: {c.ParsableForPrimitives.ToString()}")
            .AppendLine($"ToPrimitiveCasting: {c.ToPrimitiveCasting.ToString()}")
            .AppendLine($"FromPrimitiveCasting: {c.FromPrimitiveCasting.ToString()}")
            .AppendLine($"DisableStackTraceRecordingInDebug: {c.DisableStackTraceRecordingInDebug.ToString()}")
            .AppendLine($"TryFromGeneration: {c.TryFromGeneration.ToString()}")
            .AppendLine($"IsInitializedMethodGeneration: {c.IsInitializedMethodGeneration.ToString()}")
            .AppendLine($"SystemTextJsonConverterFactoryGeneration: {c.SystemTextJsonConverterFactoryGeneration.ToString()}")
            .AppendLine($"OpenApiSchemaCustomizations: {c.OpenApiSchemaCustomizations.ToString()}")
            .AppendLine($"StaticAbstractsGeneration: {c.StaticAbstractsGeneration.ToString()}");
    }

    public void RecordTargets(ImmutableArray<VoTarget> targets) => _targets = targets;

    public void RecordResolvedGlobalConfig(VogenConfiguration mergedConfig) => _resolvedGlobalConfig = _ignore ? null : mergedConfig;
}

internal class NullInternalDiagnostics : IInternalDiagnostics
{
    public void Dispose() { }

    public void RecordGlobalConfig(VogenConfiguration? globalConfig)
    {
    }

    public void RecordTargets(ImmutableArray<VoTarget> targets) 
    {
    }

    public void RecordResolvedGlobalConfig(VogenConfiguration mergedConfig)
    {
    }
}
