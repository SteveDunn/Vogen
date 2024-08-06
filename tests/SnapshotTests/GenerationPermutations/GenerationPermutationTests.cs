using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vogen;
using Xunit.Abstractions;

namespace SnapshotTests.GenerationPermutations;

public class GenerationPermutationTests
{
    private readonly ITestOutputHelper _logger;

    public GenerationPermutationTests(ITestOutputHelper logger) => _logger = logger;

    public class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in Factory.TypeVariations)
            {
                string conversion =
                    "Conversions.NewtonsoftJson | Conversions.SystemTextJson | Conversions.EfCoreValueConverter | Conversions.DapperTypeHandler | Conversions.LinqToDbValueConverter";
                foreach (string underlyingType in Factory.UnderlyingTypes)
                {
                    foreach (string accessModifier in _accessModifiers)
                    {
                        var qualifiedType = $"{accessModifier} {type}";
                        yield return
                        [
                            qualifiedType,
                            conversion,
                            underlyingType,
                            CreateClassName(qualifiedType, conversion, underlyingType)
                        ];
                    }
                }
            }
        }

        private static string CreateClassName(string type, string conversion, string underlyingType) =>
            Normalize($"{type}{conversion}{underlyingType}");

        private static string Normalize(string input) => input.Replace(" ", "_").Replace("|", "_").Replace(".", "_").Replace("@", "_");

        private readonly string[] _accessModifiers =
        {
            "public"
#if THOROUGH
            , "internal"
#endif
        };
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private Task Run(string type, string conversions, string underlyingType, string className, string locale)
    {
        _logger.WriteLine($"Running permutation, type: {type}, conversions: {conversions}, underlyingType: {underlyingType}, className: {className}, locale: {locale}");

        string declaration;

        if (underlyingType.Length == 0)
        {
            declaration = $$"""

                            [ValueObject(conversions: {{conversions}})]
                            {{type}} {{className}} { }
                            """;
        }
        else
        {
            declaration = $$"""

                            [ValueObject(conversions: {{conversions}}, underlyingType: typeof({{underlyingType}}))]
                            {{type}} {{className}} { }
                            """;
        }

        var source = $$"""
                       using Vogen;
                       namespace Whatever
                       {
                           {{declaration}}
                       }
                       """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithLogger(_logger)
            .WithSource(source)
            .WithLocale(locale)
            .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(className)))
            .RunOnAllFrameworks();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task GenerationTest(string type, string conversions, string underlyingType, string className) => Run(
        type,
        conversions,
        underlyingType,
        className,
        string.Empty);

#if THOROUGH
    [Theory]
    [UseCulture("fr-FR")]
    [ClassData(typeof(Types))]
    public Task GenerationTests_FR(string type, string conversions, string underlyingType, string className) => Run(
        type,
        conversions,
        underlyingType,
        className,
        "fr");

    [Theory]
    [UseCulture("en-US")]
    [ClassData(typeof(Types))]
    public Task GenerationTests_US(string type, string conversions, string underlyingType, string className) => Run(
        type,
        conversions,
        underlyingType,
        className,
        "us");
#endif
}