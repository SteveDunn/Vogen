using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using Vogen;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotCompareWithPrimitivesInEfCoreAnalyzer>;

// ReSharper disable CoVariantArrayConversion

namespace AnalyzerTests;

public class DoNotCompareWithPrimitivesInEfCoreAnalyzerTests
{
    private const string _source =
        """
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using Microsoft.EntityFrameworkCore;
        using Microsoft.EntityFrameworkCore.ChangeTracking;
        using Microsoft.EntityFrameworkCore.ValueGeneration;
        using Vogen;
        
        [assembly: VogenDefaults(
        staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon | StaticAbstractsGeneration.InstanceMethodsAndProperties, 
          conversions: Conversions.EfCoreValueConverter)]

        namespace Whatever;

        public class EmployeeEntity
        {
            public Id Id { get; set; } = null!; // must be null in order for EF core to generate a value
            public required Name Name { get; set; } = Name.NotSet;
            public required Age Age { get; set; }
        }

        [ValueObject]
        public partial class Id;

        [ValueObject<string>]
        [Instance("NotSet", "[NOT_SET]")]
        public partial class Name;

        [ValueObject]
        public readonly partial struct Age;

        internal class DbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            public DbSet<EmployeeEntity> Entities { get; set; } = default!;
        
            protected override void OnModelCreating(ModelBuilder builder)
            {
                builder.Entity<EmployeeEntity>(b =>
                {
                    b.HasKey(x => x.Id);
                    
                    // There are two ways of registering these, you can do them inline here,
                    // or with the `RegisterAllIn[xxx]` like above in `ConfigureConventions`
                    
                    // b.Property(e => e.Id).HasVogenConversion();
                    // b.Property(e => e.Name).HasVogenConversion();
                });
            }
        }
        """;


    private static readonly Regex _placeholderPattern = new(@"{\|#\d+:", RegexOptions.Compiled);

    //No diagnostics expected to show up
    [Fact]
    public async Task NoDiagnosticsForEmptyCode()
    {
        var test = @"";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    public class NonQuerySyntax
    {
        [Fact]
        public async Task Triggers_when_found_in_IQueryableOfDbSet()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           using var ctx = new DbContext();
                                       
                                           var entities = ctx.Entities.Where(e => {|#0:e.Age == 50|});
                                       }
                                   }
                                   """;
            var sources = await CombineUserAndGeneratedSource(source);

            await Run(
                sources,
                WithDiagnostics("VOG034", DiagnosticSeverity.Error, "Age", 0));
        }

        [Fact]
        public async Task Triggers_when_found_in_IQueryableOfDbSet_in_separate_expression()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           using var ctx = new DbContext();
                                           
                                           DbSet<EmployeeEntity> step1 = ctx.Entities;
                                           var entities = step1.Where(e => {|#0:e.Age == 50|});
                                       }
                                   }
                                   """;
            var sources = await CombineUserAndGeneratedSource(source);

            await Run(
                sources,
                WithDiagnostics("VOG034", DiagnosticSeverity.Error, "Age", 0));
        }

        [Fact(Skip = "It would be nice if this did work, but I couldn't get it working. Please see the thread at https://github.com/SteveDunn/Vogen/issues/684")]
        public async Task Triggers_when_found_in_IQueryableOfDbSet_in_separate_expression_twice_removed()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           using var ctx = new DbContext();
                                           
                                           DbSet<EmployeeEntity> step1 = ctx.Entities;
                                           var step2 = step1.Take(4);
                                           var entities = step2.Where(e => {|#0:e.Age == 50|});
                                       }
                                   }
                                   """;
            var sources = await CombineUserAndGeneratedSource(source);

            await Run(
                sources,
                WithDiagnostics("VOG034", DiagnosticSeverity.Error, "Age", 0));
        }

        [Fact]
        public async Task Triggers_when_found_using_query_syntax()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           using var ctx = new DbContext();
                                           
                                           var entities = from e in ctx.Entities
                                           where {|#0:e.Age == 50|}
                                           select e;        
                                       }
                                   }
                                   """;
            var sources = await CombineUserAndGeneratedSource(source);

            await Run(
                sources,
                WithDiagnostics("VOG034", DiagnosticSeverity.Error, "Age", 0));
        }

        [Fact]
        public async Task Triggers_when_found_in_complex_IQueryableOfDbSet()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           using var ctx = new DbContext();
                                       
