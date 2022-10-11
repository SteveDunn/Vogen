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
        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AddValidationAnalyzerTitle),
        Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.AddValidationAnalyzerMessageFormat), Resources.ResourceManager,
        typeof(Resources));
        private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.AddValidationAnalyzerDescription), Resources.ResourceManager,
        typeof(Resources));
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(RuleIdentifiers.AddValidationMethod, Title, MessageFormat,
            RuleCategories.Usage, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {

            //context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol) context.Symbol;

            INamedTypeSymbol voSymbolInformation = namedTypeSymbol;

            var attrs = VoFilter.TryGetValueObjectAttributes(voSymbolInformation).ToImmutableArray();

            if (attrs.Length != 1) return;

            VogenConfigurationBuildResult x = GlobalConfigFilter.BuildConfigurationFromAttribute(attrs[0]);
            VogenConfiguration? vogenConfig = x.ResultingConfiguration;
            if (!vogenConfig.HasValue) return;
            if (x.Diagnostics.Count > 0) return;

            string retType = vogenConfig.Value.UnderlyingType!.Name;//..Value?.ToString() ?? "int";


            // if (!VoFilter.IsTarget(namedTypeSymbol))
            // {
            //     return;
            // }
            //
            // ImmutableArray<AttributeData> attributes = voSymbolInformation.GetAttributes();
            //
            // if (attributes.Length == 0)
            // {
            //     return;
            // }
            //
            // AttributeData? attr = attributes.SingleOrDefault(a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");
            //
            // if (attr is null)
            // {
            //     return;
            // }

            // var localConfig = GlobalConfigFilter.BuildConfigurationFromAttributeWithoutAnyDiagnosticErrors(voAttribute);
            //
            // if (localConfig == null)
            // {
            //     return;
            // }
            //
            var voTypeSyntax = namedTypeSymbol;

            // if(attr.ConstructorArguments.Length == 0) return;
            //
            // // the first argument is the type - if it's null, then then it's an int
            // TypedConstant firstArg = attr.ConstructorArguments[0];
            //
            // if (firstArg.Kind != TypedConstantKind.Type) return;
            //

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
