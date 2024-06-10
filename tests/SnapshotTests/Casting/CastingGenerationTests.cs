using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.Casting;

[UsesVerify] 
public class CastingGenerationTests
{
    public class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in Factory.TypeVariations)
            {
                foreach ((string toPrimitive, string fromPrimitive) eachCast in _casts)
                {
                    foreach (string underlyingType in Factory.UnderlyingTypes)
                    {
                        var qualifiedType = "public " + type;
                        yield return
                        [
                            qualifiedType, "CastOperator."+eachCast.toPrimitive, "CastOperator."+eachCast.fromPrimitive, underlyingType,
                            CreateClassName(qualifiedType, eachCast, underlyingType)
                        ];
                    }
                }
            }
        }

        private static string CreateClassName(string type, (string toPrimitive, string fromPrimitive) casts, string underlyingType) =>
            Normalize($"_casting_{type}{casts.fromPrimitive}{casts.toPrimitive}{underlyingType}");

        private static string Normalize(string input) => input.Replace(" ", "_").Replace("|", "_").Replace(".", "_");

        // for each of the types above, create classes for each one of these attributes
        private readonly (string, string)[] _casts =
        [
            ("None",     "None"),
            ("None",     "Implicit"),
            ("None",     "Explicit"),
            ("Implicit", "None"),
            ("Implicit", "Implicit"),
            ("Implicit", "Explicit"),
            ("Explicit", "None"),
            ("Explicit", "Implicit"),
            ("Explicit", "Explicit")
        ];

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task Can_specify_casting_at_the_value_object_level(string type, string toPrimitiveCast, string fromPrimitivePrimitiveCast, string underlyingType, string className)
    {
        string declaration;
        
        if (underlyingType.Length == 0)
        {
            declaration = $$"""
                              [ValueObject(toPrimitiveCasting: {{toPrimitiveCast}}, fromPrimitiveCasting: {{fromPrimitivePrimitiveCast}})]
                              {{type}} {{className}} { }
                            """;
        }
        else
        {
            declaration = $$"""
                              [ValueObject(toPrimitiveCasting: {{toPrimitiveCast}}, fromPrimitiveCasting: {{fromPrimitivePrimitiveCast}}, underlyingType: typeof({{underlyingType}}))]
                              {{type}} {{className}} { }
                            """;

        }

        var source = $"""
                      using Vogen;
                      namespace Whatever;

                      {declaration}
                      """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseHashedParameters(type, toPrimitiveCast, fromPrimitivePrimitiveCast, underlyingType, className))
            .RunOnAllFrameworks();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task Can_specify_casting_at_the_global_config_level(string type, string toPrimitiveCast, string fromPrimitivePrimitiveCast, string underlyingType, string className)
    {
        string declaration;
        
        if (underlyingType.Length == 0)
        {
            declaration = $$"""
                              [ValueObject]
                              {{type}} {{className}} { }
                            """;
        }
        else
        {
            declaration = $$"""
                              [ValueObject(underlyingType: typeof({{underlyingType}}))]
                              {{type}} {{className}} { }
                            """;

        }

        var source = $"""
                      using Vogen;
                      [assembly: VogenDefaults(toPrimitiveCasting: {toPrimitiveCast}, fromPrimitiveCasting: {fromPrimitivePrimitiveCast})]

                      namespace Whatever;

                      {declaration}
                      """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseHashedParameters(type, toPrimitiveCast, fromPrimitivePrimitiveCast, underlyingType, className))
            .RunOnAllFrameworks();
    }
}