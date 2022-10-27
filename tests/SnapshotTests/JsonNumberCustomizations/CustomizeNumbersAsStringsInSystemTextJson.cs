using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.JsonNumberCustomizations;

/// <summary>
/// These tests verify that types containing <see cref="Customizations.TreatNumberAsStringInSystemTextJson"/> are written correctly.
/// </summary>
[UsesVerify] 
public class CustomizeNumbersAsStringsInSystemTextJson
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
                        var qualifiedType = "public " + type;
                        yield return new object[]
                        {
                            qualifiedType, conversion, underlyingType,
                            CreateClassName(qualifiedType, conversion, underlyingType, customized: true), true
                        };

                        yield return new object[]
                        {
                            qualifiedType, conversion, underlyingType,
                            CreateClassName(qualifiedType, conversion, underlyingType, customized: false), false
                        };
                    }
                }
            }
        }

        private static string CreateClassName(string type, string conversion, string underlyingType, bool customized) =>
            "stj_number_as_string_" + 
            type.Replace(" ", "_") + 
            conversion.Replace(".", "_").Replace("|", "_") +
            underlyingType + 
            (customized ? "_customized" : "");

        // for each of the types above, create classes for each one of these attributes
        private readonly string[] _conversions = new[]
        {
            "Conversions.None",
            "Conversions.NewtonsoftJson",
            "Conversions.SystemTextJson",
            "Conversions.NewtonsoftJson | Conversions.SystemTextJson"
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task GenerationTest(string type, string conversions, string underlyingType, string className, bool treatNumberAsString)
    {
        var customization = treatNumberAsString
            ? "Customizations.TreatNumberAsStringInSystemTextJson"
            : "Customizations.None";
        
        string declaration = $@"
  [ValueObject(conversions: {conversions}, underlyingType: typeof({underlyingType}), customizations: {customization})]
  {type} {className} {{ }}";
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseFileName(className))
            .RunOnAllFrameworks();
    }
}