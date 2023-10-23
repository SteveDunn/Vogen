using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.CastingOperators;

/// <summary>
/// These tests verify that types containing <see cref="Customizations.TreatNumberAsStringInSystemTextJson"/> are written correctly.
/// </summary>
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
                        yield return new object[]
                        {
                            qualifiedType, eachCast.toPrimitive, eachCast.fromPrimitive, underlyingType,
                            CreateClassName(qualifiedType, eachCast, underlyingType)
                        };
                    }
                }
            }
        }

        private static string CreateClassName(string type, (string toPrimitive, string fromPrimitive) casts, string underlyingType) =>
            Normalize($"_casting_{type}{casts.fromPrimitive}{casts.toPrimitive}{underlyingType}");

        private static string Normalize(string input) => input.Replace(" ", "_").Replace("|", "_").Replace(".", "_");

        // for each of the types above, create classes for each one of these attributes
        private readonly (string, string)[] _casts = new[]
        {
            ("CastOperator.None", "CastOperator.None"),
            ("CastOperator.None", "CastOperator.Implicit"),
            ("CastOperator.None", "CastOperator.Explicit"),
            ("CastOperator.Implicit", "CastOperator.None"),
            ("CastOperator.Implicit", "CastOperator.Implicit"),
            ("CastOperator.Implicit", "CastOperator.Explicit"),
            ("CastOperator.Explicit", "CastOperator.None"),
            ("CastOperator.Explicit", "CastOperator.Implicit"),
            ("CastOperator.Explicit", "CastOperator.Explicit")
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task Tests(string type, string toPrimitiveCast, string fromPrimitivePrimitiveCast, string underlyingType, string className)
    {
        string declaration = "";
        
        if (underlyingType.Length == 0)
        {
            declaration = $@"
  [ValueObject(toPrimitiveCasting: {toPrimitiveCast}, fromPrimitiveCasting: {fromPrimitivePrimitiveCast})]
  {type} {className} {{ }}";
        }
        else
        {
            declaration = $@"
  [ValueObject(toPrimitiveCasting: {toPrimitiveCast}, fromPrimitiveCasting: {fromPrimitivePrimitiveCast}, underlyingType: typeof({underlyingType}))]
  {type} {className} {{ }}";

        }

        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(className)))
            .RunOnAllFrameworks();
    }
}