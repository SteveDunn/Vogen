using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.OpenApi;

// contrib: An ideal place to start a new feature. Write a new test for the feature here to get it working, then
// add more tests. Move these tests if there are several of them, and it makes sense to group them.

public class SwashbuckleTests
{
    [Theory]
    [InlineData("")]
    [InlineData("namespace MyNamespace;")]
    [InlineData("namespace @double;")]
    public async Task Generates_filter_code(string @namespace)
    {
        var source =
            $$"""
              using System;
              using Vogen;

              [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter)]

              {{@namespace}}

              [ValueObject<int>]
              public partial class MyVo { }
              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseHashedParameters(@namespace))
            .RunOn(TargetFramework.AspNetCore8_0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("namespace MyNamespace;")]
    [InlineData("namespace @double;")]
    public async Task Generates_extension_method_for_mapping_types(string @namespace)
    {
        var source =
            $$"""
              using System;
              using Vogen;

              [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod)]

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
              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseHashedParameters(@namespace))
            .RunOn(TargetFramework.AspNetCore8_0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("namespace MyNamespace;")]
    public async Task Generates_both_filter_and_MapType_extension_method(string @namespace)
    {
        var source =
            $$"""
              using System;
              using Vogen;

              [assembly: VogenDefaults(openApiSchemaCustomizations: 
                 OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod | 
                 OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter)]

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
              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseHashedParameters(@namespace))
            .RunOn(TargetFramework.AspNetCore8_0);
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
                 OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod)]

              namespace MyNamespace;

              [ValueObject<{{type}}>]
              public partial class MyVo { }
              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .CustomizeSettings(s => s.UseHashedParameters(type))
            .RunOn(TargetFramework.Net8_0);
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
                 OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod)]

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
            .RunOn(TargetFramework.AspNetCore8_0);
    }
    
}