﻿using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.GeneralStuff;

// contrib: An ideal place to start a new feature. Write a new test for the feature here to get it working, then
// add more tests. Move these tests if there are several of them, and it makes sense to group them.

public class GeneralTests
{
    [Fact]
    public async Task Generates_LinqPadToDump()
    {
        var source =
            $$"""

            using System;
            using Vogen;
            
            namespace Whatever;
            
            //[ValueObject(customizations: Customizations.LinqPadToDump)]
            [ValueObject()]
            public partial struct MyId;
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Generates_messagepack()
    {
        var source =
            $$"""

            using System;
            using Vogen;
            
            namespace Whatever;
            
            [ValueObject(conversions: Conversions.MessagePack)]
            public partial struct MyId;
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .WithPackage(new NuGetPackage("MessagePack", "2.5.187", "lib/netstandard2.0" ))
                .WithPackage(new NuGetPackage("MessagePack.Annotations", "2.5.192", "lib/netstandard2.0" ))
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Generates_messagepack_markers()
    {
        var source =
            $$"""

            using System;
            using Vogen;
            
            namespace @double;
            
            [ValueObject<int>(conversions: Conversions.None)]
            public partial struct MyId;
            
            [MessagePack<MyId>]
            public partial class MyMarkers;
            
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .WithPackage(new NuGetPackage("MessagePack", "2.5.187", "lib/netstandard2.0" ))
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Generates_messagepack_and_efcore_markers()
    {
        var source =
            $$"""

            using System;
            using Vogen;
            
            namespace @double;
            
            [ValueObject<int>(conversions: Conversions.None)]
            public partial struct MyId;
            
            [MessagePack<MyId>]
            [EfCoreConverter<MyId>]
            public partial class MyMarkers;
            
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .WithPackage(new NuGetPackage("MessagePack", "2.5.187", "lib/netstandard2.0" ))
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Skips_ToString_methods_that_implement_an_interface()
    {
        var source =
            $$"""

            using System;
            using Vogen;
            
            [ValueObject]
            public partial struct MyId;
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Hoists_ISpanFormattable()
    {
        var source =
            $$"""

            using System;
            using Vogen;
            
            [ValueObject<DateTimeOffset>]
            public partial struct Dob;
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Hoists_parameter_attributes_from_underlying_tostring_methods()
    {
        var source =
            $$"""

            using System;
            using Vogen;
            
            [ValueObject<DateTimeOffset>]
            public partial struct Dob;
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Does_not_include_inheritdoc_if_underlying_has_no_tostring()
    {
        var source =
            $$"""

            using System;
            using Vogen;
            
            public class D;
            
            [ValueObject<D>]
            public partial struct Vo;
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task No_validation()
    {
        var source =
            $$"""
            #define VOGEN_NO_VALIDATION

            using System;
            using Vogen;
            
            public class C<T>
            {
            }

            #nullable enable
            [ValueObject<C<int>>]
            public partial class MyVo;
            
            #nullable restore
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Disable_stack_trace_in_debug()
    {
        var source =
            """
            using System;
            using Vogen;

            [assembly: VogenDefaults(disableStackTraceRecordingInDebug: true)]

            [ValueObject]
            public partial class MyVo;
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Theory]
    [InlineData("struct")]
    [InlineData("class")]
    [InlineData("record struct")]
    [InlineData("record class")]
    public async Task Nullable_enabled_if_set_in_scope(string type)
    {
        var source =
            $$"""
            #nullable enable

            using System;
            using Vogen;

            [ValueObject]
            public partial {{type}} MyVo;
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename($"{nameof(Nullable_enabled_if_set_in_scope)}{type}")))
                .RunOn(TargetFramework.Net8_0);
    }

    [Theory]
    [InlineData("struct")]
    [InlineData("class")]
    [InlineData("record struct")]
    [InlineData("record class")]
    public async Task Nullable_try_parse_if_validation_supplied(string type)
    {
        var source =
            $$"""
            #nullable enable

            using System;
            using Vogen;

            [ValueObject]
            public partial {{type}} MyVo
            {
                private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("Must be > 0");
            }
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename($"{nameof(Nullable_try_parse_if_validation_supplied)}{type}")))
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Can_specify_a_generic_underlying()
    {
        var source =
            """
            using System;
            using Vogen;

            public class C<T>
            {
            }

            [ValueObject<C<int>>]
            public partial class MyVo;
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Nullable_Equals_override_with_object()
    {
        var source =
            """
            #nullable disable
            
            using System;
            using Vogen;

            [ValueObject<DateTimeOffset>]
            public partial struct MyVo;
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Nullable_enabled_does_not_allow_null_for_Parse_methods_for_underlying_string()
    {
        var source =
            """
            #nullable disable
            
            using System;
            using Vogen;

            [ValueObject<string>(comparison:ComparisonGeneration.UseUnderlying, parsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces, parsableForStrings: ParsableForStrings.GenerateMethodsAndInterface)]
            public partial class C;
            
            """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Can_specify_swashbuckle_filter_generation_for_openapi()
    {
        var source = $$"""

                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter)]
                     
                       [ValueObject]
                       public partial class MyVo { }

                     """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.AspNetCore8_0);
    }

    [Fact]
    public async Task Can_specify_swashbuckle_MapType_extension_method_generation_for_openapi()
    {
        var source = $$"""

                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod)]
                     
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
                .RunOn(TargetFramework.AspNetCore8_0);
    }

    [Fact]
    public async Task Can_specify_both_swashbuckle_filter_and_MapType_extension_method_generation_for_openapi()
    {
        var source = $$"""

                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(openApiSchemaCustomizations: 
                        OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod | 
                        OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter)]
                     
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
                .RunOn(TargetFramework.AspNetCore8_0);
    }

    [Theory]
    [InlineData("struct")]
    [InlineData("class")]
    [InlineData("record struct")]
    [InlineData("record class")]
    public async Task Can_specify_a_factory_method_for_wrappers_for_guids(string type)
    {
        var source = $$"""

                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(customizations: Customizations.AddFactoryMethodForGuids)]
                     
                       [ValueObject<Guid>]
                       public partial {{type}} MyVo { }

                     """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(type)))
                .RunOnAllFrameworks();
    }

    [Fact]
    public async Task ServiceStackDotTextConversion_generates_static_constructor_for_strings()
    {
        var source = @"using Vogen;
  [ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(string))]
  public partial struct MyVo { }
";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task ServiceStackDotTextConversion_generates_static_constructor_for_non_strings()
    {
        var source = @"using Vogen;
  [ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(int))]
  public partial struct MyVo { }
";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            // .WithPackage(new NuGetPackage("ServiceStack.Text", "8.2.2", "lib/net8.0" ))
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task ServiceStackDotTextConversion_generates_static_constructor_for_time_related_primitives()
    {
        var source = @"using System;
  using Vogen;
  [ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(TimeOnly))]
  public partial struct MyVo { }
";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task ServiceStackDotTextConversion_generates_static_constructor_for_date_time()
    {
        var source = @"using System;
  using Vogen;
  [ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateTime))]
  public partial struct MyVo { }
";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Can_use_derived_ValueObjectAttribute()
    {
        var source = @"using System;
  using Vogen;

public class IntVoAttribute : ValueObjectAttribute<int>
{
}

  [IntVo]
  public partial struct MyVo { }

internal class C
{
    C() {
        MyVo.From(123);
    }
}
";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task EFCore_generated_stuff()
    {
        var source = @"using Vogen;
  [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(string))]
  public partial struct MyVo { }
";

        await RunTest(source);
    }

