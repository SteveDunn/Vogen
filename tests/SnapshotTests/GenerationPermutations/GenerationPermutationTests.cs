using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.GenerationPermutations;

[UsesVerify] 
public class GenerationPermutationTests
{
    public class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in Factory.TypeVariations)
            {
                foreach (string conversion in _conversions)
                {
                    foreach (string underlyingType in Factory.UnderlyingTypes)
                    {
                        foreach (string accessModifier in _accessModifiers)
                        {
                            var qualifiedType = $"{accessModifier} {type}";
                            yield return new object[]
                            {
                                qualifiedType, 
                                conversion, 
                                underlyingType,
                                CreateClassName(qualifiedType, conversion, underlyingType)
                            };
                        }
                    }
                }
            }
        }

        private static string CreateClassName(string type, string conversion, string underlyingType) => 
            type.Replace(" ", "_") + conversion.Replace(".", "_").Replace("|", "_") + underlyingType;

        private readonly string[] _accessModifiers =
        {
            "public"
#if THOROUGH
            , "internal"
#endif
        };
        
        // for each of the types above, create classes for each one of these attributes
        private readonly string[] _conversions = 
        {
            "Conversions.None",
            "Conversions.TypeConverter",
            "Conversions.NewtonsoftJson",
            "Conversions.SystemTextJson",
#if THOROUGH
            "Conversions.NewtonsoftJson | Conversions.SystemTextJson",
            "Conversions.EfCoreValueConverter",
            "Conversions.DapperTypeHandler",
            "Conversions.LinqToDbValueConverter",
#endif
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private Task Run(string type, string conversions, string underlyingType, string className, string locale)
    {
        string declaration = $@"
  [ValueObject(conversions: {conversions}, underlyingType: typeof({underlyingType}))]
  {type} {className} {{ }}";
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        return new SnapshotRunner<ValueObjectGenerator>()
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