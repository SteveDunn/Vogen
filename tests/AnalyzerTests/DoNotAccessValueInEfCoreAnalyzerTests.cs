using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Shared;
using Vogen;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotAccessValueInEfCoreAnalyzer>;

namespace AnalyzerTests;

public class DoNotAccessValueInEfCoreAnalyzerTests
{
    private const string _source =
        """
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using Microsoft.EntityFrameworkCore;
        using Vogen;
        
        [assembly: VogenDefaults(conversions: Conversions.EfCoreValueConverter, openApiSchemaCustomizations: OpenApiSchemaCustomizations.Omit)]

        namespace Whatever;

        public class EmployeeEntity
        {
            public Id Id { get; set; } = null!;
            public required Age Age { get; set; }
        }

        [ValueObject]
        public partial class Id;

        [ValueObject]
        public readonly partial struct Age;

        internal class DbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            public DbSet<EmployeeEntity> Entities { get; set; } = default!;
        }
        """;

    private static readonly Regex _placeholderPattern = new(@"{\|#\d+:", RegexOptions.Compiled);

    [Fact]
    public async Task NoDiagnosticsForEmptyCode()
    {
        var test = @"";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task Triggers_when_Value_is_accessed_in_Where()
    {
        var source = _source + """

                               public static class Test
                               {
                                   public static void FilterItems()
                                   {
                                       using var ctx = new DbContext();
                                       var entities = ctx.Entities.Where(e => {|#0:e.Age.Value|} == 50);
                                   }
                               }
                               """;
        var sources = await CombineUserAndGeneratedSource(source);

        await Run(
            sources,
            WithDiagnostics("VOG039", DiagnosticSeverity.Warning, "Age", 0));
    }
    [Fact]
    public async Task Triggers_when_Value_is_accessed_in_chained_Where()
    {
        var source = _source + """

                               public static class Test
                               {
                                   public static void FilterItems()
                                   {
                                       using var ctx = new DbContext();
                                       var entities = ctx.Entities.Where(x => true).Where(e => {|#0:e.Age.Value|} == 50);
                                   }
                               }
                               """;
        var sources = await CombineUserAndGeneratedSource(source);

        await Run(
            sources,
            WithDiagnostics("VOG039", DiagnosticSeverity.Warning, "Age", 0));
    }

    

    [Fact]
    public async Task Triggers_when_Value_is_accessed_in_QuerySyntax()
    {
        var source = _source + """

                               public static class Test
                               {
                                   public static void FilterItems()
                                   {
                                       using var ctx = new DbContext();
                                       var entities = from e in ctx.Entities
                                                      where {|#0:e.Age.Value|} == 50
                                                      select e;
                                   }
                               }
                               """;
        var sources = await CombineUserAndGeneratedSource(source);

        await Run(
            sources,
            WithDiagnostics("VOG039", DiagnosticSeverity.Warning, "Age", 0));
    }

    [Fact]
    public async Task Not_triggered_when_found_in_non_IQueryableOfDbSet()
    {
        var source = _source + """

                               public static class Test
                               {
                                   public static void FilterItems()
                                   {
                                       var employees = new List<EmployeeEntity>();
                                       var matching = employees.Where(e => e.Age.Value == 50);
                                   }
                               }
                               """;
        var sources = await CombineUserAndGeneratedSource(source);

        await Run(sources, Enumerable.Empty<DiagnosticResult>());
    }

    private static IEnumerable<DiagnosticResult> WithDiagnostics(string code,
        DiagnosticSeverity severity,
        string arguments,
        params int[] locations)
    {
        foreach (var location in locations)
        {
            yield return VerifyCS.Diagnostic(code).WithSeverity(severity).WithLocation(location)
                .WithArguments(arguments);
        }
    }

    private async Task Run(string[] sources, IEnumerable<DiagnosticResult> expected)
    {
        var test = new VerifyCS.Test
        {
            CompilerDiagnostics = CompilerDiagnostics.Errors,
            ReferenceAssemblies = References.Net90WithEfCoreAndOurs.Value,
        };

        foreach (var eachSource in sources)
        {
            test.TestState.Sources.Add(eachSource);
        }

        test.ExpectedDiagnostics.AddRange(expected);

        await test.RunAsync();
    }

    private static async Task<string[]> CombineUserAndGeneratedSource(string userSource)
    {
        PortableExecutableReference peReference = MetadataReference.CreateFromFile(typeof(ValueObjectAttribute).Assembly.Location);
        var strippedSource = _placeholderPattern.Replace(userSource, string.Empty).Replace("|}", string.Empty);

        (ImmutableArray<Diagnostic> Diagnostics, SyntaxTree[] GeneratedSources) output = await new ProjectBuilder()
            .WithUserSource(strippedSource)
            .WithTargetFramework(TargetFramework.Net9_0)
            .GetGeneratedOutput<ValueObjectGenerator>(ignoreInitialCompilationErrors: true, peReference);

        if (output.Diagnostics.Length > 0)
        {
            throw new Xunit.Sdk.XunitException(
                $"Expected user source to be error and generated code to be free from errors:\nErrors: {string.Join(",", output.Diagnostics.Select(d => d.ToString()))}");
        }

        return [userSource, .. output.GeneratedSources.Select(o => o.ToString())];
    }
}