    [Fact]
    public async Task User_supplied_GetHashCode()
    {
        var source = """
                     using Vogen;
                     
                     [ValueObject<string>]
                     public readonly partial record struct VoWithGetHashCode
                     {
                         public override int GetHashCode()
                         {
                             // custom code
                             return 0;
                         }
                     }

                     """;

        await RunTest(source);
    }

    [Fact]
    public async Task User_supplied_Equals()
    {
        var source = @"using Vogen;
[ValueObject(typeof(string))]
public readonly partial record struct MyVo
{
    public readonly bool Equals(MyVo other)
    {
        // custom code
        return Value == other.Value;
    }

    public readonly bool Equals(string other)
    {
        // custom code
        return Value == other;
    }
}
";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [SkippableFact]
    public async Task User_supplied_Equals_generic()
    {
        var source = @"using Vogen;
[ValueObject<string>]
public readonly partial record struct MyVo
{
    public readonly bool Equals(MyVo other)
    {
        // custom code
        return Value == other.Value;
    }

    public readonly bool Equals(string other)
    {
        // custom code
        return Value == other;
    }
}
";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [Fact]
    public async Task Bug_fix_516_user_can_provide_Equals_and_GetHashCode()
    {
        var source = @"using Vogen;
[ValueObject(typeof(string))]
public readonly partial record struct BugFix516Vo
{
    public readonly bool Equals(BugFix516Vo other)
    {
        // custom code
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        // custom code
        return 0;
    }
}";

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Partial_record_struct_created_successfully()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(typeof(string))]
public readonly partial record struct CustomerId
{
}";

        return RunTest(source);
    }

    [Fact]
    public Task No_stack_trace_recording()
    {
        var source = @"using Vogen;
[assembly: VogenDefaults(disableStackTraceRecordingInDebug: true)]

namespace Whatever;

[ValueObject(typeof(string))]
public readonly partial record struct CustomerId
{
}";

        return RunTest(source);
    }

    [Fact]
    public Task No_casting()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(typeof(string), toPrimitiveCasting: CastOperator.Implicit, fromPrimitiveCasting: CastOperator.None)]
public partial class MyVo { }
";

        return RunTest(source);
    }

    [Fact]
    public Task No_is_initialized_method()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(isInitializedMethodGeneration: IsInitializedMethodGeneration.Omit)]
