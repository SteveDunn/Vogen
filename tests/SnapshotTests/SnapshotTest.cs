using FluentAssertions;
using MediumTests.DiagnosticsTests;
using Vogen;

namespace MediumTests.SnapshotTests;

[UsesVerify]
public class ValueObjectGeneratorTests
{
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

    private static async Task RunTest(string source)
    {
        // await RunTest(source, TargetFramework.NetStandard2_0); // works!
        // await RunTest(source, TargetFramework.NetStandard2_1); // works!
        // await RunTest(source, TargetFramework.NetCoreApp3_1); // does not work (when host is net7)!
        // await RunTest(source, TargetFramework.Net4_6_1); // works!
        // await RunTest(source, TargetFramework.Net4_8); // works!
        // await RunTest(source, TargetFramework.Net5_0); // does not work (when host is net7)
        // await RunTest(source, TargetFramework.Net6_0); // does not work (when host is net7)
        await RunTest(source, TargetFramework.Net7_0); // works!
    }

    private static Task RunTest(string source, TargetFramework targetFramework)
    {
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source, targetFramework);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output)
            .UseDirectory(SnapshotUtils.GetSnapshotDirectoryName(targetFramework));
    }

    [Fact]
    public Task No_namespace() =>
        RunTest(@"using Vogen;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
}");


    [Fact]
    public Task Produces_instances()
    {
        return RunTest(@"using Vogen;

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
    }

    [Fact]
    public Task Validation_with_PascalCased_validate_method()
    {
        return RunTest(@"using Vogen;

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
    }

    [Fact]
    public Task Validation_with_camelCased_validate_method()
    {
        return RunTest(@"using Vogen;

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
    }

    [Fact]
    public Task Instance_names_can_have_reserved_keywords()
    {
        return RunTest(@"using Vogen;

namespace Whatever;

[ValueObject]
[Instance(name: ""@class"", value: 42)]
[Instance(name: ""@event"", value: 69)]
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
    }

    [Fact]
    public Task Basic_test()
    {
        return RunTest(@"using Vogen;

namespace Whatever;

[ValueObject(typeof(string))]
public partial struct CustomerId
{
}
");
    }

    [Fact]
    public Task Namespace_names_can_have_reserved_keywords()
    {
        return RunTest(@"using Vogen;

namespace @double;

[ValueObject]
[Instance(name: ""@class"", value: 42)]
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
    }
}

[UsesVerify]
public class ValueObjectGeneratorTests_GenerateFromGenericAttribute
{
#if NET7_0_OR_GREATER
    [Fact]
    public Task Partial_struct_created_successfully()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject<int>]
public partial struct CustomerId
{
}";

        return RunTest(source);
    }

    private static Task RunTest(string source)
    {
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }

    [Fact]
    public Task No_namespace() =>
        RunTest(@"using Vogen;

[ValueObject<int>]
public partial struct CustomerId
{
}");


    [Fact]
    public Task Produces_instances()
    {
        return RunTest(@"using Vogen;

namespace Whatever;

[ValueObject<int>]
[Instance(name: ""Unspecified"", value: -1, tripleSlashComment: ""a short description that'll show up in intellisense"")]
[Instance(name: ""Unspecified1"", value: -2)]
[Instance(name: ""Unspecified2"", value: -3, tripleSlashComment: ""<some_xml>whatever</some_xml"")]
[Instance(name: ""Unspecified3"", value: -4)]
[Instance(name: ""Cust42"", value: 42)]
public partial struct CustomerId
{
}
");
    }

    [Fact]
    public Task Validation_with_PascalCased_validate_method()
    {
        return RunTest(@"using Vogen;

namespace Whatever;

[ValueObject<int>]
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
    }

    [Fact]
    public Task Validation_with_camelCased_validate_method()
    {
        return RunTest(@"using Vogen;

namespace Whatever;

[ValueObject<int>]
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
    }

    [Fact]
    public Task Instance_names_can_have_reserved_keywords()
    {
        return RunTest(@"using Vogen;

namespace Whatever;

[ValueObject<int>]
[Instance(name: ""@class"", value: 42)]
[Instance(name: ""@event"", value: 69)]
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
    }

    [Fact]
    public Task Namespace_names_can_have_reserved_keywords()
    {
        return RunTest(@"using Vogen;

namespace @double;

[ValueObject<int>]
[Instance(name: ""@class"", value: 42)]
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
    }
#endif
}