                                           var entities = ctx.Entities.Where(e => e != null && {|#0:e.Age == 50|});
                                       }
                                   }
                                   """;
            var sources = await CombineUserAndGeneratedSource(source);

            await Run(
                sources,
                WithDiagnostics("VOG034", DiagnosticSeverity.Error, "Age", 0));
        }

        [Fact]
        public async Task Triggers_when_found_in_complex_IQueryableOfDbSet_Single()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           using var ctx = new DbContext();
                                       
                                           var entity = ctx.Entities.Single(e => e != null && {|#0:e.Age == 50|});
                                       }
                                   }
                                   """;
            var sources = await CombineUserAndGeneratedSource(source);

            await Run(
                sources,
                WithDiagnostics("VOG034", DiagnosticSeverity.Error, "Age", 0));
        }

        [Fact]
        public async Task Not_triggered_when_found_in_non_IQueryableOfDbSet()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           var employees = new[] 
                                           { 
                                            new EmployeeEntity {Name = Name.From("Fred"),   Age = Age.From(50) }, 
                                            new EmployeeEntity {Name = Name.From("Barney"), Age = Age.From(42) }
                                            };
                                       
                                           var matching = employees.Where(e => e.Age == 50);
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
                //.WithNugetPackages(packages)
                .WithTargetFramework(TargetFramework.Net9_0)
                .GetGeneratedOutput<ValueObjectGenerator>(ignoreInitialCompilationErrors: true, peReference);

            if (output.Diagnostics.Length > 0)
            {
                throw new AssertFailedException(
                    $"""
                     Expected user source to be error and generated code to be free from errors:
                                                                     User source: {userSource}
                                                                     Errors: {string.Join(",", output.Diagnostics.Select(d => d.ToString()))}
                     """);
            }

            return [userSource, ..output.GeneratedSources.Select(o => o.ToString())];
        }
    }

    public class QuerySyntax
    {
        [Fact]
        public async Task Triggers_when_found_in_IQueryableOfDbSet()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           using var ctx = new DbContext();
                                       
                                           var entities = from e in ctx.Entities where {|#0:e.Age == 50|} select e;
                                       }
                                   }
                                   """;
            var sources = await CombineUserAndGeneratedSource(source);

            await Run(
                sources,
                WithDiagnostics("VOG034", DiagnosticSeverity.Error, "Age", 0));
        }

        [Fact]
        public async Task Triggers_when_found_in_complex_IQueryableOfDbSet()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           using var ctx = new DbContext();
                                       
                                           var entities = from e in ctx.Entities where e != null && {|#0:e.Age == 50|} select e;
                                       }
                                   }
                                   """;
            var sources = await CombineUserAndGeneratedSource(source);

            await Run(
                sources,
                WithDiagnostics("VOG034", DiagnosticSeverity.Error, "Age", 0));
        }

        [Fact]
        public async Task Triggers_when_found_in_complex_IQueryableOfDbSet_Single()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           using var ctx = new DbContext();
                                       
                                           var entity = (from e in ctx.Entities where e != null && {|#0:e.Age == 50|} select e).Single();
                                       }
                                   }
                                   """;
            var sources = await CombineUserAndGeneratedSource(source);

            await Run(
                sources,
                WithDiagnostics("VOG034", DiagnosticSeverity.Error, "Age", 0));
        }

        [Fact]
        public async Task Not_triggered_when_found_in_non_IQueryableOfDbSet()
        {
            var source = _source + """

                                   public static class Test
                                   {
                                       public static void FilterItems()
                                       {
                                           var employees = new[] 
                                           { 
                                            new EmployeeEntity {Name = Name.From("Fred"),   Age = Age.From(50) }, 
                                            new EmployeeEntity {Name = Name.From("Barney"), Age = Age.From(42) }
                                            };
                                       
                                           var matching = from e in employees where e.Age == 50 select e;
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
                //.WithNugetPackages(packages)
                .WithTargetFramework(TargetFramework.Net9_0)
                .GetGeneratedOutput<ValueObjectGenerator>(ignoreInitialCompilationErrors: true, peReference);

            if (output.Diagnostics.Length > 0)
            {
                throw new AssertFailedException(
                    $"""
                     Expected user source to be error and generated code to be free from errors:
                                                                     User source: {userSource}
                                                                     Errors: {string.Join(",", output.Diagnostics.Select(d => d.ToString()))}
                     """);
            }

            return [userSource, ..output.GeneratedSources.Select(o => o.ToString())];
        }
    }
}