public partial class MyVo { }
";

        return RunTest(source);
    }

    [Fact]
    public Task With_is_initialized_method()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(isInitializedMethodGeneration: IsInitializedMethodGeneration.Generate)]
public partial class MyVo { }
";

        return RunTest(source);
    }

    [Fact]
    public Task Partial_struct_created_successfully()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
}";

        return RunTest(source);
    }

    [Fact]
    public Task No_namespace() =>
        RunTest(@"using Vogen;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
}");


    [Fact]
    public Task Produces_instances() =>
        RunTest(@"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
[Instance(name: ""Unspecified"", value: -1, tripleSlashComment: ""a short description that'll show up in intellisense"")]
[Instance(name: ""Unspecified1"", value: -2)]
[Instance(name: ""Unspecified2"", value: -3, tripleSlashComment: ""<some_xml>whatever</some_xml"")]
[Instance(name: ""Unspecified3"", value: -4)]
[Instance(name: ""Cust42"", value: 42)]
public partial struct CustomerId
{
}
");

    [Fact]
    public Task Validation_with_PascalCased_validate_method() =>
        RunTest(@"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
");

    [Fact]
    public Task Validation_with_camelCased_validate_method() =>
        RunTest(@"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
");

    [Fact]
    public Task Validation_can_have_fully_qualified_return_type() =>
        RunTest(@"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static Vogen.Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
");


    [Fact]
    public Task Namespace_names_can_have_reserved_keywords() =>
        RunTest(@"using Vogen;

namespace @double;

[ValueObject]
[Instance(name: ""@struct"", value: 42)]
[Instance(name: ""@double"", value: 52)]
[Instance(name: ""@event"", value: 69)]
[Instance(name: ""@void"", value: 666)]
public partial struct @class
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
");

    [Fact]
    public Task Generates_IParsable()
    {
        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource("""
                        using Vogen;

                        namespace Whatever;

                        [ValueObject(typeof(bool))]
                        public sealed partial class Foo { }
                        """)
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public Task Generates_IParsable_and_skip_user_provided_method_1()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass
            {
                public static MyClass Parse(string s, IFormatProvider provider)
                {
                    throw new NotImplementedException();
                }
            }

            """);
    }

    [Fact]
    public Task Generates_IParsable_and_skip_user_provided_method_2()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass
            {
                public static MyClass Parse(string s, IFormatProvider provider)
                {
                    throw new NotImplementedException();
                }
                
                public static MyClass Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) 
                {
                    throw new NotImplementedException();
                }
            }

            """);
    }
    
    [Fact]
    public Task Generates_IParsable_and_skip_user_provided_method_3()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass
            {
                public static MyClass Parse(string s)
                {
                    throw new NotImplementedException();
                }
            }

            """);
    }

    [Fact]
    public Task Generates_IParsable_and_skip_user_provided_method_4()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass
            {
                public static MyClass Parse(string s) => throw new NotImplementedException();
            }

            """);
    }
    
    

    [Fact]
    public Task Generates_IParsable_for_a_class_wrapping_an_int()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject(typeof(int))]
            public partial class MyVo
            {
            }
            """);
    }

    [Fact]
    public Task Does_not_generate_IParsable_in_versions_of_dotnet_prior_to_7()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject(typeof(int))]
            public partial class MyVo
            {
            }
            """);
    }

    [Fact]
    public Task Generates_IParsable_for_a_class__wrapping_a_string()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject(typeof(string))]
            public partial class MyVo
            {
            }
            """);
    }

    [Fact]
    public Task Generates_IParsable_for_record_structs_wrapping_a_string()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject(typeof(string))]
            public readonly partial record struct MyVo
            {
            }
            """);
    }

    [Fact]
    public Task Generates_IParsable_and_skip_user_provided_method_5()
    {
        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource("""
                        using Vogen;
                        using System;

                        namespace Whatever;

                        [ValueObject<int>]
                        public partial struct MyClass2
                        {
                            public static bool TryParse(string input, out MyClass2 s) => throw new NotImplementedException();
                        }

                        """)
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public Task Generates_SystemTextJson_factory()
    {
        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource("""
                        using Vogen;
                        using System;

                        namespace Whatever;

                        [ValueObject<int>]
                        public partial struct MyClass1
                        {
                        }

                        [ValueObject<int>]
                        public partial struct MyClass2
                        {
                        }

                        """)
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public Task Ignores_TryParse_where_last_parameter_is_not_out()
    {
        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource("""
                        using Vogen;
                        using System;

                        namespace Whatever;

                        [ValueObject<int>]
                        public partial struct MyClass2
                        {
                            public static bool TryParse(string input, MyClass2 s) => throw new NotImplementedException();
                        }

                        """)
            .RunOn(TargetFramework.Net8_0);
    }

    private static Task RunTest(string source) =>
        new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
        
}