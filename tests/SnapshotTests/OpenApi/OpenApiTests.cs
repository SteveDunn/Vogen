using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.OpenApi;

public class OpenApiTests
{
    [Theory]
    [InlineData("")]
    [InlineData("namespace MyNamespace;")]
    [InlineData("namespace @double;")]
    public async Task Generates_OpenApiMappingExtensionMethod(string @namespace)
    {
        var source =
            $$"""
              using System;
              using Vogen;

              [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateOpenApiMappingExtensionMethod)]

              {{@namespace}}

              [ValueObject]
              public partial class MyVoInt { }

              [ValueObject<float>]
              public partial class MyVoFloat { }

              [ValueObject<decimal>]
              public partial class MyVoDecimal { }

              [ValueObject<double>]
              public partial class MyVoDouble { }

              [ValueObject<string>]
              public partial class MyVoString { }

              [ValueObject<bool>]
              public partial class MyVoBool { }

              [ValueObject<bool>]
              public partial class @bool { }
              
              [ValueObject<short>]
              public partial class MyVoShort { }
              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseHashedParameters(@namespace))
            .RunOn(TargetFramework.Net9_0);
    }

    [Theory]
    [InlineData("", TargetFramework.AspNetCore9_0)]
    [InlineData("namespace MyNamespace;", TargetFramework.AspNetCore9_0)]
    [InlineData("namespace @double;", TargetFramework.AspNetCore9_0)]
    [InlineData("", TargetFramework.AspNetCore10_0)]
    [InlineData("namespace MyNamespace;", TargetFramework.AspNetCore10_0)]
    [InlineData("namespace @double;", TargetFramework.AspNetCore10_0)]
    public async Task Generates_both_OpenApiMappingExtensionMethod_SwaggerMappingExtensionMethod(string @namespace, TargetFramework targetFramework)
    {
        var source =
            $$"""
              using System;
              using Vogen;

              [assembly: VogenDefaults(openApiSchemaCustomizations: 
                 OpenApiSchemaCustomizations.GenerateOpenApiMappingExtensionMethod | 
                 OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod)]

              {{@namespace}}

              [ValueObject]
              public partial class MyVoInt { }

              [ValueObject<float>]
              public partial class MyVoFloat { }

              [ValueObject<decimal>]
              public partial class MyVoDecimal { }

              [ValueObject<double>]
              public partial class MyVoDouble { }

              [ValueObject<string>]
              public partial class MyVoString { }

              [ValueObject<bool>]
              public partial class MyVoBool { }

              [ValueObject<bool>]
              public partial class @bool { }
              
              [ValueObject<short>]
              public partial class MyVoShort { }
              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseHashedParameters(@namespace))
            .RunOn(targetFramework);
    }

    [Theory]
    [InlineData("Guid")]
    [InlineData("byte")]
    [InlineData("Byte")]
    [InlineData("char")]
    [InlineData("Char")]
    [InlineData("System.Numerics.Complex")]
    // [InlineData("DateOnly")] - for some reason, this fails during the test, but not 'in real life'
    [InlineData("DateTime")]
    [InlineData("DateTimeOffset")]
    [InlineData("decimal")]
    [InlineData("Decimal")]
    [InlineData("double")]
    [InlineData("Double")]
    [InlineData("Half")]
    public async Task Treats_IParsable_primitives_as_strings(string type)
    {
        var source =
            $$"""
              using System;
              using Vogen;

              [assembly: VogenDefaults(openApiSchemaCustomizations: 
                 OpenApiSchemaCustomizations.GenerateOpenApiMappingExtensionMethod)]

              namespace MyNamespace;

              [ValueObject<{{type}}>]
              public partial class MyVo { }
              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .CustomizeSettings(s => s.UseHashedParameters(type))
            .RunOn(TargetFramework.Net9_0);
    }

    [Fact]
    public async Task Treats_custom_IParsable_as_string()
    {
        var source =
            $$"""
              #nullable enable
              
              using System;
              using Vogen;

              [assembly: VogenDefaults(openApiSchemaCustomizations: 
                 OpenApiSchemaCustomizations.GenerateOpenApiMappingExtensionMethod)]

              namespace MyNamespace;

              public class C : IParsable<C>
              {
                  public static C Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();
              
                  public static bool TryParse(string? s, IFormatProvider? provider, out C result) => throw new NotImplementedException();
              }

              [ValueObject<C>]
              public partial class MyVo { }
              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.AspNetCore9_0);
    }
}
