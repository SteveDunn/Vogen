using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Vogen;
using Xunit;

namespace MediumTests.SnapshotTests.Config;

[UsesVerify]
public class LocalConfigTests
{
    [Fact]
    public Task Defaults()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }

    [Fact]
    public Task Defaults_with_validation()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }

    [Fact]
    public Task Defaults_with_validation_and_instances()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject]
[Instance(name: ""Zero"", value: 0, tripleSlashComment: ""a short description that'll show up in intellisense"")]
public partial struct CustomerId
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }

    [Fact]
    public Task Type_override()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(typeof(float))]
public partial struct CustomerId
{
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }

    [Fact]
    public Task Exception_override()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(throws: typeof(MyValidationException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(0);

        return Verifier.Verify(output).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }

    [Fact]
    public Task Conversion_override()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(conversions: Conversions.None)]
public partial struct CustomerId { }";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }

    [Fact]
    public Task Conversion_and_exceptions_override()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(conversions: Conversions.DapperTypeHandler, throws: typeof(Whatever.MyValidationException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}


public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(0);

        return Verifier.Verify(output).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }

    [Fact]
    public Task Override_global_config_locally()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(string), conversions: Conversions.None, throws:typeof(Whatever.MyValidationException))]

namespace Whatever;

[ValueObject(underlyingType:typeof(float))]
public partial struct CustomerId
{
    private static Validation Validate(float value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }
}
