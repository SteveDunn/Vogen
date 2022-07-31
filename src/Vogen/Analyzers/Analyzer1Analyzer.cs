using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vogen.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Analyzer1Analyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Analyzer1";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle),
            Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager,
                typeof(Resources));

        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

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

            if (!Util.IsVoType(namedTypeSymbol))
            {
                return;
            }

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

            // Find just those named type symbols with names containing lowercase letters.
            //if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            if (!found)
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(
                    descriptor: Rule,
                    location: namedTypeSymbol.Locations[0],
                    //effectiveSeverity: DiagnosticSeverity.Info, 
                    additionalLocations: null,
                    properties: null,
                    namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsMethodStatic(IMethodSymbol mds) => mds.IsStatic;

    }
}
