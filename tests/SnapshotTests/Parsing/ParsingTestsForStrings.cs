using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.Parsing;

public class ParsingTestsForStrings
{
    [Fact]
    public Task Generates_IParsable()
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
    public Task Generates_IParsable_and_calls_our_validation_method()
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
    public Task Generates_IParsable_except_if_it_is_already_specified()
    {
        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource("""
                        using System;
                        using Vogen;

                        namespace Whatever;

                        [ValueObject<string>]
                        public partial struct City : IParsable<City>
                        {
                            public static City Parse(string s, IFormatProvider provider) => From(s);
                            public static bool TryParse(string s, IFormatProvider provider, out City result) => throw new NotImplementedException();
                        }

                        """)
            .IgnoreInitialCompilationErrors()
            .RunOn(TargetFramework.Net8_0);
    }


    [Fact]
    public Task Omits_parse_method_if_user_provides_exact_match()
    {
        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource("""
                        #nullable enable

                        using Vogen;
                        using System;

                        namespace Whatever;

                        [ValueObject(typeof(string))]
                        public partial struct MyClass
                        {
                            public static MyClass Parse(string s, IFormatProvider? provider)
                            {
                                throw new NotImplementedException();
                            }
                        }

                        """)
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public Task Writes_explicit_interface_implementation_if_user_provided_Parse_method()
    {
        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource("""
                        #nullable enable

                        using Vogen;
                        using System;

                        namespace Whatever;

                        [ValueObject(typeof(string))]
                        public partial struct MyClass
                        {
                            public static int Parse(string s, IFormatProvider? provider)
                            {
                                throw new NotImplementedException();
                            }
                        }

                        """)
            .RunOn(TargetFramework.Net8_0);
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

    [Fact]
    public Task Generates_IParsable_for_record_structs()
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

    private static Task RunTest(string source) =>
        new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net8_0);
        
}