using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.Config;

[UsesVerify] 
public class GlobalConfigTests
{
    [Fact]
    public Task Disable_stack_trace_recording_in_debug()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(disableStackTraceRecordingInDebug: true)]


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
    public Task Enable_stack_trace_recoding_in_debug()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(disableStackTraceRecordingInDebug: false)]


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
    public Task Type_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(float))]


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
    public Task OmitDebugAttributes_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(debuggerAttributes: DebuggerAttributeGeneration.Basic)]


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
    public Task Exception_override()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(throws: typeof(Whatever.MyValidationException))]

                     namespace Whatever;

                     [ValueObject]
                     public partial struct CustomerId
                     {
                         private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("xxxx");
                     }

                     public class MyValidationException : Exception
                     {
                         public MyValidationException(string message) : base(message) { }
                     }

                     """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Exception_override_in_different_namespace()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(throws: typeof(Whatever2.MyValidationException))]

                     namespace Whatever
                     {
                         [ValueObject]
                         public partial struct CustomerId
                         {
                             private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("xxxx");
                         }
                     }
                     
                     namespace Whatever2
                     {
                         public class MyValidationException : Exception
                         {
                             public MyValidationException(string message) : base(message) { }
                         }
                     }

                     """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Customization_override()
    {
        var source = @"
#pragma warning disable CS0618 // 'Customizations.TreatNumberAsStringInSystemTextJson' is obsolete
using System;
using Vogen;

[assembly: VogenDefaults(customizations: Customizations.TreatNumberAsStringInSystemTextJson)]

namespace Whatever;

[ValueObject]
public partial struct CustomerId { }
";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Conversion_and_exceptions_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(conversions: Conversions.DapperTypeHandler, throws: typeof(Whatever.MyValidationException))]


namespace Whatever;

[ValueObject]
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
    public Task Override_all()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(
                        underlyingType: typeof(string), 
                        conversions: Conversions.None, 
                        throws:typeof(Whatever.MyValidationException),
                        tryFromGeneration: TryFromGeneration.Omit)]

                     namespace Whatever;

                     [ValueObject]
                     public partial struct CustomerId
                     {
                         private static Validation Validate(string value) => value.Length > 0 ? Validation.Ok : Validation.Invalid("xxxx");
                     }

                     public class MyValidationException : Exception
                     {
                         public MyValidationException(string message) : base(message) { }
                     }

                     """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }
}
