using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared;
using VerifyXunit;
using Vogen;
using Xunit;

namespace LargeTests.GenerationTests;

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
            foreach (string type in _types)
            {
                foreach (string conversion in _conversions)
                {
                    foreach (string underlyingType in _underlyingTypes)
                    {
                        var qualifiedType = "public " + type;
                        yield return new object[] { qualifiedType, conversion, underlyingType, createClassName(qualifiedType, conversion, underlyingType, true), true };
                        yield return new object[] { qualifiedType, conversion, underlyingType, createClassName(qualifiedType, conversion, underlyingType, false), false };
                    }
                }
            }
        }

        private string createClassName(string type, string conversion, string underlyingType, bool customized) => 
            "stj_number_as_string_" + type.Replace(" ", "_") + conversion.Replace(".", "_").Replace("|", "_") + underlyingType + (customized ? "_customized" : "");

        private readonly string[] _types = new[]
        {
            "readonly partial struct",
            
            "partial class",

            "readonly partial record struct",
            
            "sealed partial record class",

            "partial record",
            "sealed partial record",
        };

        // for each of the types above, create classes for each one of these attributes
        private readonly string[] _conversions = new[]
        {
            "Conversions.None",
            "Conversions.NewtonsoftJson",
            "Conversions.SystemTextJson",
            "Conversions.NewtonsoftJson | Conversions.SystemTextJson"
        };

        // for each of the attributes above, use this underlying type
        private readonly string[] _underlyingTypes = new[]
        {
            "byte",
            "char",
            "bool",
            "System.DateTimeOffset",
            "System.DateTime",
            "decimal",
            "double",
            "float",
            "int",
            "System.Guid",
            "long",
            "short",
            "string",
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