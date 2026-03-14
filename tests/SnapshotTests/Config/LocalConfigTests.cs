using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.Config;

public class LocalConfigTests
{
    [Fact]
    public Task OmitDebugAttributes_override()
    {
        // language=c#
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(debuggerAttributes: DebuggerAttributeGeneration.Basic)]
public partial struct CustomerId
{
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Defaults()
    {
        // language=c#
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Defaults_with_validation()
    {
        // language=c#
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

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Defaults_with_validation_and_instances()
    {
        // language=c#
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

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Type_override()
    {
        // language=c#
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(typeof(float))]
public partial struct CustomerId
{
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Exception_override()
    {
        // language=c#
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

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Conversion_override()
    {
        // language=c#
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(conversions: Conversions.None)]
public partial struct CustomerId { }";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Conversion_and_exceptions_override()
    {
        // language=c#
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

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Override_global_config_locally()
    {
        // language=c#
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

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }
}
