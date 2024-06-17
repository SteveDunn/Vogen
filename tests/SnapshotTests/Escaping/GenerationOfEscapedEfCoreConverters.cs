using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.Escaping;

[UsesVerify]
public class GenerationOfEscapedEfCoreConverters
{
    public class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in Factory.TypeVariations)
            {
                foreach (string conversion in _conversions)
                {
                    foreach (string underlyingType in _underlyingTypes)
                    {
                        var qualifiedType = "public " + type;
                        yield return
                        [
                            qualifiedType, conversion, underlyingType,
                            CreateClassName(qualifiedType, conversion, underlyingType)
                        ];
                    }
                }
            }
        }

        private static string CreateClassName(string type, string conversion, string underlyingType) =>
            Normalize($"escapedTests{type}{conversion}{underlyingType}");

        private static string Normalize(string input) => 
            input.Replace(" ", "_").Replace("|", "_").Replace(".", "_").Replace("@", "_");
        
        // for each of the types above, create classes for each one of these attributes
        private readonly string[] _conversions = new[]
        {
            "Conversions.None",
            "Conversions.NewtonsoftJson",
            "Conversions.NewtonsoftJson | Conversions.SystemTextJson | Conversions.EfCoreValueConverter | Conversions.DapperTypeHandler | Conversions.LinqToDbValueConverter",
        };

        // for each of the attributes above, use this underlying type
        private readonly string[] _underlyingTypes = 
        {
            "byte",
            "double",
            "System.Guid",
            "string",
            "record.@struct.@float.@decimal",
            "record.@struct.@float.@event2",
            "record.@struct.@float.@event",
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task GenerationOfEscapedTypes(string type, string conversions, string underlyingType, string className)
    {
        var source =
            $$"""
              using Vogen;
              namespace @class
              {
                    namespace record.@struct.@float
                    {
                       public readonly record struct @decimal();
                       public readonly record struct @event2();
                       public readonly record struct @event();
                    }
                       
                     [ValueObject(conversions: {{conversions}}, underlyingType: typeof({{underlyingType}}))]
                     {{type}} {{className}} { }
                   
                     [EfCoreConverter<{{className}}>]
                     public partial class EfCoreConverters;
              }
              """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseHashedParameters(nameof(GenerationOfEscapedEfCoreConverters), conversions, underlyingType, className))
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public Task MixtureOfKeywords()
    {
        string declaration = """
using Vogen;

namespace record.@struct.@float
{
    public readonly record struct @decimal();
}

namespace @double
{
    public readonly record struct @decimal();

    [ValueObject(typeof(@decimal))]
    public partial class classFromEscapedNamespaceWithReservedUnderlyingType
    {
    }

    [ValueObject<int>]
    public partial class classFromEscapedNamespace
    {
    }


    [EfCoreConverter<@classFromEscapedNamespace>]
    [EfCoreConverter<classFromEscapedNamespaceWithReservedUnderlyingType>]
    public partial class EfCoreConverters;
}

namespace @bool.@byte.@short.@float.@object
{
    [ValueObject<int>]
    public partial class @class
    {
    }

    [ValueObject<string>]
    public partial class @event
    {
    }

    [ValueObject(typeof(record.@struct.@float.@decimal))]
    public partial class @event2
    {
    }
    
    [EfCoreConverter<@class>]
    [EfCoreConverter<@event>]
    [EfCoreConverter<@event2>]
    public partial class EfCoreConverters;
}
""";
        
        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(declaration)
            .RunOn(TargetFramework.Net8_0);
    }
}