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
        
        private const string Category = "Usage";

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

            INamedTypeSymbol voSymbolInformation = namedTypeSymbol;

            var attrs = VoFilter.TryGetValueObjectAttributes(voSymbolInformation).ToImmutableArray();

            if (attrs.Length != 1) return;

            VogenConfigurationBuildResult buildResult = GlobalConfigFilter.TryBuildConfigurationFromAttribute(attrs[0]);
            
            VogenConfiguration? vogenConfig = buildResult.ResultingConfiguration;
            
            if (!vogenConfig.HasValue) return;
            
            if (buildResult.Diagnostics.Count > 0)
            {
                return;
            }

            string retType = vogenConfig.Value.UnderlyingType!.Name;

            var voTypeSyntax = namedTypeSymbol;

            bool found = false;

            foreach (ISymbol? memberDeclarationSyntax in voTypeSyntax.GetMembers("Validate"))
            {
                if (memberDeclarationSyntax is IMethodSymbol mds)
                {
                    if (!IsMethodStatic(mds))
                    {
                        continue;
                    }

                    if (mds.ReturnType.Name == "Validation")
                    {
                        found = true;
                        break;
                    }
                }
            }

            if (found)
            {
                return;
            }

            Dictionary<string, string?> properties = new()
            {
                { "PrimitiveType", retType}
            };

            var diagnostic = Diagnostic.Create(
                descriptor: Rule,
                location: namedTypeSymbol.Locations[0],
                additionalLocations: null,
                properties: properties.ToImmutableDictionary(),
                namedTypeSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsMethodStatic(IMethodSymbol mds) => mds.IsStatic;
    }
}
