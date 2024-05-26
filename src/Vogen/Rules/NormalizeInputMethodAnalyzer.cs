using System.Collections.Generic;
using System.Collections.Immutable;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Vogen.Diagnostics;

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NormalizeInputMethodAnalyzer : DiagnosticAnalyzer
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it won't pick this up when implied
    private static readonly DiagnosticDescriptor _ruleNotFound = new DiagnosticDescriptor(
        RuleIdentifiers.AddNormalizeInputMethod,
        "Value Objects can have a method that normalizes (sanitizes) input",
        "Type name '{0}' can have a method that normalizes (sanitizes) input",
        RuleCategories.Usage,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Value objects can have a method that normalizes (sanitizes) input.");

    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it won't pick this up when implied
    public static readonly DiagnosticDescriptor RuleNotStatic = new DiagnosticDescriptor(
        RuleIdentifiers.AddStaticToExistingNormalizeInputMethod,
        "NormalizeInput methods should be static",
        "Type name '{0}' should have a static NormalizeInput method",
        RuleCategories.Usage,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Value objects should have static NormalizeInputMethod methods.");

    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it won't pick this up when implied
    public static readonly DiagnosticDescriptor RuleWrongInputType = new DiagnosticDescriptor(
        RuleIdentifiers.IncorrectUseOfNormalizeInputMethod,
        "NormalizeInput methods should take the same primitive type as the value object",
        "Type name '{0}' should have a NormalizeInput method that takes the same primitive type as the value object",
        RuleCategories.Usage,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Value objects should have a NormalizeInput method that takes the same primitive as the thing it wraps.");


    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(_ruleNotFound, RuleNotStatic, RuleWrongInputType);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var namedTypeSymbol = (INamedTypeSymbol) context.Symbol;

        INamedTypeSymbol? ulType = TryResolveUnderlyingType(namedTypeSymbol, context);

        if (ulType is null) return;

        (MethodInspectionResult, Location) findResult = InspectNormalizeInputMethod(namedTypeSymbol, ulType);

        MethodInspectionResult inspectionResult = findResult.Item1;
        
        if (inspectionResult == MethodInspectionResult.Pristine)
        {
            return;
        }

        Dictionary<string, string?> properties = new()
        {
            { "PrimitiveType", ulType.ToString()},
            { "InspectionResult", inspectionResult.ToString()}
        };

        List<DiagnosticDescriptor> l = [];
        if(inspectionResult.HasFlag(MethodInspectionResult.Missing)) l.Add(_ruleNotFound);
        if(inspectionResult.HasFlag(MethodInspectionResult.NotStatic)) l.Add(RuleNotStatic);
        if(inspectionResult.HasFlag(MethodInspectionResult.WrongInputType)) l.Add(RuleWrongInputType);

        foreach (var eachDescriptor in l)
        {
            var diagnostic = Diagnostic.Create(
                eachDescriptor,
                findResult.Item2,
                null,
                properties.ToImmutableDictionary(),
                namedTypeSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }    }

    private static INamedTypeSymbol? TryResolveUnderlyingType(INamedTypeSymbol namedTypeSymbol, SymbolAnalysisContext context)
    {
        INamedTypeSymbol voSymbolInformation = namedTypeSymbol;

        var attrs = VoFilter.TryGetValueObjectAttributes(voSymbolInformation).ToImmutableArray();
            
        if (attrs.Length != 1)
        {
            return null;
        }

        VogenConfigurationBuildResult buildResult = BuildConfigurationFromAttributes.TryBuildFromValueObjectAttribute(attrs[0]);
            
        VogenConfiguration? vogenConfig = buildResult.ResultingConfiguration;
            
        if (vogenConfig is null)
        {
            return null;
        }

        if (buildResult.HasDiagnostics)
        {
            return null;
        }

        INamedTypeSymbol? ulType = vogenConfig.UnderlyingType;

        return ulType ?? 
               TryGetFromGlobalAttribute(context.Compilation) ?? 
               context.Compilation.GetSpecialType(SpecialType.System_Int32);
    }

    private static INamedTypeSymbol? TryGetFromGlobalAttribute(Compilation compilation)
    {
        var v = ManageAttributes.GetDefaultConfigFromGlobalAttribute(compilation);
            
        return v.ResultingConfiguration?.UnderlyingType;
    }

    private static (MethodInspectionResult, Location) InspectNormalizeInputMethod(INamedTypeSymbol symbolForType, INamedTypeSymbol ulType)
    {
        ImmutableArray<ISymbol> members = symbolForType.GetMembers("NormalizeInput");

        foreach (ISymbol? eachMemberSyntax in members)
        {
            if (eachMemberSyntax is not IMethodSymbol mds)
            {
                continue;
            }

            MethodInspectionResult r = MethodInspectionResult.Pristine;

            if (!IsMethodStatic(mds)) r |= MethodInspectionResult.NotStatic;
            if (!DoesMethodTakeCorrectInputType(mds, ulType)) r |= MethodInspectionResult.WrongInputType;

            return (r, mds.Locations[0]);
        }

        return (MethodInspectionResult.Missing, symbolForType.Locations[0]);
    }

    private static bool IsMethodStatic(IMethodSymbol mds) => mds.IsStatic;
    
    private static bool DoesMethodTakeCorrectInputType(IMethodSymbol mds, ISymbol? primitiveType)
    {
        if (mds.Parameters.Length != 1) return true;
        return mds.Parameters[0].Type.IsEqualTo(primitiveType);
    }    
}