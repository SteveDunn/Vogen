using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Vogen.Diagnostics;

namespace Vogen.Rules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddValidationAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.AddValidationAnalyzerTitle),
            Resources.ResourceManager, 
            typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.AddValidationAnalyzerMessageFormat), 
            Resources.ResourceManager,
            typeof(Resources));
        
        private static readonly LocalizableString Description =new LocalizableResourceString(
            nameof(Resources.AddValidationAnalyzerDescription), 
            Resources.ResourceManager,
            typeof(Resources));
        
        // private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            RuleIdentifiers.AddValidationMethod,
            Title,
            MessageFormat,
            RuleCategories.Usage,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var namedTypeSymbol = (INamedTypeSymbol) context.Symbol;

            var ulType = TryResolveUnderlyingType(namedTypeSymbol, context);

            if (ulType is null) return;

            if (AlreadyContainsAValidationMethod(namedTypeSymbol))
            {
                return;
            }

            Dictionary<string, string?> properties = new()
            {
                { "PrimitiveType", ulType.Name}
            };

            var diagnostic = Diagnostic.Create(
                descriptor: Rule,
                location: namedTypeSymbol.Locations[0],
                additionalLocations: null,
                properties: properties.ToImmutableDictionary(),
                namedTypeSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }

        private static INamedTypeSymbol? TryResolveUnderlyingType(INamedTypeSymbol namedTypeSymbol, SymbolAnalysisContext context)
        {
            INamedTypeSymbol voSymbolInformation = namedTypeSymbol;

            var attrs = VoFilter.TryGetValueObjectAttributes(voSymbolInformation).ToImmutableArray();
            
            if (attrs.Length != 1) return null;

            VogenConfigurationBuildResult buildResult = BuildConfigurationFromAttributes.TryBuildFromValueObjectAttribute(attrs[0]);
            
            VogenConfiguration? vogenConfig = buildResult.ResultingConfiguration;
            
            if (!vogenConfig.HasValue) return null;
            
            if (buildResult.HasDiagnostics)
            {
                return null;
            }

            INamedTypeSymbol? ulType = vogenConfig.Value.UnderlyingType;

            return ulType ?? 
                   TryGetFromGlobalAttribute(context.Compilation) ?? 
                   context.Compilation.GetSpecialType(SpecialType.System_Int32);
        }

        private static INamedTypeSymbol? TryGetFromGlobalAttribute(Compilation compilation)
        {
            var v = ManageAttributes.GetDefaultConfigFromGlobalAttribute(compilation);
            
            return v.ResultingConfiguration?.UnderlyingType;
        }

        private static bool AlreadyContainsAValidationMethod(INamedTypeSymbol namedTypeSymbol)
        {
            bool found = false;

            ImmutableArray<ISymbol> members = namedTypeSymbol.GetMembers("Validate");

            foreach (ISymbol? eachMemberSyntax in members)
            {
                if (eachMemberSyntax is not IMethodSymbol mds)
                {
                    continue;
                }

                if (!IsMethodStatic(mds))
                {
                    continue;
                }

                if (mds.ReturnType is not INamedTypeSymbol s || s.FullName() != "Vogen.Validation")
                {
                    continue;
                }

                found = true;
                break;
            }

            return found;
        }

        private static bool IsMethodStatic(IMethodSymbol mds) => mds.IsStatic;
    }
}
