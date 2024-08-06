using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.Parsing;

public class ParsingTestsForPrimitives
{
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