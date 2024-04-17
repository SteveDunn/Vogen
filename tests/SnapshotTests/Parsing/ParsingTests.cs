using System.Threading.Tasks;
using Shared;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.Parsing;

[UsesVerify]
public class ParsingTests
{
    [Fact]
    public Task Generates_IParsable_for_strings()
    {
        return RunTest(
            """
            using Vogen;

            namespace Whatever;

            [ValueObject<string>]
            public partial struct MyClass
            {
            }

            """);
    }

    [Fact]
    public Task Generates_IParsable_for_strings_and_calls_our_validation_method()
    {
        return RunTest(
            """
            using Vogen;

            namespace Whatever;

            [ValueObject<string>]
            public partial struct MyClass
            {
                public static Validation Validate(string input) => throw null;
            }

            """);
    }

    [Fact]
    public Task Generates_IParsable_for_strings_except_if_it_is_already_specified()
    {
        return RunTest(
            """
            using System;
            using Vogen;

            namespace Whatever;

            [ValueObject<string>]
            public partial struct City : IParsable<City>
            {
                public static City Parse(string s, IFormatProvider provider) => From(s);
                public static bool TryParse(string s, IFormatProvider provider, out City result) => throw new NotImplementedException();
            }

            """);

        static Task RunTest(string source) =>
            new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.Net7_0, TargetFramework.Net8_0);
    }


    [Fact]
    public Task Generates_IParsable()
    {
        return RunTest(
            """
            using Vogen;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass
            {
            }

            """);
    }

    [Fact]
    public Task Skips_user_provided_Parse_method()
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
    public Task Skip_user_provided_multiple_Parse_methods()
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
    public Task Skips_user_provided_Parse_method_with_one_parameter()
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
    public Task Skip_user_provided_expression_bodied_method_with_one_parameter()
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
    public Task Generates_IParsable_for_a_class_wrapping_a_string()
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

    // If any of the Parse/TryParse methods on the primitive are private, we can't call them.
    // Since we can't call them, we can't implement them.
    [Fact]
    public Task Does_not_generate_IParsable_for_a_class_wrapping_a_bool_because_it_implements_it_privately()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject(typeof(bool))]
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
    public Task Skip_user_provided_TryParse_method()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass2
            {
                public static bool TryParse(string input, out MyClass2 s) => throw new NotImplementedException();
            }

            """);
    }

    [Fact]
    public Task Ignores_TryParse_where_last_parameter_is_not_out()
    {
        return RunTest(
            """
            using Vogen;
            using System;

            namespace Whatever;

            [ValueObject<int>]
            public partial struct MyClass2
            {
                public static bool TryParse(string input, MyClass2 s) => throw new NotImplementedException();
            }

            """);
    }

    private static Task RunTest(string source) =>
        new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
        
}